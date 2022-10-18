import { HttpClient } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, OnDestroy } from '@angular/core';
import { tap, takeUntil, takeWhile } from 'rxjs';
import {
  StockItemCurrentPrice,
  StockItems,
} from './stocks/interfaces/stock.interface';
import { SignalrService } from './stocks/services/signalr.service';
import { Timestamp } from 'google-protobuf/google/protobuf/timestamp_pb';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppComponent implements OnDestroy {
  private componentIsAlive = true;
  stockItemCurrentPrice$ = this.signalRService.stockItemCurrentPrice$;
  stockItems$ = this.signalRService.stockItems$;

  stockItemCurrentPrice: StockItemCurrentPrice[] = [];
  stockItems: StockItems[] = [];

  constructor(public signalRService: SignalrService, public http: HttpClient) {
    this.signalRService.stockItemCurrentPrice$.subscribe(
      (rs) => (this.stockItemCurrentPrice = rs)
    );

    this.signalRService.stockItems$.subscribe((rs) => (this.stockItems = rs));
  }

  public TimeStampToDate(
    updateTime: undefined | { seconds: number; nanos: number }
  ) {
    if (!updateTime) return '--';
    var a = new Timestamp();
    a.setSeconds(updateTime.seconds);
    a.setNanos(updateTime.nanos);
    return a.toDate().toLocaleString() + " --- "+new Date (updateTime.seconds*1000+ updateTime.nanos/1000_000).toLocaleString();
  }
  ngOnDestroy(): void {
    this.componentIsAlive = false;
  }

  ngOnInit() {
    this.signalRService.startConnection();
  }

  title = 'signalRAngular';
}
