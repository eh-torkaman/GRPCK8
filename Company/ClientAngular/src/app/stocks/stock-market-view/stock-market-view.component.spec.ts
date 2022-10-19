import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StockMarketViewComponent } from './stock-market-view.component';

describe('StockMarketViewComponent', () => {
  let component: StockMarketViewComponent;
  let fixture: ComponentFixture<StockMarketViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StockMarketViewComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StockMarketViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
