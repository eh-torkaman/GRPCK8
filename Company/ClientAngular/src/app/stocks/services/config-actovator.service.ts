import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  CanDeactivate,
  RouterStateSnapshot,
} from '@angular/router';
import { SignalrService } from './signalr.service';
@Injectable()
export class ConfigActivator implements CanActivate, CanDeactivate<any> {
  public constructor(private  signalrService:SignalrService) {}

  public canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean {
    //this.config.lazyModuleLoaded();
    console.log('canActivate...');
    return true;
  }

  public canDeactivate(
    component: any,
    currentRoute: ActivatedRouteSnapshot,
    currentState: RouterStateSnapshot,
    nextState?: RouterStateSnapshot
  ): boolean {
    //this.config.lazyModuleUnloaded();
    console.log('canDeactivate...');
    this.signalrService.close();
    return true;
  }
}
