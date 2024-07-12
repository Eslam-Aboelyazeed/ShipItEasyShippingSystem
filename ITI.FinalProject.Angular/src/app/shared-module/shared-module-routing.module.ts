import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OrderReportComponent } from './Components/order-report/order-report.component';

const routes: Routes = [];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SharedModuleRoutingModule {}
