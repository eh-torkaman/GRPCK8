using Dapr;
using FrontApiStockMarket.Hubs;
using Google.Protobuf.Collections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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

        

    }
}
