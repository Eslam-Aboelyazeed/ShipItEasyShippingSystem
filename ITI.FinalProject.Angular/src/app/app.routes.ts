import { Routes } from '@angular/router';
import { OrderReportComponent } from './shared-module/Components/order-report/order-report.component';
import { HomeComponent } from './shared-module/Components/home/home.component';
import { LoginComponent } from './shared-module/Components/login/login.component';
import { UpdateOrderComponent } from './shared-module/Components/update-order/update-order.component';
import { accountGuard } from './shared-module/Guards/account.guard';
import { authenticationGuard } from './shared-module/Guards/authentication.guard';
import { editOrderAuthenticationGuard } from './shared-module/Guards/editOrderAuthentication.guard';
import { adminAuthorizationGuard } from './shared-module/Guards/adminAuthorization.guard';
import { merchantAuthorizationGuard } from './shared-module/Guards/MerchantAuthorization.guard';
import { representativeAuthorizationGuard } from './shared-module/Guards/RepresentativeAuthorization.guard';
import { OrderShowComponent } from './shared-module/Components/order-show/order-show.component';
import { showOrderAuthorizationGuard } from './shared-module/Guards/showOrderAuthorization.guard';

export const routes: Routes = [
  {path:'',component:HomeComponent, canActivate:[authenticationGuard]},
  {path:'login',component:LoginComponent, canActivate:[accountGuard]},
  {
    path: 'admin',
    loadChildren: () =>
      import('../admin/admin.module').then((m) => m.AdminModule),
    canActivate:[adminAuthorizationGuard]
  },
  {
    path: 'employee',
    loadChildren: () =>
      import('../employee/employee.module').then((m) => m.EmployeeModule),
    canActivate:[adminAuthorizationGuard]
  },
  {
    path: 'merchant',
    loadChildren: () =>
      import('../merchant/merchant.module').then((m) => m.MerchantModule),
    canActivate:[merchantAuthorizationGuard]
  },
  {
    path: 'representative',
    loadChildren: () =>
      import('../representative/representative.module').then((m) => m.RepresentativeModule),
    canActivate:[representativeAuthorizationGuard]
  },
  {path:'orderReport',component:OrderReportComponent, canActivate:[authenticationGuard]},
  {path:'order/edit/:id',component:UpdateOrderComponent, canActivate:[editOrderAuthenticationGuard]},
  {path:'orderShow', component:OrderShowComponent, canActivate:[showOrderAuthorizationGuard]}
  
];
