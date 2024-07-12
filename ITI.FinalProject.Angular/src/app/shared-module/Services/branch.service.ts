import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { GenericService } from './generic.service';
import { IDisplayBranch } from '../DTOs/DisplayDTOs/IDisplayBranch';
import { IAddBranch } from '../DTOs/InsertDTOs/IAddBranch';
import { IUpdateBranch } from '../DTOs/UpdateDTOs/IUpdateBranch';


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
