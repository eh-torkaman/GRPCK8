export interface UpdateTime {
    seconds: number;
    nanos: number;
}

export interface StockItems {
    id: number;
    name: string;
    description: string;
    initPrice: number;
    finalPrice: number;
    updateTime: UpdateTime;
}

 

export interface StockItemCurrentPrice {
    id: number;
    currentPrice: number;
    finalPrice: number;
    priceChangePercentage: number;
    updateTime: UpdateTime;
}