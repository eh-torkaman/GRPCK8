using Grpc.Core;
using MainOutsiderService;
using Microsoft.AspNetCore.Mvc;
using SharedData.proto;
using SharedData.Generators;
using Google.Protobuf.WellKnownTypes;

namespace MainOutsiderService.Services
{
    public class StockItemsService : StockItems.StockItemsBase
    {
        private Int64 rnd = new Random().NextInt64();
        private readonly ILogger<StockItemsService> _logger;
        public StockItemsService(ILogger<StockItemsService> logger)
        {
            _logger = logger;
            _logger.LogInformation(" StockItemsService" + rnd);
        }

        public override Task<AllStockItemReply> GetAllStockItem(AllStockItemRequest request, ServerCallContext context)
        {
            _logger.LogInformation("entering GetAllStockItem");
            var rs = new AllStockItemReply();
            foreach (var item in StockItemGenerators.GetStockItems())
                rs.StockItems.Add(new StockItem()
                {
                    Id = item.Id,
                    Name = item.Name,
                    FinalPrice = item.FinalPrice,
                    InitPrice = item.InitPrice,
                    Description = item.Description,
                    UpdateTime = Timestamp.FromDateTime(DateTime.SpecifyKind(item.UpdateTime, DateTimeKind.Utc))
                });
            return Task.FromResult(rs);
        }


        public override async Task GetStockItemsCurrentPrice(StockItemsCurrentPriceRequest request,
            IServerStreamWriter<StockItemsCurrentPriceReply> responseStream, ServerCallContext context)
        {
            _logger.LogInformation("entering GetStockItemsCurrentPrice");

            var writeAStockItemCurrentPriceToSream = async () =>

                 await   Task.Run(async () =>
                  {
                      if (!context.CancellationToken.IsCancellationRequested)
                      {
                          var rs = new StockItemsCurrentPriceReply();
                          foreach (var item in StockItemGenerators.GetStockItemCurrentPrice())
                              rs.StockItemsCurrentPrice.Add(new StockItemCurrentPrice()
                              {
                                  Id = item.Id,
                                  CurrentPrice = item.CurrentPrice,
                                  PriceChangePercentage = item.PriceChangePercentage,
                                  FinalPrice = item.FinalPrice,
                                  UpdateTime = Timestamp.FromDateTime(DateTime.SpecifyKind(item.UpdateTime, DateTimeKind.Utc))
                              });
                          if (rs.StockItemsCurrentPrice.Count > 0)
                              await responseStream.WriteAsync(rs);
                      }

                  });


            while (!context.CancellationToken.IsCancellationRequested)
            {
                await writeAStockItemCurrentPriceToSream();
                await Task.Delay(500, context.CancellationToken);
            }

        }


    }
}