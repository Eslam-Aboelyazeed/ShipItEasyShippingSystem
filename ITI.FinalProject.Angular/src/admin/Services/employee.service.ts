import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GenericService } from './generic.service';
import { IEmployee } from '../DTOs/DisplayDTOs/IEmployee';
import { IEmployeeInsert } from '../DTOs/InsertDTOs/IEmployeeInsert';
import { IEmployeeUpdate } from '../DTOs/UpdateDTOs/IEmployeeUpdate';
@Injectable({
  providedIn: 'root'
})
export class EmployeeService extends GenericService<
IEmployee,
  IEmployeeInsert,
  IEmployeeUpdate

>{
  [x: string]: any;
  constructor(httpClient: HttpClient) {
    super(httpClient);
    this.baseUrl = 'Employee';
  }
}