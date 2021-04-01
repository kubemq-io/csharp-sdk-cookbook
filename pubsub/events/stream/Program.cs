﻿using System;
using System.Threading;

namespace stream
{
    class Program
    {
        static void Main(string[] args)
        {
            string ChannelName = "events",
             KubeMQServerAddress = "localhost:50000";

            var subscriber = new KubeMQ.SDK.csharp.Events.Subscriber(KubeMQServerAddress);
            try
            {
                subscriber.SubscribeToEvents(new KubeMQ.SDK.csharp.Subscription.SubscribeRequest
                {
                    Channel = ChannelName,
                    SubscribeType = KubeMQ.SDK.csharp.Subscription.SubscribeType.Events,
                    ClientID = "Csharp-sdk-cookbook-pubsub-events-stream-receiver"

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

            Thread.Sleep(1000);

            var channel = new KubeMQ.SDK.csharp.Events.Channel(new KubeMQ.SDK.csharp.Events.ChannelParameters
            {
                ChannelName = ChannelName,
                ClientID = "Csharp-sdk-cookbook-pubsub-events-stream-sender",
                KubeMQAddress = KubeMQServerAddress

            });

            try
            {
                _ = channel.StreamEvent(new KubeMQ.SDK.csharp.Events.Event
                {
                    Body = KubeMQ.SDK.csharp.Tools.Converter.ToByteArray("hello kubemq - sending stream event 1")
                });
                _ = channel.StreamEvent(new KubeMQ.SDK.csharp.Events.Event
                {
                    Body = KubeMQ.SDK.csharp.Tools.Converter.ToByteArray("hello kubemq - sending stream event 2")
                });

                Console.WriteLine("Finished stream");
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
