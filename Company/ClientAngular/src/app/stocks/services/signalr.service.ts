import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import {
  StockItemCurrentPrice,
  StockItems,
  StockItemsAndCurrentPrice,
} from '../interfaces/stock.interface';
import { environment } from 'src/environments/environment';
import { BehaviorSubject, combineLatest, map, tap } from 'rxjs';
import { HttpClient } from '@angular/common/http';
@Injectable()
export class SignalrService {
  /**
   *
   */
  constructor(public http: HttpClient) {}
  public stockItemCurrentPrice$ = new BehaviorSubject<StockItemCurrentPrice[]>(
    []
  );
  public stockItems$ = new BehaviorSubject<StockItems[]>([]);
  private stockItemsAndCurrentPriceDict: Record<
    number,
    StockItemsAndCurrentPrice
  > = {};
  public stockItemsAndCurrentPrice$ = new BehaviorSubject<
    StockItemsAndCurrentPrice[]
  >([]);

  private hubConnection!: signalR.HubConnection;
  public startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withAutomaticReconnect([3, 10, 20, 30, 30, 60])
      .withUrl(`${environment.stockServerUrl}/StockItemsHub`)
      .build();
    this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started');

        this.onSendStockItemCurrentPrice();
        //  this.onSendStockItems();
      })
      .catch((err) => console.log('Error while starting connection: ' + err));

    this.hubConnection.onreconnected(() => {
      console.log('reconnected');
    });
    this.hubConnection.onreconnecting(() => {
      console.log('..........onreconnecting.....');
    });
  };

  private onSendStockItemCurrentPrice() {
    this.hubConnection.on(
      'SendStockItemCurrentPrice',
      (stockItemCurrentPrice: StockItemCurrentPrice[]) => {
        for (let currentPriceItem of stockItemCurrentPrice) {
          let it = this.stockItemsAndCurrentPriceDict[currentPriceItem.id];
          if (!it) continue;
          it.updateTime = { ...currentPriceItem.updateTime };
          it.priceChangePercentage = currentPriceItem.priceChangePercentage;
          it.currentPrice = currentPriceItem.currentPrice;
        }

        this.stockItemCurrentPrice$.next(stockItemCurrentPrice);

        this.stockItemsAndCurrentPrice$.next(JSON.parse( JSON.stringify( Object.values(this.stockItemsAndCurrentPriceDict))));
        console.log(stockItemCurrentPrice);
      }
    );
  }


  public getAllStockItems() {
    this.http
      .get<StockItems[]>(
        `${environment.stockServerUrl}/api/Subscriber/allStockItems`
      )
      .pipe(
        tap((stockItems) => {
          this.stockItemsAndCurrentPriceDict = {};
          for (let stock of stockItems)
            this.stockItemsAndCurrentPriceDict[stock.id] = {
              ...stock,
              priceChangePercentage: 0,
              currentPrice: 0,
            };
            this.stockItemsAndCurrentPrice$.next(JSON.parse( JSON.stringify( Object.values(this.stockItemsAndCurrentPriceDict)))); })
      )
      .subscribe((rs) => this.stockItems$.next(rs));
  }
}
