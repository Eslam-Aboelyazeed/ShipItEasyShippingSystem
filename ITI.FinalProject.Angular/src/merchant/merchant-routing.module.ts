import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OrderAddComponent } from './Components/order-add/order-add.component';

const routes: Routes = [{ path: 'order/add', component: OrderAddComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class MerchantRoutingModule {}
