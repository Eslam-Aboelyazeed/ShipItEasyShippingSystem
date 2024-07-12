import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OrderShowComponent } from './Components/order-show/order-show.component';
import { UpdateOrderComponent } from './Components/update-order/update-order.component';

const routes: Routes = [
  {path:'orders', component:OrderShowComponent},
  {path:"order/edit/:id", component:UpdateOrderComponent}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RepresentativeRoutingModule { }
