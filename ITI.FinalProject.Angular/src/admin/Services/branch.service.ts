import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { IBranchInsert } from '../DTOs/InsertDTOs/IBranchInsert';
import { IBranchUpdate } from '../DTOs/UpdateDTOs/IBranchUpdate';
import { IBranch } from '../DTOs/DisplayDTOs/IBranch';
import { IDisplayBranch } from '../../merchant/DTOs/Display DTOs/IDisplayBranch';
import { IAddBranch } from '../../merchant/DTOs/Insert DTOs/IAddBranch';
import { IUpdateBranch } from '../../merchant/DTOs/Update DTOs/IUpdateBranch';
import { GenericService } from './generic.service';

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
