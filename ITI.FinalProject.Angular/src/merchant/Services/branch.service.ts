import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { GenericService } from './generic.service';
import { IDisplayBranch } from '../DTOs/Display DTOs/IDisplayBranch';
import { IAddBranch } from '../DTOs/Insert DTOs/IAddBranch';
import { IUpdateBranch } from '../DTOs/Update DTOs/IUpdateBranch';

@Injectable({
  providedIn: 'root',
})
export class BranchService extends GenericService<
  IDisplayBranch,
  IAddBranch,
  IUpdateBranch
> {
  constructor(httpClient: HttpClient) {
    super(httpClient);
    this.baseUrl = 'Branches';
  }
}
