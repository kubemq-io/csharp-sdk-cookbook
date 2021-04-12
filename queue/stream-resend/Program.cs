using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace resend
{
    class Program
    {
        static void Main(string[] args)
        {
            string QueueName = "resend",
             KubeMQServerAddress = "localhost:50000";

            var sender = new KubeMQ.SDK.csharp.Queue.Queue(QueueName, "Csharp-sdk-cookbook-queues-resend-client-sender", KubeMQServerAddress);
            var res = sender.Send(new KubeMQ.SDK.csharp.Queue.Message
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


            var queue = new KubeMQ.SDK.csharp.Queue.Queue(QueueName, "Csharp-sdk-cookbook-queues-resend-client-sender", KubeMQServerAddress);
            var transaction = queue.CreateTransaction();
            // get message from the queue with visibility of 5 seconds and wait timeout of 10 seconds
            KubeMQ.SDK.csharp.Queue.Stream.TransactionMessagesResponse resRec;
            try
            {
                resRec = transaction.Receive(5, 10);
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

            Console.WriteLine("Resend to new queue");
            try
            {
                var resResend = transaction.Resend("new-queue");
                if (resResend.IsError)
                {
                    Console.WriteLine($"Message Resend error, error:{resResend.Error}");
                    return;
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Message Resend error, error:{ex.Message}");
                return;
            }
            finally
            {
                transaction.Close();
            }
            try
            {
                var msg = queue.Pull("new-queue", 1, 2);
                if (msg.IsError)
                {
                    Console.WriteLine($"message dequeue error, error:{msg.Error}");
                }

                {
                    Console.WriteLine($"{msg.Messages.Count()} messages received from new-queue");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            Console.WriteLine("DONE");
        }
    }
   
}
