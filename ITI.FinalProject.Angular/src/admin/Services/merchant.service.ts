import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { GenericService } from './generic.service';
import { IDisplayMerchant } from '../DTOs/DisplayDTOs/IDisplayMerchant';
import { IAddMerchant } from '../DTOs/InsertDTOs/IAddMerchant';
import { IUpdateMerchant } from '../DTOs/UpdateDTOs/IUpdateMerchant';

@Injectable({
  providedIn: 'root',
})
export class MerchantService extends GenericService<
  IDisplayMerchant,
  IAddMerchant,
  IUpdateMerchant
> {
  constructor(httpClient: HttpClient) {
    super(httpClient);
    this.baseUrl = 'Merchant';
  }
}
