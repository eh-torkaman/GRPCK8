import { HttpClient } from '@angular/common/http';
import {
  ChangeDetectionStrategy,
  Component,OnInit,
  OnDestroy,
  ViewChild,
} from '@angular/core';
import { tap, takeUntil, takeWhile, Observable, BehaviorSubject } from 'rxjs';
import {
  StockItemCurrentPrice,
  StockItems,
  StockItemsAndCurrentPrice,
} from '../interfaces/stock.interface';
import { SignalrService } from '../services/signalr.service';
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
  selector: 'app-stock-market-view',
  templateUrl: './stock-market-view.component.html',
  styleUrls: ['./stock-market-view.component.css']
})
export class StockMarketViewComponent implements OnInit {


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
    { field: 'currentPrice', cellRenderer: 'agAnimateShowChangeCellRenderer' },
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
    return this.TimeStampToDate(params.data?.updateTime ?? { seconds: 0, nanos: 0 });
  }
 // Data that gets displayed in the grid
 public rowData$!: Observable<any[]>;

 // For accessing the Grid's API
 @ViewChild(AgGridAngular) agGrid!: AgGridAngular;

 constructor(public signalRService: SignalrService, public http: HttpClient) {}

 ngOnInit() {
  this.signalRService.startConnection();
  this.signalRService.getAllStockItems();
  
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


}
