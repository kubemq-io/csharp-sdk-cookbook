using KubeMQ.SDK.csharp.Queue;
using System;
using System.Collections.Generic;
using System.Threading;

namespace deadLetter
{
    class Program
    {
        static void Main(string[] args)
        {
            string QueueName = "queue-dead-letter",
             KubeMQServerAddress = "localhost:50000";
            var queue = new KubeMQ.SDK.csharp.Queue.Queue(QueueName, "Csharp-sdk-cookbook-queues-dead-letter-client", KubeMQServerAddress);
            try
            {
                var res = queue.SendQueueMessage(new KubeMQ.SDK.csharp.Queue.Message
                {
                    Body = KubeMQ.SDK.csharp.Tools.Converter.ToByteArray("hi, new message"),
                    Metadata = "some-metadata",
                    Tags = new Dictionary<string, string>()
                {
                    {"Action",$"SendQueueMessage" }
                }
                });
                if (res.IsError)
                {
                    Console.WriteLine($"message enqueue error, error:{res.Error}");
                }
                else
                {
                    Console.WriteLine($"message sent at, {res.SentAt}");
                }

                //Simple send Bulk of messages
                List<Message> msgs = new List<Message>();
                for (int i = 0; i < 5; i++)
                {
                    msgs.Add(new KubeMQ.SDK.csharp.Queue.Message
                    {
                        MessageID = i.ToString(),
                        Body = KubeMQ.SDK.csharp.Tools.Converter.ToByteArray($"im Message {i}"),
                        Metadata = "some-metadata",
                        Tags = new Dictionary<string, string>()/* ("Action", $"dead-letter_{testGui}_{i}")*/ 
                    {
                        {"Action",$"dead-letter_{i}"}
                    }
                    });
                }

                var resSend = queue.SendQueueMessage(new KubeMQ.SDK.csharp.Queue.Message
                {
                    Body = KubeMQ.SDK.csharp.Tools.Converter.ToByteArray("some-simple_queue-queue-message"),
                    Metadata = "emptyMeta",
                    Policy = new KubeMQ.Grpc.QueueMessagePolicy
                    {
                        MaxReceiveCount = 3,
                        MaxReceiveQueue = "DeadLetterQueue"
                    }
                });
                if (resSend.IsError)
                {
                    Console.WriteLine($"Message enqueue error, error:{resSend.Error}");
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                System.Environment.Exit(1);
            }
            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }
}
