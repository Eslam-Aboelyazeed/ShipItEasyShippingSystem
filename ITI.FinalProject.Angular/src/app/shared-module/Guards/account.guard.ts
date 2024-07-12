import { Injectable, inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../Services/auth.service';

@Injectable({providedIn:"root"})
export class RegisterationService{

  constructor(private router:Router, private authService:AuthService) { }

  canActivate():boolean {
    if (this.authService.getToken()) {
      this.router.navigate(['/']);
      return false;
    }else{
      return true;
    }
  }
}
export const accountGuard: CanActivateFn = (route, state) => {
  return inject(RegisterationService).canActivate();
};
