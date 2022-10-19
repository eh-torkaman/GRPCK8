import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { StockMarketViewComponent } from './stock-market-view/stock-market-view.component';



const routes2: Routes = [
  {
    path: '',
    component: StockMarketViewComponent
  }
  ,
  {
    path: 'marketview',
    component: StockMarketViewComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes2)],
  exports: [RouterModule]
})
export class StocksRoutingModule { }
