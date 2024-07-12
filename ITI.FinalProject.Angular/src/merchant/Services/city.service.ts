import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GenericService } from './generic.service';
import { IDisplayCity } from '../DTOs/Display DTOs/IDisplayCity';
import { IAddCity } from '../DTOs/Insert DTOs/IAddCity';
import { IUpdateCity } from '../DTOs/Update DTOs/IUpdateCity';

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
