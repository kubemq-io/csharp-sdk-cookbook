using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using KubeMQ.Grpc;

namespace single
{
    class Program
    {
        static void Main(string[] args)
        {
            string QueueName = "queue-dead-letter",
                KubeMQServerAddress = "localhost:50000";
            var queue = new KubeMQ.SDK.csharp.Queue.Queue(QueueName, "Csharp-sdk-cookbook-queues-dead-letter-client",
                KubeMQServerAddress);
            try
            {
                var res = queue.Send(new KubeMQ.SDK.csharp.Queue.Message
                {
                    Queue = "queue",
                    Body = KubeMQ.SDK.csharp.Tools.Converter.ToByteArray("hi, new message with dead-letter"),
                    Policy = new QueueMessagePolicy()
                    {
                        MaxReceiveCount = 1,
                        MaxReceiveQueue = "queue-dead-letter"
                    },
                    Metadata = "some-metadata",
                    Tags = new Dictionary<string, string>()
                    {
                        {"Action", $"SendQueueMessage"}
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
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                System.Environment.Exit(1);
            }

            var receiver = new KubeMQ.SDK.csharp.Queue.Queue("queue", "Csharp-sdk-cookbook-queues-stream-client",
                    KubeMQServerAddress);
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
                            $"message received, body:{KubeMQ.SDK.csharp.Tools.Converter.FromByteArray(resRec.Message.Body)}, rejecting");
                        try
                        {
                            var rejRes = transaction.RejectMessage(resRec.Message.Attributes.Sequence);
                            if (rejRes.IsError)
                            {
                                Console.WriteLine($"Error in reject Message, error:{rejRes.Error}");
                            }
                            else
                            {
                                Console.WriteLine($"Reject completed");
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
            
                try
                {
                    var msg = queue.Pull("queue-dead-letter", 1, 1);
                    if (msg.IsError)
                    {
                        Console.WriteLine($"message dequeue error, error:{msg.Error}");
                    }

                    {
                        Console.WriteLine($"{msg.Messages.Count()} messages received from dead-letter queue");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

 
           }
        
    }
   
}
