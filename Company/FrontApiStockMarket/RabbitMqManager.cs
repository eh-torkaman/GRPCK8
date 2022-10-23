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

        private string QueueStockItemCurrentPrices = "stockItemCurrentPrices";

        string routingKeyStockItems = "StockItems";
        string routingKeystockItemCurrentPrices = "stockItemCurrentPrices";

        string streamStocks = "stockItems-stream";

        private ILogger<RabbitMqManager> logger;
        private readonly IHubContext<StockItemsHub, IStockItemsHub> hubContext;

        private void connectToRabit()
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            channel.QueueDeclare(QueueStockItemCurrentPrices, false, exclusive: true, autoDelete: true);
            channel.QueueBind(QueueStockItemCurrentPrices, exchaneName, routingKeystockItemCurrentPrices);

            channel.QueueBind(streamStocks, exchaneName, routingKeyStockItems);

            EventingBasicConsumer consumer;
            consumer = new EventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;
            //channel.BasicConsume(QueueStockItems, true, consumer);
            channel.BasicConsume(QueueStockItemCurrentPrices, true, consumer);
            logger.LogWarning("RabbitMqManager started.........");

            var streamReciveParams = new Dictionary<string, object>();
            streamReciveParams["x-stream-offset"] = "last";
            EventingBasicConsumer consumerStreamStocks;
            consumerStreamStocks = new EventingBasicConsumer(channel);
            consumerStreamStocks.Received += ConsumerStreamStocks_Received;
            channel.BasicConsume(streamStocks, false, "consumerTag", streamReciveParams, consumerStreamStocks);
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
        public List<StockItem> stockItems;
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


        }

        private void ConsumerStreamStocks_Received(object? sender, BasicDeliverEventArgs eventArg)
        {
            logger.LogError(eventArg.RoutingKey);
            readStockItem(eventArg);

            // Note: it is possible to access the channel via
            //       ((EventingBasicConsumer)sender).Model here
            channel.BasicAck(deliveryTag: eventArg.DeliveryTag, multiple: false);

        }



        public void Dispose()
        {
            channel.Close();
            connection.Close();
        }


    }
}
