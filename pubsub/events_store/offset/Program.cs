using System;
using System.Threading;

namespace offset
{
    class Program
    {
        static void Main(string[] args)
        {
            string ChannelName = "events-store-offset-types",
             KubeMQServerAddress = "localhost:50000";


            var channel = new KubeMQ.SDK.csharp.Events.Channel(new KubeMQ.SDK.csharp.Events.ChannelParameters
            {
                ChannelName = ChannelName,
                ClientID = "Csharp-sdk-cookbook-pubsub-events-store-offset-sender",
                KubeMQAddress = KubeMQServerAddress,
                Store = true

            });

            try
            {
                for (int i = 0; i < 10; i++)
                {
                    _ = channel.SendEvent(new KubeMQ.SDK.csharp.Events.Event
                    {
                        Body = KubeMQ.SDK.csharp.Tools.Converter.ToByteArray($"hello kubemq - sending event {i}")
                    });

                }
                Console.WriteLine("Finished stream");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Thread.Sleep(2000);

            var subscriber_a = new KubeMQ.SDK.csharp.Events.Subscriber(KubeMQServerAddress);
            try
            {
                subscriber_a.SubscribeToEvents(new KubeMQ.SDK.csharp.Subscription.SubscribeRequest
                {
                    Channel = ChannelName,
                    SubscribeType = KubeMQ.SDK.csharp.Subscription.SubscribeType.EventsStore,
                    EventsStoreType = KubeMQ.SDK.csharp.Subscription.EventsStoreType.StartFromFirst,
                    EventsStoreTypeValue = 0,
                    ClientID = "Csharp-sdk-cookbook-pubsub-events-store-offset-start-from-first-receiver-a"

                }, (eventReceive) => {

                    Console.WriteLine($"Receiver A Event Received: Sequnce:{eventReceive.EventID} Channel:{eventReceive.Channel} Metadata:{eventReceive.Metadata} Body:{ KubeMQ.SDK.csharp.Tools.Converter.FromByteArray(eventReceive.Body)} ");
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
            var subscriber_b = new KubeMQ.SDK.csharp.Events.Subscriber(KubeMQServerAddress);
            try
            {
                subscriber_b.SubscribeToEvents(new KubeMQ.SDK.csharp.Subscription.SubscribeRequest
                {
                    Channel = ChannelName,
                    SubscribeType = KubeMQ.SDK.csharp.Subscription.SubscribeType.EventsStore,
                    EventsStoreType = KubeMQ.SDK.csharp.Subscription.EventsStoreType.StartAtSequence,
                    EventsStoreTypeValue = 5,
                    ClientID = "Csharp-sdk-cookbook-pubsub-events-store-offset-start-at-sequence-receiver-b"

                }, (eventReceive) => {

                    Console.WriteLine($"Receiver B Event Received: Sequnce:{eventReceive.Sequence} Channel:{eventReceive.Channel} Metadata:{eventReceive.Metadata} Body:{ KubeMQ.SDK.csharp.Tools.Converter.FromByteArray(eventReceive.Body)} ");
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

            var subscriber_c = new KubeMQ.SDK.csharp.Events.Subscriber(KubeMQServerAddress);
            try
            {
                subscriber_c.SubscribeToEvents(new KubeMQ.SDK.csharp.Subscription.SubscribeRequest
                {
                    Channel = ChannelName,
                    SubscribeType = KubeMQ.SDK.csharp.Subscription.SubscribeType.EventsStore,
                    EventsStoreType = KubeMQ.SDK.csharp.Subscription.EventsStoreType.StartAtTimeDelta,
                    EventsStoreTypeValue = 5,
                    ClientID = "Csharp-sdk-cookbook-pubsub-events-store-offset-start-at-time-delta-receiver-c"

                }, (eventReceive) => {

                    Console.WriteLine($"Receiver C Event Received: Sequnce:{eventReceive.EventID} Channel:{eventReceive.Channel} Metadata:{eventReceive.Metadata} Body:{ KubeMQ.SDK.csharp.Tools.Converter.FromByteArray(eventReceive.Body)} ");
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

            Console.WriteLine("DONE");
            Console.ReadLine();

        }
    }
   
}
