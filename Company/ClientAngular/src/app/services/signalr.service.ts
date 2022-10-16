import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';

@Injectable({
  providedIn: 'root',
})
export class SignalrService {
  public data: any[] = [];
  private hubConnection!: signalR.HubConnection;
  public startConnection = () => {
    'http://localhost:5006/StockItemsHub'
    'http://localhost:3602/v1.0/invoke/FrontApiStockMarket/method/StockItemsHub'
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:3602/v1.0/invoke/FrontApiStockMarket/method/StockItemsHub')
      .build();
    this.hubConnection
      .start()
      .then(() => {console.log('Connection started');
      this.hubConnection.on("SendStockItems",(data) => {
        this.data = data;
        console.log(data);
      })
    })
      .catch((err) => console.log('Error while starting connection: ' + err));
  };

  public addTransferChartDataListener = () => {
    this.hubConnection.on('transferchartdata', (data) => {
      this.data = data;
      console.log(data);
    });
  };
}
