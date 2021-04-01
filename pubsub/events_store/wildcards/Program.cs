using System;
using System.Threading;

namespace multicast
{
    class Program
    {
        static void Main(string[] args)
        {
            string channel = "event",
             KubeMQServerAddress = "localhost:50000";
            var subscriber_a = new KubeMQ.SDK.csharp.Events.Subscriber(KubeMQServerAddress);
            try
            {
                subscriber_a.SubscribeToEvents(new KubeMQ.SDK.csharp.Subscription.SubscribeRequest
                {
                    Channel = channel+".A",
                    SubscribeType = KubeMQ.SDK.csharp.Subscription.SubscribeType.Events,
                    ClientID = "Csharp-sdk-cookbook-pubsub-events-wildcards-receiver-a"

                }, (eventReceive) => {

                    Console.WriteLine($"subscriber A Event Received: EventID:{eventReceive.EventID} Channel:{eventReceive.Channel} Metadata:{eventReceive.Metadata} Body:{ KubeMQ.SDK.csharp.Tools.Converter.FromByteArray(eventReceive.Body)} ");
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
                    Channel = channel + ".B",
                    SubscribeType = KubeMQ.SDK.csharp.Subscription.SubscribeType.Events,
                    ClientID = "Csharp-sdk-cookbook-pubsub-events-wildcards-receiver-b"

                }, (eventReceive) => {

                    Console.WriteLine($"subscriber B Event Received: EventID:{eventReceive.EventID} Channel:{eventReceive.Channel} Metadata:{eventReceive.Metadata} Body:{ KubeMQ.SDK.csharp.Tools.Converter.FromByteArray(eventReceive.Body)} ");
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
                    Channel = channel + ".*",
                    SubscribeType = KubeMQ.SDK.csharp.Subscription.SubscribeType.Events,
                    ClientID = "Csharp-sdk-cookbook-pubsub-events-wildcards-receiver"

                }, (eventReceive) => {

                    Console.WriteLine($"subscriber C Event Received: EventID:{eventReceive.EventID} Channel:{eventReceive.Channel} Metadata:{eventReceive.Metadata} Body:{ KubeMQ.SDK.csharp.Tools.Converter.FromByteArray(eventReceive.Body)} ");
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

            var sender = new KubeMQ.SDK.csharp.Events.Channel(new KubeMQ.SDK.csharp.Events.ChannelParameters
            {
                ChannelName = channel+".A",
                ClientID = "Csharp-sdk-cookbook-pubsub-events-multicast-sender",
                KubeMQAddress = KubeMQServerAddress,
            });

            try
            {
                var result = sender.SendEvent(new KubeMQ.SDK.csharp.Events.Event()
                {
                    Body = KubeMQ.SDK.csharp.Tools.Converter.ToByteArray("hello kubemq - sending wildcards A")
                });
                if (!result.Sent)
                {
                    Console.WriteLine($"Could not send wildcards message:{result.Error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            sender = new KubeMQ.SDK.csharp.Events.Channel(new KubeMQ.SDK.csharp.Events.ChannelParameters
            {
                ChannelName = channel + ".B",
                ClientID = "Csharp-sdk-cookbook-pubsub-events-multicast-sender",
                KubeMQAddress = KubeMQServerAddress,
            });

            try
            {
                var result = sender.SendEvent(new KubeMQ.SDK.csharp.Events.Event()
                {
                    Body = KubeMQ.SDK.csharp.Tools.Converter.ToByteArray("hello kubemq - sending wildcards B")
                });
                if (!result.Sent)
                {
                    Console.WriteLine($"Could not send wildcards message:{result.Error}");
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
