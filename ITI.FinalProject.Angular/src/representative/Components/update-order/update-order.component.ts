import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { error } from 'console';
import { IDeliveredOrderUpdate } from '../../DTOs/IDeliveredOrderUpdate';
import { GenericService } from '../../../admin/Services/generic.service';
import { OrderStatus } from '../../../admin/Enums/OrderStatus';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-update-order',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './update-order.component.html',
  styleUrl: './update-order.component.css'
})
export class UpdateOrderComponent implements OnInit, OnDestroy {
  orderStatus:any;

  options:string[];

  form:FormGroup;

  id:number;

  oSub:any;

  orderSub:any;

  arSub:any;

  

  showInputs:boolean;

  

  constructor(private orderService: GenericService<any, any, IDeliveredOrderUpdate>,
    
    private router:Router,
    private activatedRoute:ActivatedRoute
  ){
    this.form = new FormGroup({
      orderStatus: new FormControl(-1, [Validators.required, Validators.min(0)]),
      
      orderMoneyReceived: new FormControl(0),
      shippingMoneyReceived:  new FormControl(0),
      notes:  new FormControl('')
    })

    this.orderStatus = OrderStatus;

    this.options = Object.keys(this.orderStatus).map(key => this.orderStatus[key]);

    this.options = this.options.filter(el => typeof(el) != "number");

    this.id = 0;

    

    this.showInputs = true;
  }
  ngOnInit(): void {
    this.arSub = this.activatedRoute.params.subscribe({
      next: id => {
        console.log(id['id']);
        this.id = id['id'];

        
        
        
            
            this.orderSub = this.orderService.GetById(`http://localhost:5241/api/orders/${this.id}`).subscribe({
              next: data => {
                if (data) {
                  this.form.controls['orderStatus'].setValue(data.status);
                  if (data.status == OrderStatus.Cancelled) {
                    this.showInputs = false;
                    this.form.controls['orderMoneyReceived'].setValue(0);
                    this.form.controls['shippingMoneyReceived'].setValue(0);
                    this.form.controls['notes'].setValue('');
                  }
                }
              },
              error: error => {
                if (error.status == 401) {
                  Swal.fire({
                    icon: "error",
                    title: "Error",
                    text: "Unauthorized",
                  })
                }
                else if (error.error.message) {          
                  Swal.fire({
                    icon: "error",
                    title: "Error",
                    text: error.error.message,
                  })
                }else{
                  Swal.fire({
                    icon: "error",
                    title: "Error",
                    text: "Something went wrong, please try again later",
                  })
                }
              }
            });
          
          
          
          
        
      },
      error: error => {
        Swal.fire({
          icon: "error",
          title: "Error",
          text: "Something went wrong, please try again later",
        })
      }
    })
  }
  ngOnDestroy(): void {
    if (this.oSub) {
      this.oSub.unsubscribe();
    }
    if (this.orderSub) {
      this.orderSub.unsubscribe();
    }
    if (this.arSub) {
      this.arSub.unsubscribe();
    }
    
    
    
  }
  
  onStatusChange(){
    if (this.form.controls['orderStatus'].value == OrderStatus.Cancelled || this.form.controls['orderStatus'].value == OrderStatus.Unreachable || this.form.controls['orderStatus'].value == OrderStatus.RejectedWithoutPayment) {
      this.showInputs = false;
      this.form.controls['representativeId'].setValue('');
      this.form.controls['orderMoneyReceived'].setValue(0);
      this.form.controls['shippingMoneyReceived'].setValue(0);
      
    }else{
      this.showInputs = true;
    }
  }

  updateOrder(){

    let order:IDeliveredOrderUpdate = {
      id:Number(this.id),
      orderStatus:Number(this.form.controls['orderStatus'].value),
      
      orderMoneyReceived:Number(this.form.controls['orderMoneyReceived'].value),
      shippingMoneyReceived:Number(this.form.controls['shippingMoneyReceived'].value),
      notes:this.form.controls['notes'].value
    }
    console.log(order)
    this.oSub = this.orderService.Edit(`http://localhost:5241/api/RepresentativeOrder/${this.id}`,order).subscribe({
      next: data => {
        console.log(data);
        this.router.navigate(['/representative/orders']);
      },
      error: error => {
        if (error.status == 401) {
          Swal.fire({
            icon: "error",
            title: "Error",
            text: "Unauthorized",
          })
        }
        else if (error.error.message) {          
          Swal.fire({
            icon: "error",
            title: "Error",
            text: error.error.message,
          })
        }else{
          Swal.fire({
            icon: "error",
            title: "Error",
            text: "Something went wrong, please try again later",
          })
        }
      }
    })
  }
}
