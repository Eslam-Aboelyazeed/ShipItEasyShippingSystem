import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnDestroy } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AccountService } from '../../Services/account.service';
import { CommonModule } from '@angular/common';
import { LoginCredentials } from '../../../../admin/DTOs/InsertDTOs/IAccount';
import Swal from 'sweetalert2';
import { JwtHelperService } from '@auth0/angular-jwt';
import { AuthService } from '../../Services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  imports:[ReactiveFormsModule,CommonModule],
standalone:true
})
export class LoginComponent implements OnDestroy {
  
  
  


  
  
  
  
  
  

  
  

  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  






  




  

  




  

  
  















  zSub:any;
  buttonMessage = 'Login';
  LoginForm: FormGroup;
  errorMessage = '';

  constructor(private accountService: AccountService, private router: Router, private authService:AuthService) {
    this.LoginForm = new FormGroup({
      userName: new FormControl('', [Validators.required]),
      password: new FormControl('', [Validators.required]),
    });
  }

  ngOnDestroy(): void {
    if (this.zSub != undefined) {
      this.zSub.unsubscribe();
    }
  }

  LoginSubmitHandler() {
    const { userName, password } = this.LoginForm.value;
    const loginCredentials: LoginCredentials = {
      emailOrUserName: userName,
      password: password,
    };
    this.buttonMessage = 'Loading...';
    this.zSub = this.accountService.Login(loginCredentials).subscribe({
      next: (data) => {
        console.log(data);
        let jwtHelper:JwtHelperService = new JwtHelperService();
        let token = jwtHelper.decodeToken(data.token);
       
        this.authService.setToken(data.token)
        this.authService.setId(token['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'])
        this.authService.setName(token['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'])
        this.authService.setRole(token['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'])
        if (this.authService.getRole() == 'Merchant') {
          this.authService.setMerchantId(token['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']);
        }

        if (this.authService.getRole() == 'Representative') {
          this.authService.setRepresentativeId(token['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']);
        }
        this.router.navigateByUrl('/');
      },
      error: (e: HttpErrorResponse) => {
        
        
        
        
        
        
        
        if (e.error.message) {          
          Swal.fire({
            icon: "error",
            title: "Error",
            text: e.error.message,
          })
        }else{
          Swal.fire({
            icon: "error",
            title: "Error",
            text: "Something went wrong, please try again later",
          })
        }
      },
    });
  }
}