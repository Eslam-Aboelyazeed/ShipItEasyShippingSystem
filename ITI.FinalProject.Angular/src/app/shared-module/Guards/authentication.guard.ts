import { Injectable, inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../Services/auth.service';

@Injectable({providedIn: 'root'})
export class AuthenticationService {

  constructor(
  public router: Router,
  private authService:AuthService
  ) { }

  canActivate(): boolean {
    if (this.authService.getToken()) {
      if ((this.authService.getRole())) {
        return true
      }
      return false;
    } else {
      this.router.navigate(['/login']);
      return false
    }
  }

}

export const authenticationGuard: CanActivateFn = (route, state) => {
  return inject(AuthenticationService).canActivate();
};
