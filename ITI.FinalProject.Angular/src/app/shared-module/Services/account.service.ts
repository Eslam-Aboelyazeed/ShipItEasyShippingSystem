import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { IAccountView, LoginCredentials, LoginResponse } from '../../../admin/DTOs/InsertDTOs/IAccount';
import { GenericService } from '../../../admin/Services/generic.service';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class AccountService  {
  IsLogged = false;
  Role: string[] = [];
  private token = '';
  Name = '';
  
  constructor(private router: Router,
  private httpClient:HttpClient, 
  private genericService :GenericService<IAccountView, LoginCredentials, any>,
  private authService: AuthService
  ) { 
  
  }

  GetToken() {
    return this.token;
  }

  SetToken(newToken: string) {
    this.token = newToken;
  }

  CheckRole(Role: string) {
    return this.Role.includes(Role);
  }

  Login(Credentials: LoginCredentials): Observable<any> {
    return this.httpClient.post<any>('http://localhost:5241/api/login' , Credentials);
  }

  LogOut() {
    return this.httpClient.get(`http://localhost:5241/api/logout?UserId=${this.authService.getId()}`)
  }

}
