import { HttpClient } from '@angular/common/http';
import {
  ChangeDetectionStrategy,
  Component,
  OnDestroy,
  ViewChild,
} from '@angular/core';
import { tap, takeUntil, takeWhile, Observable, BehaviorSubject } from 'rxjs';
import {
  StockItemCurrentPrice,
  StockItems,
  StockItemsAndCurrentPrice,
} from './stocks/interfaces/stock.interface';
import { SignalrService } from './stocks/services/signalr.service';
import { Timestamp } from 'google-protobuf/google/protobuf/timestamp_pb';
import { AgGridAngular } from 'ag-grid-angular';
import {
  CellClickedEvent,
  ColDef,
  GetRowIdFunc,
  GetRowIdParams,
  GridApi,
  GridReadyEvent,
  ValueGetterFunc,
  ValueGetterParams,
} from 'ag-grid-community';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  changeDetection: ChangeDetectionStrategy.Default,
})
export class AppComponent  {

  
 

  constructor( ) {}
 

  
  title = 'signalRAngular';
}
