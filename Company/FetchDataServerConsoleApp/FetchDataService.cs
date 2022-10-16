using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProtoBufGeneratedClasses;
using ProtoBufGeneratedClasses.Messages;
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

        public async void FetchData()
        {

            using var channel = GrpcChannel.ForAddress(serverAdress, new GrpcChannelOptions() { });
            var _client = new StockItems.StockItemsClient(channel);
            var stockItemsReply = _client.GetAllStockItemAsync(new AllStockItemRequest()).GetAwaiter().GetResult();

            //foreach (var it in stockItemsReply.StockItems)
            //    Console.WriteLine("--: " + it.ToString() + "\n");
            var metadata = new Dictionary<string, string>();
            metadata.Add("ttlInSeconds", "25");
            try
            {
                rabbitMqManager.SendStockItems(stockItemsReply.StockItems.ToList());
                // await daprClient.PublishEventAsync("pubsub", "StockItems", stockItemsReply.StockItems.ToList(), metadata);
                logger.LogInformation("stockItemsReply.StockItems published");
            }
            catch (Exception err) { logger.LogError(exception: err, " daprClient Err"); }

            var stockItemsCurrentPriceReply = _client.GetStockItemsCurrentPrice(new StockItemsCurrentPriceRequest());

            while (await stockItemsCurrentPriceReply.ResponseStream.MoveNext())
            {
                var stockItemsCurrentPrice = stockItemsCurrentPriceReply.ResponseStream.Current.StockItemsCurrentPrice;

                try
                {
                    //del me
                    rabbitMqManager.SendStockItems(stockItemsReply.StockItems.ToList());
                    ///
                    var message = new StockItemsMessage(stockItemsCurrentPrice.ToList());
                    //   await daprClient.PublishEventAsync("pubsub", "CurrentPrice", message, metadata);
                    logger.LogInformation("stockItemsCurrentPrice published");
                }
                catch (Exception err) { logger.LogError(exception: err, " daprClient Err"); }

                //foreach (var it in stockItemsCurrentPrice)
                //    Console.WriteLine(it.Id + " " + it.CurrentPrice);
            }
        }
    }


}