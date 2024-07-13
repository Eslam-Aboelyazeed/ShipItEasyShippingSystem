import { Component, OnDestroy, OnInit } from '@angular/core';
import { GenericService } from '../../Services/generic.service';
import { IDisplayOrder } from '../../DTOs/DisplayDTOs/IDisplayOrder';
import { IAddOrder } from '../../DTOs/InsertDTOs/IAddOrder';
import { IOrderNewUpdate } from '../../DTOs/UpdateDTOs/IOrderNewUpdate';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { OrderStatus } from '../../Enums/OrderStatus';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { IRepresentative } from '../../../../admin/DTOs/DisplayDTOs/IRepresentative';
import { IRepresentativeInsert } from '../../../../admin/DTOs/InsertDTOs/IRepresentativeInsert';
import { IRepresentativeUpdate } from '../../../../admin/DTOs/UpdateDTOs/IRepresentativeUpdate';
import { error } from 'console';
import { Status } from '../../../../admin/Enums/Status';
import { IOption } from '../../DTOs/DisplayDTOs/IOption';
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

  rSub:any;

  gSub:any;

  showRep:boolean;

  representatives:IOption[];

  governorates:IOption[];

  constructor(
    private orderService: GenericService<IDisplayOrder, IAddOrder, IOrderNewUpdate>,
    private representativeService: GenericService<IRepresentative, IRepresentativeInsert, IRepresentativeUpdate>,
    private governorateService: GenericService<IOption, any, any>,
    private router:Router,
    private activatedRoute:ActivatedRoute
  ){
    this.form = new FormGroup({
      orderStatus: new FormControl(-1, [Validators.required, Validators.min(0)]),
      representativeId : new FormControl('')
    })

    this.orderStatus = OrderStatus;

    this.options = Object.keys(this.orderStatus).map(key => this.orderStatus[key]);

    this.options = this.options.filter(el => typeof(el) != "number" && (el == "New" || el == "Pending" || el == "Cancelled" || el == "RepresentativeDelivered"));

    this.id = 0;

    this.representatives = [];
    this.governorates = [];

    this.showRep = true;
  }
  ngOnInit(): void {
    this.arSub = this.activatedRoute.params.subscribe({
      next: id => {
        console.log(id['id']);
        this.id = id['id'];

        this.rSub = this.representativeService.GetOptions('http://localhost:5241/api/representativeOptions').subscribe({
          next: data => {
            this.representatives = data;
            this.gSub = this.governorateService.GetOptions('http://localhost:5241/api/governorateOptions').subscribe({
              next: data => {
                this.governorates = data;
                this.orderSub = this.orderService.GetById(`http://localhost:5241/api/orders/${this.id}`).subscribe({
                  next: data => {
                    if (data) {
                      this.form.controls['representativeId'].setValue(this.representatives.filter(r => r.name == data.representativeName)[0]?.id??'');
                      this.form.controls['orderStatus'].setValue(data.status);
                      this.representatives = this.representatives.filter(r => r.dependentIds?.includes(this.governorates.filter(g => g.name == data.governorateName)[0].id as number));
                      if (data.status == OrderStatus.Cancelled) {
                        this.showRep = false;
                        this.form.controls['representativeId'].setValue('');
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
    if (this.rSub) {
      this.rSub.unsubscribe();
    }
    if (this.gSub) {
      this.gSub.unsubscribe();
    }
  }
  
  onStatusChange(){
    if (this.form.controls['orderStatus'].value == OrderStatus.Cancelled) {
      this.showRep = false;
      this.form.controls['representativeId'].setValue('');
    }else{
      this.showRep = true;
    }
  }

  getEnumIndexFromString(enumName: string): number {
    const keys = Object.keys(OrderStatus).filter(key => isNaN(Number(key))); 
    return keys.indexOf(enumName);
  }

  updateOrder(){

    let order:IOrderNewUpdate = {
      id:Number(this.id),
      orderStatus:Number(this.form.controls['orderStatus'].value),
      representativeId:this.form.controls['representativeId'].value 
    }
    console.log(order)
    this.oSub = this.orderService.Edit(`http://localhost:5241/api/orders/${this.id}`,order).subscribe({
      next: data => {
        console.log(data);
        this.router.navigate(['/employee/order']);
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
