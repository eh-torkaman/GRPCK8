using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace FrontApiStockMarket
{
    public class RabbitMqManager : IDisposable
    {
        private IConnection connection;
        private IModel channel;
        private string exchaneName = "exchaneName";

        private string queueName = "r443344444";

        private ILogger<RabbitMqManager> logger;
        EventingBasicConsumer consumer;
        public RabbitMqManager(ILogger<RabbitMqManager> logger)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queueName,false,exclusive: false,autoDelete:true);
            channel.QueueBind(queueName, exchaneName, "");

            consumer =new EventingBasicConsumer(channel);
            consumer.Received += Consumer_Received; ;
            this.logger = logger;
            logger.LogWarning("RabbitMqManager started.........");
            channel.BasicConsume(queueName,true, consumer);
        }

        private  void Consumer_Received(object? sender, BasicDeliverEventArgs eventArg)
        {
            var msg = eventArg.Body.ToString();
            logger.LogWarning(msg);
        }

        public void Dispose()
        {
            channel.Close();
            connection.Close();
        }

        
    }
}
