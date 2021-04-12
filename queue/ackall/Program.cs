using System;
using System.Collections.Generic;
using System.Threading;

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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                System.Environment.Exit(1);
            }

            Thread.Sleep(1000);

            var ackmsg = queue.AckAllQueueMessages();
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
