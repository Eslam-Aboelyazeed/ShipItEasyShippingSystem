import { GovernorateAddComponent } from './Components/governorate-add/governorate-add.component';
import { RepresentativeListComponent } from './Components/representative-list/representative-list.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MerchantAddComponent } from './Components/merchant-add/merchant-add.component';
import { RepresentativeFormComponent } from './Components/representative-form/representative-form.component';
import { BranchFromComponent } from './Components/branch-from/branch-from.component';
import { GovernorateListComponent } from './Components/governorate-list/governorate-list.component';
import { BranchListComponent } from './Components/branch-list/branch-list.component';
import { MerchantListComponent } from './Components/merchant-list/merchant-list.component';
import { MerchantEditComponent } from './Components/merchant-edit/merchant-edit.component';
import { RolePowersListComponent } from './Components/role-powers-list/role-powers-list.component';
import { GovernorateEditComponent } from './Components/governorate-edit/governorate-edit.component';
import { RolePowersEditComponent } from './Components/role-powers-edit/role-powers-edit.component';
import { CityEditComponent } from './Components/city-edit/city-edit.component';
import { CityListComponent } from './Components/city-list/city-list.component';
import { CityAddComponent } from './Components/city-add/city-add.component';
import { EmployeshowComponent } from './Components/employee-show/employe-show.component';
import { EmployeeAddComponent } from './Components/employee-add/employee-add.component';
import { EmployeeEditComponent } from './Components/employee-edit/employee-edit.component';

const routes: Routes = [
  { path: 'representative',component:RepresentativeListComponent},
  { path: 'representative/add', component: RepresentativeFormComponent },
  { path: 'representative/edit/:id', component: RepresentativeFormComponent },
  { path: 'merchant/add', component: MerchantAddComponent },
  { path: 'merchant/edit/:id', component: MerchantEditComponent },
  { path: 'merchant', component: MerchantListComponent },
  { path: 'branch', component:BranchListComponent},
  { path: 'branch/add', component: BranchFromComponent },
  { path: 'branch/edit/:id', component: BranchFromComponent },
  { path: 'governorate', component:GovernorateListComponent},
  { path: 'governorate/add', component:GovernorateAddComponent},
  { path: 'role', component:RolePowersListComponent},
  { path: 'governorate/edit/:id', component:GovernorateEditComponent },
  { path: 'role/edit/:id', component:RolePowersEditComponent },
  { path: 'city/edit/:id', component: CityEditComponent },
  { path: 'city', component:CityListComponent},
  { path: 'city/add', component:CityAddComponent},
  { path: 'employee', component:EmployeshowComponent},
  { path: 'employee/add', component:EmployeeAddComponent},
  { path: 'employee/edit/:id', component:EmployeeEditComponent},
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminRoutingModule {}
