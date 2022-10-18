using FrontApiStockMarket.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedData.proto;
using System.Text;

namespace FrontApiStockMarket
{
    public class RabbitMqManager : IDisposable
    {
        private IConnection connection;
        private IModel channel;
        private string exchaneName = "Stocks";

        private string QueueStockItems = "QueueStockItems";
        private string QueueStockItemCurrentPrices = "stockItemCurrentPrices";

        string routingKeyStockItems = "StockItems";
        string routingKeystockItemCurrentPrices = "stockItemCurrentPrices";

        private ILogger<RabbitMqManager> logger;
        private readonly IHubContext<StockItemsHub, IStockItemsHub> hubContext;

        private void connectToRabit()
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(QueueStockItems, false, exclusive: true, autoDelete: true);
            channel.QueueDeclare(QueueStockItemCurrentPrices, false, exclusive: true, autoDelete: true);

            channel.QueueBind(QueueStockItems, exchaneName, routingKeyStockItems);
            channel.QueueBind(QueueStockItemCurrentPrices, exchaneName, routingKeystockItemCurrentPrices);
            EventingBasicConsumer consumer;
            consumer = new EventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;
            channel.BasicConsume(QueueStockItems, true, consumer);
            channel.BasicConsume(QueueStockItemCurrentPrices, true, consumer);
            logger.LogWarning("RabbitMqManager started.........");

        }
        public RabbitMqManager(ILogger<RabbitMqManager> logger, IHubContext<StockItemsHub, IStockItemsHub> hubContext)
        {
            this.hubContext = hubContext;
            this.logger = logger;

            connectToRabit();
        }
        private void readStockItemCurrentPrices(BasicDeliverEventArgs eventArg)
        {
            List<StockItemCurrentPrice> stockItemCurrentPrice = new List<StockItemCurrentPrice>();
            var jsonText = System.Text.Encoding.UTF8.GetString(eventArg.Body.ToArray());
            if (jsonText != null)
                stockItemCurrentPrice = System.Text.Json.JsonSerializer.Deserialize<List<StockItemCurrentPrice>>(jsonText) ?? new List<StockItemCurrentPrice>();
            if (stockItemCurrentPrice != null)
            {
                logger.LogWarning("StockItemCurrentPrice Count: " + stockItemCurrentPrice.Count().ToString());
                this.hubContext.Clients.All.SendStockItemCurrentPrice(stockItemCurrentPrice);
            }
        }
        public List<StockItem> stockItems ;
        private void readStockItem(BasicDeliverEventArgs eventArg)
        {
            List<StockItem> stockItems = new List<StockItem>();
            var jsonText = System.Text.Encoding.UTF8.GetString(eventArg.Body.ToArray());
            if (jsonText != null)
                stockItems = System.Text.Json.JsonSerializer.Deserialize<List<StockItem>>(jsonText) ?? new List<StockItem>();
            if (stockItems != null)
            {
                this.stockItems = stockItems;
                logger.LogWarning("routingKeyStockItems Count: " + stockItems.Count().ToString());
               // this.hubContext.Clients.All.SendStockItems(stockItems);
            }
        }
        private void Consumer_Received(object? sender, BasicDeliverEventArgs eventArg)
        {
            //logger.LogError(eventArg.RoutingKey);
            if (eventArg.RoutingKey == routingKeystockItemCurrentPrices)
            {
                readStockItemCurrentPrices(eventArg);
            }
            if (eventArg.RoutingKey == routingKeyStockItems)
            {
                readStockItem(eventArg);
            }

        }

        public void Dispose()
        {
            channel.Close();
            connection.Close();
        }


    }
}
