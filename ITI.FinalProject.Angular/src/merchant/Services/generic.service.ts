import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IPaginationDTO } from '../DTOs/Display DTOs/IPaginationDTO';
import { IOption } from '../../app/shared-module/DTOs/DisplayDTOs/IOption';

@Injectable({
  providedIn: 'root',
})
export class GenericService<
  T1 extends object,
  T2 extends object,
  T3 extends object
> {
  baseUrl: string;
  
  constructor(private httpClient: HttpClient) {
    this.baseUrl = '';
  }

  GetAll(url: string) {
    return this.httpClient.get<T1[]>(url);
  }

  GetPage(url: string) {
    return this.httpClient.get<IPaginationDTO<T1>>(url);
  }

  GetOptions(url:string){
    return this.httpClient.get<IOption[]>(
      url
    );
  }

  GetById(url: string) {
    return this.httpClient.get<T1 | undefined>(url);
  }

  Add(url: string, element: T2) {
    return this.httpClient.post<any>(url, element);
  }

  Edit(url: string, element: T3) {
    return this.httpClient.put(url, element);
  }

  Delete(url: string) {
    return this.httpClient.delete(url);
  }
}
