import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StockMarketViewComponent } from './stock-market-view/stock-market-view.component';
import { AgGridModule } from 'ag-grid-angular';
import { SignalrService } from './services/signalr.service';
import { StocksRoutingModule } from './stocke-routing.module';
import { ConfigActivator } from './services/config-actovator.service';



@NgModule({
  declarations: [
    StockMarketViewComponent,StockMarketViewComponent
  ],
  imports: [
    CommonModule,AgGridModule,StocksRoutingModule
  ],
  exports:[StockMarketViewComponent],
  providers:[SignalrService,ConfigActivator]
})
export class StocksModule { }
