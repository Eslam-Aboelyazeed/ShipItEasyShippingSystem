import { SettingListComponent } from './components/setting-list/setting-list.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OrderShowComponent } from '../app/shared-module/Components/order-show/order-show.component';
import { SettingEditComponent } from './components/setting-edit/setting-edit.component';

const routes: Routes = [
  {path:"order",component:OrderShowComponent},
  {path:'setting/edit/:id',component:SettingEditComponent},
  {path:'setting',component:SettingListComponent}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class EmployeeRoutingModule { }
