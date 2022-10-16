using SharedData.proto;
using System.Text.Json.Serialization;

namespace ProtoBufGeneratedClasses.Messages
{
    public record StockItemsMessage([property: JsonPropertyName("items")] List<StockItemCurrentPrice> stockItemCurrentPrice);
}