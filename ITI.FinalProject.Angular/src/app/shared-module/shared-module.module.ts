import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SharedModuleRoutingModule } from './shared-module-routing.module';
import { OrderReportComponent } from './Components/order-report/order-report.component';

@NgModule({
  declarations: [],
  imports: [CommonModule, SharedModuleRoutingModule, OrderReportComponent],
  exports: [OrderReportComponent],
})
export class SharedModuleModule {}
