using System;
using System.Threading;

namespace load_balancer
{
    class Program
    {
        static void Main(string[] args)
        {
            string groupName = "group1";
            string Channel = "events",
            KubeMQServerAddress = "localhost:50000";
            var subscriber_a = new KubeMQ.SDK.csharp.Events.Subscriber(KubeMQServerAddress);
            try
            {
                subscriber_a.SubscribeToEvents(new KubeMQ.SDK.csharp.Subscription.SubscribeRequest
                {
                    Channel = Channel,
                    SubscribeType = KubeMQ.SDK.csharp.Subscription.SubscribeType.Events,
                    ClientID = "Csharp-sdk-cookbook-pubsub-events-load-balancer-receiver-a",
                    Group = groupName

                }, (eventReceive) => {

                    Console.WriteLine($"subscriber a Event Received: EventID:{eventReceive.EventID} Channel:{eventReceive.Channel} Metadata:{eventReceive.Metadata} Body:{ KubeMQ.SDK.csharp.Tools.Converter.FromByteArray(eventReceive.Body)} ");
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
                    Channel = Channel,
                    SubscribeType = KubeMQ.SDK.csharp.Subscription.SubscribeType.Events,
                    ClientID = "Csharp-sdk-cookbook-pubsub-events-load-balancer-receiver-b",
                    Group = groupName

                }, (eventReceive) => {

                    Console.WriteLine($"subscriber b Event Received: EventID:{eventReceive.EventID} Channel:{eventReceive.Channel} Metadata:{eventReceive.Metadata} Body:{ KubeMQ.SDK.csharp.Tools.Converter.FromByteArray(eventReceive.Body)} ");
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
                    Channel = Channel,
                    SubscribeType = KubeMQ.SDK.csharp.Subscription.SubscribeType.Events,
                    ClientID = "Csharp-sdk-cookbook-pubsub-events-load-balancer-receiver-c",
                    Group = groupName

                }, (eventReceive) => {

                    Console.WriteLine($"subscriber c Event Received: EventID:{eventReceive.EventID} Channel:{eventReceive.Channel} Metadata:{eventReceive.Metadata} Body:{ KubeMQ.SDK.csharp.Tools.Converter.FromByteArray(eventReceive.Body)} ");
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

            Thread.Sleep(1000);

            var channel = new KubeMQ.SDK.csharp.Events.Channel(new KubeMQ.SDK.csharp.Events.ChannelParameters
            {
                ChannelName = Channel,
                ClientID = "Csharp-sdk-cookbook-pubsub-events-load-balancer-sender",
                KubeMQAddress = KubeMQServerAddress,
            });

            try
            {
                for (int i = 0; i < 5; i++)
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
 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }
}
