import { Injectable } from '@angular/core';
import { GenericService } from './generic.service';
import { HttpClient } from '@angular/common/http';
import { IDisplayProduct } from '../DTOs/DisplayDTOs/IDisplayProduct';
import { IAddProduct } from '../DTOs/InsertDTOs/IAddProduct';
import { IUpdateProduct } from '../DTOs/UpdateDTOs/IUpdateProduct';

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
