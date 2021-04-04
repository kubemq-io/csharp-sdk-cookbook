using KubeMQ.SDK.csharp.Queue;
using System;
using System.Collections.Generic;
using System.Threading;

namespace batch
{
    class Program
    {
        static void Main(string[] args)
        {
            string QueueName = "queue-batch",
             KubeMQServerAddress = "localhost:50000";
            var queue = new KubeMQ.SDK.csharp.Queue.Queue(QueueName, "Csharp-sdk-cookbook-queues-batch-client", KubeMQServerAddress);
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
                        Tags = new Dictionary<string, string>()/* ("Action", $"Batch_{testGui}_{i}")*/ 
                    {
                        {"Action",$"Batch_{i}"}
                    }
                    });
                }

                //Batch send messages
                var resBatch = queue.SendQueueMessagesBatch(msgs);
                if (resBatch.HaveErrors)
                {
                    Console.WriteLine($"message sent batch has errors");
                }
                foreach (var item in resBatch.Results)
                {
                    if (item.IsError)
                    {
                        Console.WriteLine($"message enqueue error, error:{item.Error}");
                    }
                    else
                    {
                        Console.WriteLine($"message sent at, {item.SentAt}");
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                System.Environment.Exit(1);
            }

            Thread.Sleep(1000);

            var msg = queue.ReceiveQueueMessages();
            if (msg.IsError)
            {
                Console.WriteLine($"message dequeue error, error:{msg.Error}");
            }
            foreach (var item in msg.Messages)
            {
                Console.WriteLine($"message received body:{KubeMQ.SDK.csharp.Tools.Converter.FromByteArray(item.Body)}");

            }
        }
    }
}
