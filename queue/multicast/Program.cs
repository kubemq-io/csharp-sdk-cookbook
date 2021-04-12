using System;
using System.Collections.Generic;
using System.Threading;

namespace multicast
{
    class Program
    {
        static void Main(string[] args)
        {
            
            string KubeMQServerAddress = "localhost:50000";

            var queue = new KubeMQ.SDK.csharp.Queue.Queue("", "Csharp-sdk-cookbook-queues-multicast-client", KubeMQServerAddress);
            try
            {
                var res = queue.Send(new KubeMQ.SDK.csharp.Queue.Message
                {
                    Queue = "queue.a;queue.b;queue.c",
                    Body = KubeMQ.SDK.csharp.Tools.Converter.ToByteArray("hi, new message"),
                    Metadata = "some-metadata",
                    Tags = new Dictionary<string, string>()
                {
                    {"Action",$"SendQueueMessage" }
                },
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


            
            var msgA = queue.Pull("queue.a",1,1);
            if (msgA.IsError)
            {
                Console.WriteLine($"message error, error:{msgA.Error}");
            }
            foreach (var item in msgA.Messages)
            {
                Console.WriteLine($"queue A message received body:{KubeMQ.SDK.csharp.Tools.Converter.FromByteArray(item.Body)}");

            }

         
            var msgB = queue.Pull("queue.b",1,1);
            if (msgB.IsError)
            {
                Console.WriteLine($"message error, error:{msgB.Error}");
            }
            foreach (var item in msgB.Messages)
            {
                Console.WriteLine($"queue B message received body:{KubeMQ.SDK.csharp.Tools.Converter.FromByteArray(item.Body)}");

            }
            var msgC = queue.Pull("queue.c",1,1);
            if (msgC.IsError)
            {
                Console.WriteLine($"message error, error:{msgC.Error}");
            }
            foreach (var item in msgC.Messages)
            {
                Console.WriteLine($"queue C message received body:{KubeMQ.SDK.csharp.Tools.Converter.FromByteArray(item.Body)}");

            }
            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }
}
