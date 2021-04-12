using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using KubeMQ.SDK.csharp.Queue;

namespace peek
{
   class Program
    {
        static void Main(string[] args)
        {
            string QueueName = "queue-peek",
             KubeMQServerAddress = "localhost:50000";
            var queue = new KubeMQ.SDK.csharp.Queue.Queue(QueueName, "Csharp-sdk-cookbook-queues-peek-client", null,12,KubeMQServerAddress,null);
            try
            {
                //Simple send Bulk of messages
                List<Message> msgs = new List<Message>();
                for (int i = 0; i < 1000; i++)
                {
                    msgs.Add(new KubeMQ.SDK.csharp.Queue.Message
                    {
                        MessageID = i.ToString(),
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

            Thread.Sleep(1000);

            try
            {
                Console.WriteLine("Peeking 1000 messages");
                var msg = queue.Peek(1000,2);
                if (msg.IsError)
                {
                    Console.WriteLine($"message dequeue error, error:{msg.Error}");
                }
                {
                    Console.WriteLine($"{msg.Messages.Count()} messages peeked");    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            try
            {
                Console.WriteLine("Pulling 1000 messages");
                var msg = queue.Peek(1000,2);
                if (msg.IsError)
                {
                    Console.WriteLine($"message dequeue error, error:{msg.Error}");
                }
                {
                    Console.WriteLine($"{msg.Messages.Count()} messages recevied");    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }
    }
}
