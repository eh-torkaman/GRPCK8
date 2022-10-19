using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProtoBufGeneratedClasses;
using SharedData.proto;
using System.Collections;
using System.Text.Json.Serialization;

namespace FetchDataServerConsoleApp
{
    // Worker.cs
    internal class FetchDataService
    {
        private readonly string serverAdress = "";
        private readonly IConfiguration configuration;
        private readonly ILogger logger;
        private readonly RabbitMqManager rabbitMqManager;

        public FetchDataService(IConfiguration configuration, ILogger<FetchDataService> logger, RabbitMqManager rabbitMqManager)
        {
            serverAdress = configuration.GetValue<string>("StockServerAddress");
            this.configuration = configuration;
            this.logger = logger;
            this.rabbitMqManager = rabbitMqManager;
            //Console.ForegroundColor = ConsoleColor.Magenta;
            //foreach (DictionaryEntry e in System.Environment.GetEnvironmentVariables())
            //    try { Console.WriteLine(e.Key + ":" + e.Value); }
            //    catch { }
            //Console.ForegroundColor = ConsoleColor.Cyan;
            //Console.WriteLine(" -------------- \n -------------- \n -------------- \n");
            //Console.ForegroundColor = ConsoleColor.White;
        }

        public async void FetchStockItemsCurrentPrice()
        {
            this.logger.LogInformation("FetchStockItemsCurrentPrice");
            using var channel = GrpcChannel.ForAddress(serverAdress, new GrpcChannelOptions() { });
            var _client = new StockItems.StockItemsClient(channel);
            var stockItemsCurrentPriceReply = _client.GetStockItemsCurrentPrice(new StockItemsCurrentPriceRequest());
            while (await stockItemsCurrentPriceReply.ResponseStream.MoveNext())
            {
                var stockItemsCurrentPrice = stockItemsCurrentPriceReply.ResponseStream.Current.StockItemsCurrentPrice;
                try
                {
                    rabbitMqManager.SendStockItemCurrentPrice(stockItemsCurrentPrice.ToList());
                    logger.LogInformation("stockItemsCurrentPrice published");
                }
                catch (Exception err) { logger.LogError(exception: err, "  Err"); }
            }
        }

        public async void FetchStockItems()
        {
            this.logger.LogInformation("FetchStockItems");
            using var channel = GrpcChannel.ForAddress(serverAdress, new GrpcChannelOptions() { });
            var _client = new StockItems.StockItemsClient(channel);
            while (true)
            {
                var stockItemsReply = _client.GetAllStockItemAsync(new AllStockItemRequest()).GetAwaiter().GetResult();
                try
                {
                    rabbitMqManager.SendStockItems(stockItemsReply.StockItems.ToList());
                    logger.LogInformation("stockItemsReply.StockItems published");
                }
                catch (Exception err) { logger.LogError(exception: err, " Err"); }
                await Task.Delay(35*1000);
            }
        }
    }


}