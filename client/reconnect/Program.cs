using System;

namespace reconnect
{
    class Program
    {
        static void Main(string[] args)
        {

                string ChannelName = "reconnect",
                 ClientID = "Csharp-sdk-cookbook-client-reconnect",
                 KubeMQServerAddress = "localhost:50001";

                var subscriber = new KubeMQ.SDK.csharp.Events.Subscriber(KubeMQServerAddress);
                try
                {
                    subscriber.SubscribeToEvents(new KubeMQ.SDK.csharp.Subscription.SubscribeRequest
                    {
                        Channel = ChannelName,
                        SubscribeType = KubeMQ.SDK.csharp.Subscription.SubscribeType.Events,
                        ClientID = ClientID

                    }, (eventReceive) => {

                        Console.WriteLine($"Event Received: EventID:{eventReceive.EventID} Channel:{eventReceive.Channel} Metadata:{eventReceive.Metadata} Body:{ KubeMQ.SDK.csharp.Tools.Converter.FromByteArray(eventReceive.Body)} ");
                    },
                        (errorHandler) => {
                            Console.WriteLine(errorHandler.Message);
                        });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("first connect failed , retry");
                    subscriber = new KubeMQ.SDK.csharp.Events.Subscriber("localhost:50000");
                    Console.WriteLine(ex.Message);
                    subscriber.SubscribeToEvents(new KubeMQ.SDK.csharp.Subscription.SubscribeRequest
                    {
                        Channel = ChannelName,
                        SubscribeType = KubeMQ.SDK.csharp.Subscription.SubscribeType.Events,
                        ClientID = ClientID

                    }, (eventReceive) => {

                        Console.WriteLine($"Event Received: EventID:{eventReceive.EventID} Channel:{eventReceive.Channel} Metadata:{eventReceive.Metadata} Body:{ KubeMQ.SDK.csharp.Tools.Converter.FromByteArray(eventReceive.Body)} ");
                    },
                        (errorHandler) => {
                            Console.WriteLine(errorHandler.Message);
                        });
                }
                Console.WriteLine("DONE");
                Console.ReadLine();
            
        }
    }
}
