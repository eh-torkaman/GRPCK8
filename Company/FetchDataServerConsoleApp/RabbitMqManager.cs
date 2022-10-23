using RabbitMQ.Client;
using SharedData.proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FetchDataServerConsoleApp
{
    public class RabbitMqManager : IDisposable
    {
        IConnection connection;
        IModel channel;
        string exchaneName = "Stocks";
        string streamStocks = "stockItems-stream";
        string routingKeyStockItems = "StockItems";
        public RabbitMqManager()
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
            connection = factory.CreateConnection();

            channel = connection.CreateModel();
            channel.ExchangeDeclare(exchaneName, ExchangeType.Topic, true);
            var streamParams = new Dictionary<string, object>();
            streamParams["x-queue-type"] = "stream";
            streamParams["x-max-length-bytes"] = 10_000_000;// maximum stream size: 10 Mb
            streamParams["x-stream-max-segment-size-bytes"] = 1_000_000;

           var stockItems_stream= channel.QueueDeclare(streamStocks,
                                  true,         // durable
                                  false, false, // not exclusive, not auto-delete
                                  streamParams
                                );

            channel.QueueBind(streamStocks, exchaneName, routingKeyStockItems);


        }

        public void Dispose()
        {
            channel.Close();
            connection.Close();
        }

        public void SendMessage(string msg = "")
        {
            ReadOnlyMemory<byte> byteBody = System.Text.Encoding.UTF8.GetBytes(msg);
            channel.BasicPublish(exchaneName, "", body: byteBody);
        }

        public void SendStockItems(List<StockItem> stockItem)
        {
            var rs = System.Text.Json.JsonSerializer.Serialize(stockItem);
            var byteBody = System.Text.Encoding.UTF8.GetBytes(rs);
            channel.BasicPublish(exchaneName, "StockItems", body: byteBody);
        }
        public void SendStockItemCurrentPrice(List<StockItemCurrentPrice> stockItemCurrentPrices)
        {
            var rs = System.Text.Json.JsonSerializer.Serialize(stockItemCurrentPrices);
            var byteBody = System.Text.Encoding.UTF8.GetBytes(rs);
            channel.BasicPublish(exchaneName, "stockItemCurrentPrices", body: byteBody);
        }
    }
}
