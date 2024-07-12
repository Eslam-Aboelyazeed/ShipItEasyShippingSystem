import { Injectable } from '@angular/core';
import { GenericService } from './generic.service';
import { IAddShipping } from '../DTOs/Insert DTOs/IAddShipping';
import { IUpdateShipping } from '../DTOs/Update DTOs/IUpdateShipping';
import { IDisplayShipping } from '../DTOs/Display DTOs/IDisplayShipping';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class ShippingService extends GenericService<
  IDisplayShipping,
  IAddShipping,
  IUpdateShipping
> {
  constructor(httpClient: HttpClient) {
    super(httpClient);
    this.baseUrl = 'Cities';
  }
}
