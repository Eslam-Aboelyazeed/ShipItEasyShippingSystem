import { Injectable } from '@angular/core';
import { GenericService } from './generic.service';
import { IDisplayProduct } from '../DTOs/Display DTOs/IDisplayProduct';
import { IAddProduct } from '../DTOs/Insert DTOs/IAddProduct';
import { IUpdateProduct } from '../DTOs/Update DTOs/IUpdateProduct';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class ProductService extends GenericService<
  IDisplayProduct,
  IAddProduct,
  IUpdateProduct
> {
  constructor(httpClient: HttpClient) {
    super(httpClient);
    this.baseUrl = 'Cities';
  }
}
