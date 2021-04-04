using System;
using System.Collections.Generic;
using System.Threading;

namespace expiration
{
    class Program
    {
        static void Main(string[] args)
        {
            string QueueName = "queue-expiration",
             KubeMQServerAddress = "localhost:50000";

            var queue = new KubeMQ.SDK.csharp.Queue.Queue(QueueName, "Csharp-sdk-cookbook-queues-expiration-client", 1, 1, KubeMQServerAddress);
            try
            {
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
                        ExpirationSeconds = 2
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                System.Environment.Exit(1);
            }

            Thread.Sleep(3000);

            var expirationmsg = queue.ReceiveQueueMessages( );
            {
                if (expirationmsg.IsError)
                {
                    Console.WriteLine($"message expiration error, error:{expirationmsg.Error}");
                }
                foreach (var item in expirationmsg.Messages)
                {
                    throw new Exception("found message after expiration!");
                }

            }
            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }
}
