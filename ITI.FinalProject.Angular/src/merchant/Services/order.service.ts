import { Injectable } from '@angular/core';
import { GenericService } from './generic.service';
import { HttpClient } from '@angular/common/http';
import { IAddOrder } from '../DTOs/Insert DTOs/IAddOrder';
import { IUpdateOrder } from '../DTOs/Update DTOs/IUpdateOrder';
import { IDisplayOrder } from '../DTOs/Display DTOs/IDisplayOrder';

@Injectable({
  providedIn: 'root',
})
export class OrderService extends GenericService<
  IDisplayOrder,
  IAddOrder,
  IUpdateOrder
> {
  constructor(httpClient: HttpClient) {
    super(httpClient);
    this.baseUrl = 'Merchant';
  }
}
