import { AuthService } from './../Services/auth.service';
import { Injectable, inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

@Injectable({providedIn: 'root'})
  export class RepresentativeAuthorizationService {

  constructor(
  public router: Router,
  private authService:AuthService
  ) { }

  canActivate(): boolean {
    if (this.authService.getToken()) {
      if ((this.authService.getRole() && (this.authService.getRole() == "Representative"))) {
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

export const representativeAuthorizationGuard: CanActivateFn = (route, state) => {
  return inject(RepresentativeAuthorizationService).canActivate();
};
