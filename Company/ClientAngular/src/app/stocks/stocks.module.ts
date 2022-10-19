import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StockMarketViewComponent } from './stock-market-view/stock-market-view.component';
import { AgGridModule } from 'ag-grid-angular';
import { SignalrService } from './services/signalr.service';
import { StocksRoutingModule } from './stocke-routing.module';



@NgModule({
  declarations: [
    StockMarketViewComponent,StockMarketViewComponent
  ],
  imports: [
    CommonModule,AgGridModule,StocksRoutingModule
  ],
  exports:[StockMarketViewComponent],
  providers:[SignalrService]
})
export class StocksModule { }
