using Dapr;
using FrontApiStockMarket.Hubs;
using Google.Protobuf.Collections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ProtoBufGeneratedClasses.Messages;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FrontApiStockMarket.Controllers
{
    [Route("api/Subscriber")]
    [ApiController]
    public class SubscriberController : ControllerBase
    {
        private readonly ILogger<SubscriberController> logger;
        private readonly RabbitMqManager rabbitMqManager;

        private  IHubContext<StockItemsHub, IStockItemsHub> hubContext { get; }

        
        public SubscriberController(ILogger<SubscriberController> logger,
            IHubContext<StockItemsHub, IStockItemsHub> hubContext,
            RabbitMqManager rabbitMqManager)
        {
            this.logger = logger;
            this.hubContext = hubContext;
            this.rabbitMqManager = rabbitMqManager;
        }
        [HttpGet]
        public async Task sdfsd()
        {
            var a = this.hubContext.Clients;
            await this.hubContext.Clients.All.SendMessage("a", "b");
        }


        //List<global::SharedData.proto.StockItemCurrentPrice>
        [Topic("pubsub", "CurrentPrice")]

        public async Task<IActionResult> ReadPubSubCurrentPrice(/*[FromBody] */StockItemsMessage data)
        {
            logger.LogInformation(data.stockItemCurrentPrice.First().Id.ToString());
            string jsonString = "";
            try { jsonString = JsonSerializer.Serialize(data.stockItemCurrentPrice); }
            catch (Exception ee) { jsonString = ee.Message; }
            logger.LogInformation(" CurrentPrice => recieved \n\n " + jsonString);
            await this.hubContext.Clients.All.SendStockItems(data.stockItemCurrentPrice);
            await Task.Delay(2000);
            return Ok();
        }

        

        public record Order([property: JsonPropertyName("data")] string data);
    }
}
