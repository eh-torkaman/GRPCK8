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
        string exchaneName = "exchaneName";
        public RabbitMqManager()
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.ExchangeDeclare(exchaneName, ExchangeType.Fanout, true);
            this.SendMessage("i'm Up");
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

        public void SendStockItems(List<StockItem> stockItem )
        {
            var rs = System.Text.Json.JsonSerializer.Serialize(stockItem);
            var byteBody = System.Text.Encoding.UTF8.GetBytes(rs);
            channel.BasicPublish(exchaneName, "", body: byteBody);
        }

    }
}
