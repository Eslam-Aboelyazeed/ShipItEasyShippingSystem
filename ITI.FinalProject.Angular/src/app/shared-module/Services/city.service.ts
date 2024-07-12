import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GenericService } from './generic.service';
import { IDisplayCity } from '../DTOs/DisplayDTOs/IDisplayCity';
import { IAddCity } from '../DTOs/InsertDTOs/IAddCity';
import { IUpdateCity } from '../DTOs/UpdateDTOs/IUpdateCity';

@Injectable({
  providedIn: 'root',
})
export class CityService extends GenericService<
  IDisplayCity,
  IAddCity,
  IUpdateCity
> {
  constructor(httpClient: HttpClient) {
    super(httpClient);
    this.baseUrl = 'Cities';
  }
}
