import { Component, OnDestroy, OnInit } from '@angular/core';
import { OrderService } from '../../Services/order.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit, OnDestroy{
  orderStatuses: any={};
  zSub:any;
  constructor(private orderService:OrderService) { }
  
  ngOnInit(): void {
    this.zSub = this.orderService.getOrderStatuses().subscribe(statuses => {
      this.orderStatuses = statuses;
      console.log(this.orderStatuses);
    });
  }

  ngOnDestroy(): void {
    if (this.zSub != undefined) {
      this.zSub.unsubscribe();
    }
  }
  
}
