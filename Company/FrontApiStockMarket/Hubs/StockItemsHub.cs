using Google.Protobuf.Collections;
using Microsoft.AspNetCore.SignalR;
using SharedData.proto;

namespace FrontApiStockMarket.Hubs
{
    
    public interface IStockItemsHub
    {
        public Task SendMessage(string user, string message);
        public Task SendStockItems(List<StockItemCurrentPrice> StockItems);
    }

    public class StockItemsHub : Hub< IStockItemsHub>
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendMessage(user, message);
        }

        public async Task SendStockItems(List<StockItemCurrentPrice> SendStockItems)
        {
            await Clients.All.SendStockItems(SendStockItems);
        }

    }

}
