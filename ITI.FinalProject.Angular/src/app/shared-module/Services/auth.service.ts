import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private cookieService:CookieService) {}

  getToken(): string | null{
    if (this.cookieService.get('Token') != '') {
      return this.cookieService.get('Token');
    }else{
      return null;
    }
  }

  setToken(token: string): void {
    this.cookieService.set('Token', token, 1, '/');
  }

  getRole(){
    if (typeof localStorage !== 'undefined'){
      return localStorage.getItem('Role');
    }else{
      return null
    }
  }

  getRepresentativeId(){
    if (typeof localStorage !== 'undefined'){
      return localStorage.getItem('RepresentativeId');
    }else{
      return null
    }
  }

  getName(){
    if (typeof localStorage !== 'undefined'){
      return localStorage.getItem('UserName');
    }else{
      return null
    }
  }

  setName(name:string){
    if (typeof localStorage !== 'undefined') {
      localStorage.setItem('UserName', name);
    }
  }

  setId(id:string){
    if (typeof localStorage !== 'undefined') {
      localStorage.setItem('Id', id);
    }
  }

  setRole(role:string){
    if (typeof localStorage !== 'undefined') {
      localStorage.setItem('Role', role);
    }
  }

  getId(){
    if (typeof localStorage !== 'undefined'){
      return localStorage.getItem('Id');
    }else{
      return null
    }
  }

  setMerchantId(id:string){
    if (typeof localStorage !== 'undefined') {
      localStorage.setItem('MerchantId', id);
    }
  }

  getMerchantId(){
    if (typeof localStorage !== 'undefined'){
      return localStorage.getItem('MerchantId');
    }else{
      return null
    }
  }
  
  setRepresentativeId(id:string){
    if (typeof localStorage !== 'undefined') {
      localStorage.setItem('RepresentativeId', id);
    }
  }

  clear(){
    if (typeof localStorage !== 'undefined') {
      localStorage.clear();
      this.cookieService.deleteAll();
    }
  }
}

