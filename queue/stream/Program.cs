using System;
using System.Collections.Generic;
using System.Threading;

namespace stream
{
    class Program
    {
        static void Main(string[] args)
        {
            string QueueName = "stream",
             KubeMQServerAddress = "localhost:50000";

            var queue = new KubeMQ.SDK.csharp.Queue.Queue(QueueName, "Csharp-sdk-cookbook-queues-stream-client-sender", KubeMQServerAddress);
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

            var receiver = new KubeMQ.SDK.csharp.Queue.Queue(QueueName, "Csharp-sdk-cookbook-queues-stream-client", KubeMQServerAddress);
            var transaction = receiver.CreateTransaction();
            KubeMQ.SDK.csharp.Queue.Stream.TransactionMessagesResponse resRec;
            try
            {
                resRec = transaction.Receive(1, 1);
                if (resRec.IsError)
                {
                    Console.WriteLine($"Message dequeue error, error:{resRec.Error}");
                    return;
                }
                Console.WriteLine($"MessageID: {resRec.Message.MessageID}, Body:{KubeMQ.SDK.csharp.Tools.Converter.FromByteArray(resRec.Message.Body)}");

            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Message dequeue error, error:{ex.Message}");
                return;
            }
            Console.WriteLine("Doing some work.....");
            Thread.Sleep(1000);
            Console.WriteLine("DONE");
        }
    }
   
}
