using System;
using System.Collections.Generic;
using System.Threading;

namespace multicastMix
{
    class Program
    {
        static void Main(string[] args)
        {

            string ChannelName = "e1;events_store:es1;queues:q1",
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
                    ClientID = "Csharp-sdk-cookbook-events-multicast-mix-client-A"

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
                    ClientID = "Csharp-sdk-cookbook-events-store-multicast-mix-client-B"

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

            var channel = new KubeMQ.SDK.csharp.Events.Channel(new KubeMQ.SDK.csharp.Events.ChannelParameters
            {
                ChannelName = ChannelName,
                ClientID = "Csharp-sdk-cookbook-pubsub-single-multicast-mix-client-sender",
                KubeMQAddress = KubeMQServerAddress,
                Store = false
            });

            try
            {
                var result = channel.SendEvent(new KubeMQ.SDK.csharp.Events.Event()
                {
  
                    Body = KubeMQ.SDK.csharp.Tools.Converter.ToByteArray("hello kubemq - sending single event")
                });
                if (!result.Sent)
                {
                    Console.WriteLine($"Could not send single message:{result.Error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            var queueA = new KubeMQ.SDK.csharp.Queue.Queue("q1", "Csharp-sdk-cookbook-events-multicast-mix-client-A", KubeMQServerAddress);
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
