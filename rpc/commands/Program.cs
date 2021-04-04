using System;

namespace commands
{
    class Program
    {
        static void Main(string[] args)
        {
            string ChannelName = "commands",
             KubeMQServerAddress = "localhost:50000";

            KubeMQ.SDK.csharp.CommandQuery.Responder responder = new KubeMQ.SDK.csharp.CommandQuery.Responder(KubeMQServerAddress);
            try
            {
                responder.SubscribeToRequests(new KubeMQ.SDK.csharp.Subscription.SubscribeRequest()
                {
                    Channel = ChannelName,
                    SubscribeType = KubeMQ.SDK.csharp.Subscription.SubscribeType.Commands,
                    ClientID = "Csharp-sdk-cookbook-rpc-commands-client-sender"
                }, (commandReceive) =>
                {
                    Console.WriteLine($"Command Received: Id:{commandReceive.RequestID} Channel:{commandReceive.Channel} Metadata:{commandReceive.Metadata} Body:{ KubeMQ.SDK.csharp.Tools.Converter.FromByteArray(commandReceive.Body)} ");
                    return new KubeMQ.SDK.csharp.CommandQuery.Response(commandReceive)
                    {
                        Body = new byte[0],
                        CacheHit = false,
                        Error = "None",
                        ClientID = "Csharp-sdk-cookbook-rpc-commands-client-sender",
                        Executed = true,
                        Metadata = string.Empty,
                        Timestamp = DateTime.UtcNow,
                    };

                }, (errorHandler) =>
                {
                    Console.WriteLine(errorHandler.Message);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            var channel = new KubeMQ.SDK.csharp.CommandQuery.Channel(new KubeMQ.SDK.csharp.CommandQuery.ChannelParameters
            {
                RequestsType = KubeMQ.SDK.csharp.CommandQuery.RequestType.Command,
                Timeout = 1000,
                ChannelName = ChannelName,
                ClientID = "Csharp-sdk-cookbook-rpc-commands-client-receiver",
                KubeMQAddress = KubeMQServerAddress
            });
            try
            {
                var result = channel.SendRequest(new KubeMQ.SDK.csharp.CommandQuery.Request
                {
                    Body = KubeMQ.SDK.csharp.Tools.Converter.ToByteArray("hello kubemq - sending a command, please reply")
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
