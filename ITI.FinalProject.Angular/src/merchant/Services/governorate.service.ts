import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { GenericService } from './generic.service';
import { IGovernorate } from '../DTOs/Display DTOs/IGovernorate';
import { IGovernorateInsert } from '../DTOs/Insert DTOs/IGovernorateInsert';
import { IGovernorateUpdate } from '../DTOs/Update DTOs/IGovernorateUpdate';

@Injectable({
  providedIn: 'root',
})
export class GovernorateService extends GenericService<
  IGovernorate,
  IGovernorateInsert,
  IGovernorateUpdate
> {
  constructor(httpClient: HttpClient) {
    super(httpClient);
    this.baseUrl = 'Governorate';
  }
}
