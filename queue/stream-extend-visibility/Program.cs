using System;
using System.Collections.Generic;
using System.Threading;

namespace extend
{
    class Program
    {
        static void Main(string[] args)
        {
            string QueueName = "extend",
             KubeMQServerAddress = "localhost:50000";

            var sender = new KubeMQ.SDK.csharp.Queue.Queue(QueueName, "Csharp-sdk-cookbook-queues-extend-client-sender", KubeMQServerAddress);
            var res = sender.SendQueueMessage(new KubeMQ.SDK.csharp.Queue.Message
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


            var queue = new KubeMQ.SDK.csharp.Queue.Queue(QueueName, "Csharp-sdk-cookbook-queues-extend-client-sender", KubeMQServerAddress);
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

            Console.WriteLine("Work for 1 seconds");
            Thread.Sleep(1000);
            Console.WriteLine("Need more time to process, extend visibility for more 3 seconds");
            try
            {
                var resExt = transaction.ExtendVisibility(3);
                if (resExt.IsError)
                {
                    Console.WriteLine($"Message ExtendVisibility error, error:{resExt.Error}");
                    return;
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Message ExtendVisibility error, error:{ex.Message}");
            }

            Console.WriteLine("Approved. work for 2.5 seconds");
            Thread.Sleep(2500);
            Console.WriteLine("Work done... ackall the message");
            try
            {
                var resAck = transaction.AckMessage(resRec.Message.Attributes.Sequence);
                if (resAck.IsError)
                {
                    Console.WriteLine($"Ack message error:{resAck.Error}");
                }
                Console.WriteLine("Ack done");
            }
            catch (System.Exception ex)
            {

                Console.WriteLine($"Ack message error:{ex.Message}");
            }

            Console.WriteLine("DONE");
        }
    }

}
