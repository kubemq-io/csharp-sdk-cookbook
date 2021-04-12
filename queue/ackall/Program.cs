using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using KubeMQ.SDK.csharp.Queue;

namespace ackall
{
        class Program
    {
        static void Main(string[] args)
        {
            string QueueName = "queue-ackall",
                KubeMQServerAddress = "localhost:50000";

            var queue = new KubeMQ.SDK.csharp.Queue.Queue(QueueName, "Csharp-sdk-cookbook-queues-ackall-client", KubeMQServerAddress);
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

            var ackmsg = queue.AckAll(2);
            {
                if (ackmsg.IsError)
                {
                    Console.WriteLine($"message ack error, error:{ackmsg.Error}");
                }

                else
                {
                    Console.WriteLine($"Total Messages Acked:{ackmsg.AffectedMessages} ");    
                }

            }
        }
    }
}
