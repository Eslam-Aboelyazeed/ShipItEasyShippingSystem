import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { GenericService } from './generic.service';
import { IGovernorate } from '../DTOs/DisplayDTOs/IGovernorate';
import { IGovernorateInsert } from '../DTOs/InsertDTOs/IGovernorateInsert';
import { IGovernorateUpdate } from '../DTOs/UpdateDTOs/IGovernorateUpdate';

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
