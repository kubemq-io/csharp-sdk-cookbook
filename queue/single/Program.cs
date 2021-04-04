using System;
using System.Collections.Generic;
using System.Threading;

namespace single
{
    class Program
    {
        static void Main(string[] args)
        {
            string QueueName = "queue",
             KubeMQServerAddress = "localhost:50000";

            var  queue = new KubeMQ.SDK.csharp.Queue.Queue(QueueName, "Csharp-sdk-cookbook-queues-single-client", KubeMQServerAddress);
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
                    Console.WriteLine($"message  error:{res.Error}");
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

            var msg = queue.ReceiveQueueMessages();
            if (msg.IsError)
            {
                Console.WriteLine($"message error, error:{msg.Error}");
            }
            foreach (var item in msg.Messages)
            {
                Console.WriteLine($"message received body:{KubeMQ.SDK.csharp.Tools.Converter.FromByteArray(item.Body)}");

            }
            Console.WriteLine("DONE");
            Console.ReadLine();

        }
    }
   
}
