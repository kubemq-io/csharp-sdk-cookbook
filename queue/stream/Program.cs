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

            while (true)
            {
                Console.WriteLine("Checking Messages");
                var transaction = receiver.CreateTransaction();
                KubeMQ.SDK.csharp.Queue.Stream.TransactionMessagesResponse resRec;
                try
                {
                    resRec = transaction.Receive(1, 5);
                    if (resRec.IsError)
                    {
                        Console.WriteLine($"Message dequeue error, error:{resRec.Error}");
                        transaction.Close();
                        
                    }
                    else
                    {
                        Console.WriteLine(
                            $"MessageID: {resRec.Message.MessageID} Received");
                        try
                        {
                            var ackRes = transaction.AckMessage(resRec.Message.Attributes.Sequence);
                            if (ackRes.IsError)
                            {
                                Console.WriteLine($"Error in ack Message, error:{ackRes.Error}");
                            }
                            else
                            {
                                Console.WriteLine($"Ack completed");
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        finally
                        {
                            transaction.Close();
                        }
                    }

                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"Message Receive error, error:{ex.Message}");
                }
            }
        }
           
    }
   
}
