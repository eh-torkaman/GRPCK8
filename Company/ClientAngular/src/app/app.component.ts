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
import { type } from 'os';
import { json } from 'stream/consumers';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  changeDetection: ChangeDetectionStrategy.Default,
})
export class AppComponent implements OnDestroy {
  private componentIsAlive = true;
  stockItemCurrentPrice$ = this.signalRService.stockItemCurrentPrice$;
  stockItems$ = this.signalRService.stockItems$;

  public columnDefs: ColDef<StockItemsAndCurrentPrice>[] = [
    { field: 'id', maxWidth: 100 },
    { field: 'name', maxWidth: 100 },
    { field: 'description' },
    { field: 'initPrice', maxWidth: 150 },
    { field: 'finalPrice', cellRenderer: 'agAnimateShowChangeCellRenderer' },
    { field: 'updateTime', valueGetter: this.getDate.bind(this) },
    {
      field: 'priceChangePercentage',
      cellRenderer: 'agAnimateShowChangeCellRenderer',
      maxWidth: 150,
    },
    { field: 'currentPrice' },
  ];
 
  public defaultColDef: ColDef = {
    sortable: true,
    filter: true,
  };
  public getRowId: GetRowIdFunc = (params: GetRowIdParams) => params.data.id;
  TimeStampToDate = (
    updateTime: undefined | { seconds: number; nanos: number }
  ) => {
    if (!updateTime) return '--';
    return new Date(
      updateTime.seconds * 1000 + updateTime.nanos / 1000_000
    ).toLocaleString();
    // // var time = new Timestamp();
    // time.setSeconds(updateTime.seconds);
    // time.setNanos(updateTime.nanos);
    // return ( time.toDate().toLocaleString()        );
  };
  getDate(params: ValueGetterParams<StockItemsAndCurrentPrice>) {
    let updateTime = params.data?.updateTime ?? { seconds: 0, nanos: 0 };
    console.log('updateTime', updateTime);
    return this.TimeStampToDate(updateTime);
  }

  // Data that gets displayed in the grid
  public rowData$!: Observable<any[]>;

  // For accessing the Grid's API
  @ViewChild(AgGridAngular) agGrid!: AgGridAngular;

  constructor(public signalRService: SignalrService, public http: HttpClient) {}

  ngOnDestroy(): void {
    this.componentIsAlive = false;
  }

  ngOnInit() {
    this.signalRService.startConnection();
    this.signalRService.start();
    
  }
  private gridApi!: GridApi;
  onGridReady(params: GridReadyEvent) {
    this.rowData$ = this.signalRService.stockItemsAndCurrentPrice$
      this.gridApi = params.api;
  }

  // Example of consuming Grid Event
  onCellClicked(e: CellClickedEvent): void {
    console.log('cellClicked', e);
  }

  
  title = 'signalRAngular';
}
