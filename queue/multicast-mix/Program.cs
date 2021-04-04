using System;
using System.Collections.Generic;
using System.Threading;

namespace multicastMix
{
    class Program
    {
        static void Main(string[] args)
        {
            string QueueName = "q1;events:e1;events_store:es1",
             KubeMQServerAddress = "localhost:50000";

            var es = new KubeMQ.SDK.csharp.Events.Subscriber(KubeMQServerAddress);
            try
            {
                es.SubscribeToEvents(new KubeMQ.SDK.csharp.Subscription.SubscribeRequest
                {
                    Channel = "e1",
                    SubscribeType = KubeMQ.SDK.csharp.Subscription.SubscribeType.Events,
                    EventsStoreType = KubeMQ.SDK.csharp.Subscription.EventsStoreType.Undefined,
                    EventsStoreTypeValue = 0,
                    ClientID = "Csharp-sdk-cookbook-queues-multicast-mix-client-A"

                }, (eventReceive) => {

                    Console.WriteLine($"Event Received: EventID:{eventReceive.EventID} Channel:{eventReceive.Channel} Metadata:{eventReceive.Metadata} Body:{ KubeMQ.SDK.csharp.Tools.Converter.FromByteArray(eventReceive.Body)} ");
                },
                    (errorHandler) => {
                        Console.WriteLine(errorHandler.Message);
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                System.Environment.Exit(1);
            }

            var subscriber = new KubeMQ.SDK.csharp.Events.Subscriber(KubeMQServerAddress);
            try
            {
                subscriber.SubscribeToEvents(new KubeMQ.SDK.csharp.Subscription.SubscribeRequest
                {
                    Channel = "es1",
                    SubscribeType = KubeMQ.SDK.csharp.Subscription.SubscribeType.EventsStore,
                    EventsStoreType = KubeMQ.SDK.csharp.Subscription.EventsStoreType.StartFromFirst,
                    ClientID = "Csharp-sdk-cookbook-queues-multicast-mix-client-B"

                }, (eventReceive) => {

                    Console.WriteLine($"subscriber a Event Store Received: EventID:{eventReceive.EventID} Channel:{eventReceive.Channel} Metadata:{eventReceive.Metadata} Body:{ KubeMQ.SDK.csharp.Tools.Converter.FromByteArray(eventReceive.Body)} ");
                },
                    (errorHandler) => {
                        Console.WriteLine(errorHandler.Message);
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                System.Environment.Exit(1);
            }

            var queue = new KubeMQ.SDK.csharp.Queue.Queue(QueueName, "Csharp-sdk-cookbook-queues-multicast-mix-client-C", KubeMQServerAddress);
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


            var queueA = new KubeMQ.SDK.csharp.Queue.Queue("q1", "Csharp-sdk-cookbook-queues-multicast-mix-client-A", KubeMQServerAddress);
            var msgA = queueA.ReceiveQueueMessages();
            if (msgA.IsError)
            {
                Console.WriteLine($"message error, error:{msgA.Error}");
            }
            foreach (var item in msgA.Messages)
            {
                Console.WriteLine($"queue C message received body:{KubeMQ.SDK.csharp.Tools.Converter.FromByteArray(item.Body)}");

            }

            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }
}
