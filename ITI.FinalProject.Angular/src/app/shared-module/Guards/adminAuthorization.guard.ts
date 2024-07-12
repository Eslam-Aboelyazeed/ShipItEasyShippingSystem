import { AuthService } from './../Services/auth.service';
import { Injectable, inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

@Injectable({providedIn: 'root'})
  export class AdminAuthorizationService {

  constructor(
  public router: Router,
  private authService:AuthService
  ) { }

  canActivate(): boolean {
    if (this.authService.getToken()) {
      if ((this.authService.getRole() && (this.authService.getRole() != "Merchant" && this.authService.getRole() != "Representative"))) {
        return true
      }else { 
        this.router.navigate(['/']);
        return false;
      }
      return false;
    } else {
      this.router.navigate(['/login']);
      return false
    }
  }

}

export const adminAuthorizationGuard: CanActivateFn = (route, state) => {
  return inject(AdminAuthorizationService).canActivate();
};
