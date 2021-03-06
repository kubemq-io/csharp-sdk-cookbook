using KubeMQ.SDK.csharp.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using KubeMQ.Grpc;

namespace delayed
{
    class Program
    {
        static void Main(string[] args)
        {
            string QueueName = "queue-delayed",
             KubeMQServerAddress = "localhost:50000";
            var queue = new KubeMQ.SDK.csharp.Queue.Queue(QueueName, "Csharp-sdk-cookbook-queues-delayed-client", KubeMQServerAddress);
            try
            {
                //Simple send Bulk of messages
                List<Message> msgs = new List<Message>();
                for (int i = 0; i < 1000; i++)
                {
                    msgs.Add(new KubeMQ.SDK.csharp.Queue.Message
                    {
                        MessageID = i.ToString(),
                        Policy =   new  QueueMessagePolicy()
                        {
                            DelaySeconds = 10
                        },
                        Body = KubeMQ.SDK.csharp.Tools.Converter.ToByteArray($"im Message {i}"),
                        Metadata = "some-metadata",
                        Tags = new Dictionary<string, string>()/* ("Action", $"Batch_{testGui}_{i}")*/ 
                    {
                        {"Action",$"Batch_{i}"}
                    }
                    });
                }

                //Batch send messages
                var resBatch = queue.Batch(msgs);
                if (resBatch.HaveErrors)
                {
                    Console.WriteLine($"message sent batch has errors");
                    System.Environment.Exit(1);
                }
                else
                {
                    Console.WriteLine($"{resBatch.Results.Count()} messages sent");    
                }
                
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                System.Environment.Exit(1);
            }

            Thread.Sleep(2000);
            try
            {
                Console.WriteLine("Pulling after 2 seconds");
                var msg = queue.Pull(1000,5);
                if (msg.IsError)
                {
                    Console.WriteLine($"message dequeue error, error:{msg.Error}");
                }
                {
                    Console.WriteLine($"{msg.Messages.Count()} messages received");    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Thread.Sleep(5000);
            try
            {
                Console.WriteLine("Pulling after 12 seconds");
                var msg = queue.Pull(1000,12);
                if (msg.IsError)
                {
                    Console.WriteLine($"message dequeue error, error:{msg.Error}");
                }
                {
                    Console.WriteLine($"{msg.Messages.Count()} messages received");    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
