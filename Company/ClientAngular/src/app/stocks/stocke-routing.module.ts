import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ConfigActivator } from './services/config-actovator.service';
import { StockMarketViewComponent } from './stock-market-view/stock-market-view.component';

const routes2: Routes = [
  {
    path: '',
    component: StockMarketViewComponent,
    canActivate: [ConfigActivator],
    canDeactivate: [ConfigActivator],
    children: [
      {
        path: 'marketview',
        component: StockMarketViewComponent,
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes2)],
  exports: [RouterModule],
})
export class StocksRoutingModule {}
