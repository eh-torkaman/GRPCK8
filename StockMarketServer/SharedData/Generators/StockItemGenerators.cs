using Google.Protobuf.WellKnownTypes;
using SharedData.Models;

namespace SharedData.Generators
{
    public static class StockItemGenerators
    {
        private static Random random = new Random();
        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private static List<StockItem> _items { get; set; }

        private static int ItemsCount = 200;
        public static List<StockItem> GetStockItems()
        {
            if (_items == null)
            {
                _items = new List<StockItem>();
                for (int i = 0; i < ItemsCount; i++)
                {
                    var price =100+ Math.Round(random.NextSingle() * 1000);
                    var tempItem = new StockItem()
                    {
                        Description = RandomString(25),
                        Id = i,
                        InitPrice = price,
                        FinalPrice= price,
                        Name = RandomString(5),
                        UpdateTime = DateTime.Now,
                    };
                    _items.Add(tempItem);
                }
            }

            return _items;
        }

        public static List<StockItemCurrentPrice> GetStockItemCurrentPrice()
        {


            var newRandomPrice = (StockItem st) =>
            {
                var shouldAffectCurrentPrice = () => random.NextSingle() < 0.2;
                var percentageChage = 1 + ((random.Next(11) - 5) / 100.0);
                var newPrice = st.FinalPrice * percentageChage;
                if (shouldAffectCurrentPrice())
                {
                    st.FinalPrice = newPrice;
                }
                return newPrice;
            };

            var shouldGenerate = () => random.NextSingle() <0.2;
            var lsStockItemCurrentPrice = new List<StockItemCurrentPrice>();

            foreach (var stockItem in GetStockItems())
            {
                if (shouldGenerate())
                {
                    var stockItemCurrentPrice = new StockItemCurrentPrice() {Id=stockItem.Id,
                        CurrentPrice= newRandomPrice(stockItem),
                        FinalPrice=stockItem.FinalPrice,
                        PriceChangePercentage= Math.Round( ((stockItem.FinalPrice/ stockItem.InitPrice)-1)  *100),
                        UpdateTime= DateTime.Now,

                };
                    //Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2021, 7, 1), DateTimeKind.Utc));
                    lsStockItemCurrentPrice.Add(stockItemCurrentPrice);
                }
            }

            return lsStockItemCurrentPrice;
        }

    }
}
