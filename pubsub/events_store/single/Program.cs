using System;
using System.Threading;

namespace ack
{
    class Program
    {
        static void Main(string[] args)
        {
            string ChannelName = "events-store",
             KubeMQServerAddress = "localhost:50000";


            var channel = new KubeMQ.SDK.csharp.Events.Channel(new KubeMQ.SDK.csharp.Events.ChannelParameters
            {
                ChannelName = ChannelName,
                ClientID = "Csharp-sdk-cookbook-pubsub-events-store-single-sender",
                KubeMQAddress = KubeMQServerAddress,
                Store = true
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

            Thread.Sleep(1000);

            var subscriber = new KubeMQ.SDK.csharp.Events.Subscriber(KubeMQServerAddress);
            try
            {
                subscriber.SubscribeToEvents(new KubeMQ.SDK.csharp.Subscription.SubscribeRequest
                {
                    Channel = ChannelName,
                    SubscribeType = KubeMQ.SDK.csharp.Subscription.SubscribeType.EventsStore,
                    EventsStoreType = KubeMQ.SDK.csharp.Subscription.EventsStoreType.StartFromFirst,
                    EventsStoreTypeValue = 0,
                    ClientID = "Csharp-sdk-cookbook-pubsub-events-store-single-receiver"

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

            Console.WriteLine("DONE");
            Console.ReadLine();

        }
    }
   
}
