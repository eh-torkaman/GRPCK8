import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { SignalrService } from './services/signalr.service';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  public forecasts?: WeatherForecast[];

  constructor(public signalRService: SignalrService,public http: HttpClient) {
    http.get<WeatherForecast[]>('http://localhost:3602/v1.0/invoke/FrontApiStockMarket/method/weatherforecast').subscribe(result => {
      this.forecasts = result;
    }, error => console.error(error));


    
  }

  ngOnInit() {
    this.signalRService.startConnection();
    this.signalRService.addTransferChartDataListener();   
    this.startHttpRequest();
  }
  //'http://localhost:3602/v1.0/invoke/FrontApiStockMarket/method/StockItemsHub'
  private startHttpRequest = () => {
    // this.http.get('http://localhost:5006/StockItemsHub')
    //   .subscribe(res => {
    //     console.log(res);
    //   })
  }

  title = 'ClientAngular';
}

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}