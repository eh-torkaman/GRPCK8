using Grpc.Core;
using Grpc.Net.Client;
using SharedData.Models;
using SharedData.proto;
using System;

namespace test1
{
    internal class Program
    {
         static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!"); Console.ReadKey();
            //  using var channel = GrpcChannel.ForAddress("http://mainoutsiderservice:5002");
            using var channel = GrpcChannel.ForAddress("http://localhost:5002");
            var client = new StockItems.StockItemsClient(channel);
            var reply = client.GetAllStockItemAsync(new AllStockItemRequest()).GetAwaiter().GetResult();

            foreach (var it in reply.StockItems)
                Console.WriteLine("--: " + it.ToString() + "\n");

            var reply2 = client.GetStockItemsCurrentPrice(new StockItemsCurrentPriceRequest());

            while (await reply2.ResponseStream.MoveNext())
            {
                var c = reply2.ResponseStream.Current;
                foreach(var it in c.StockItemsCurrentPrice)
                {
                    Console.WriteLine(it.Id + " " + it.CurrentPrice+" "+it.UpdateTime);
                }
                Console.WriteLine("---------------------..");
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}