using KubeMQ.SDK.csharp.Queue;
using System;
using System.Collections.Generic;
using System.Threading;

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
                queue.AckAllQueueMessages();
                var res = queue.SendQueueMessage(new KubeMQ.SDK.csharp.Queue.Message
                {
                    Body = KubeMQ.SDK.csharp.Tools.Converter.ToByteArray("hi, new message"),
                    Metadata = "some-metadata",
                    Tags = new Dictionary<string, string>()
                {
                    {"Action",$"SendQueueMessage" }
                },
                    Policy = new KubeMQ.Grpc.QueueMessagePolicy
                    {
                        DelaySeconds = 3
                    }
                });
                if (res.IsError)
                {
                    Console.WriteLine($"message error:{res.Error}");
                }
                else
                {
                    Console.WriteLine($"message sent at, {res.SentAt}");
                }

                var msg = queue.ReceiveQueueMessages();
                if (msg.IsError)
                {
                    Console.WriteLine($"message error, error:{msg.Error}");
                }
                foreach (var item in msg.Messages)
                {
                    throw new Exception("found message before delay!");
                }

                Thread.Sleep(4000);
                msg = queue.ReceiveQueueMessages();
                if (msg.IsError)
                {
                    Console.WriteLine($"message error, error:{msg.Error}");
                }
                foreach (var item in msg.Messages)
                {
                    Console.WriteLine($"message received body:{KubeMQ.SDK.csharp.Tools.Converter.FromByteArray(item.Body)}");
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
