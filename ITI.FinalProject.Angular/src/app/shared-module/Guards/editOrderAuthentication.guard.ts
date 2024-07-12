import { Injectable, inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../Services/auth.service';

@Injectable({providedIn: 'root'})
export class EditOrderAuthenticationService {

  constructor(
  public router: Router,
  private authService:AuthService
  ) { }

  canActivate(): boolean {
    if (this.authService.getToken()) {
      if ((this.authService.getRole()) && (this.authService.getRole() != "Representative" && this.authService.getRole() != "Merchant")) {
        return true
      }
      return false;
    } else {
      this.router.navigate(['/login']);
      return false
    }
  }

}

export const editOrderAuthenticationGuard: CanActivateFn = (route, state) => {
  return inject(EditOrderAuthenticationService).canActivate();
};
