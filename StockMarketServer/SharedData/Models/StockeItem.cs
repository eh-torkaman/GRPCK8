using System;

namespace SharedData.Models
{
    public class StockItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double InitPrice { get; set; }
        public double FinalPrice { get; set; }
        public DateTime UpdateTime { get; set; }
    }

    public class StockItemCurrentPrice
    {
        public int Id { get; set; }
        public double CurrentPrice { get; set; }
        public double FinalPrice { get; set; }
        public double PriceChangePercentage { get; set; }

        public DateTime  UpdateTime { get; set; }
    }




}
