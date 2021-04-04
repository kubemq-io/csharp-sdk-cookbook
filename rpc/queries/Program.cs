using System;

namespace queries
{
    class Program
    {
        static void Main(string[] args)
        {
            string ChannelName = "queries",
             KubeMQServerAddress = "localhost:50000";

            KubeMQ.SDK.csharp.CommandQuery.Responder responder = new KubeMQ.SDK.csharp.CommandQuery.Responder(KubeMQServerAddress);
            try
            {
                responder.SubscribeToRequests(new KubeMQ.SDK.csharp.Subscription.SubscribeRequest()
                {
                    Channel = ChannelName,
                    SubscribeType = KubeMQ.SDK.csharp.Subscription.SubscribeType.Queries,
                    ClientID = "Csharp-sdk-cookbook-rpc-queries-client-sender"
                }, (queryReceive) => {
                    Console.WriteLine($"Query Received: Id:{queryReceive.RequestID} Channel:{queryReceive.Channel} Metadata:{queryReceive.Metadata} Body:{ KubeMQ.SDK.csharp.Tools.Converter.FromByteArray(queryReceive.Body)} ");
                    return new KubeMQ.SDK.csharp.CommandQuery.Response(queryReceive)
                    {
                        Body = KubeMQ.SDK.csharp.Tools.Converter.ToByteArray("got your query, you are good to go"),
                        CacheHit = false,
                        Error = "None",
                        ClientID = "Csharp-sdk-cookbook-rpc-queries-client-sender",
                        Executed = true,
                        Metadata = "this is a response",
                        Timestamp = DateTime.UtcNow
                    };

                }, (errorHandler) => {
                    Console.WriteLine(errorHandler.Message);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            var channel = new KubeMQ.SDK.csharp.CommandQuery.Channel(new KubeMQ.SDK.csharp.CommandQuery.ChannelParameters
            {
                RequestsType = KubeMQ.SDK.csharp.CommandQuery.RequestType.Query,
                Timeout = 1000,
                ChannelName = ChannelName,
                ClientID = "Csharp-sdk-cookbook-rpc-queries-client-receiver",
                KubeMQAddress = KubeMQServerAddress
            });
            try
            {
                var result = channel.SendRequest(new KubeMQ.SDK.csharp.CommandQuery.Request
                {
                    Body = KubeMQ.SDK.csharp.Tools.Converter.ToByteArray("hello kubemq - sending a query, please reply")
                });

                if (!result.Executed)
                {
                    Console.WriteLine($"Response error:{result.Error}");
                    return;
                }
                Console.WriteLine($"Response Received:{result.RequestID} ExecutedAt:{result.Timestamp}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

      
    }

}
