import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import {
  StockItemCurrentPrice,
  StockItems,
} from '../interfaces/stock.interface';
import { environment } from 'src/environments/environment'; 
import { BehaviorSubject } from 'rxjs';
@Injectable({
  providedIn: 'root',
})
export class SignalrService {
  
  public stockItemCurrentPrice$=new BehaviorSubject<StockItemCurrentPrice[]>([]);
  public stockItems$=new BehaviorSubject<StockItems[]> ([] ) ;

  private hubConnection!: signalR.HubConnection;
  public startConnection = () => {
 
    
    this.hubConnection = new signalR.HubConnectionBuilder().withAutomaticReconnect([3,10,20,30,30,60])
      .withUrl(`${environment.stockServerUrl}/StockItemsHub`)
      .build();
    this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started');

        this.onSendStockItemCurrentPrice();
        this.onSendStockItems();
      })
      .catch((err) => console.log('Error while starting connection: ' + err));

      this.hubConnection.onreconnected(()=>{console.log("reconnected")})
      this.hubConnection.onreconnecting(()=>{console.log("..........onreconnecting.....")})
  };

  private onSendStockItemCurrentPrice() {
    this.hubConnection.on(
      'SendStockItemCurrentPrice',
      (stockItemCurrentPrice: StockItemCurrentPrice[]) => {
        this.stockItemCurrentPrice$.next(stockItemCurrentPrice);
        console.log(stockItemCurrentPrice);
      }
    );
  }

  private onSendStockItems() {
    this.hubConnection.on('SendStockItems', (StockItems: StockItems[]) => {
      this.stockItems$.next( StockItems);
      console.log(StockItems);
    });
  }
}
