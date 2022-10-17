using Google.Protobuf.Collections;
using Microsoft.AspNetCore.SignalR;
using SharedData.proto;

namespace FrontApiStockMarket.Hubs
{
    
    public interface IStockItemsHub
    {
        public Task SendMessage(string user, string message);
        public Task SendStockItemCurrentPrice(List<StockItemCurrentPrice> StockItems);
    }

    public class StockItemsHub : Hub< IStockItemsHub>
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendMessage(user, message);
        }

        public async Task SendStockItemCurrentPrice(List<StockItemCurrentPrice> SendStockItems)
        {
            await Clients.All.SendStockItemCurrentPrice(SendStockItems);
        }

    }

}
