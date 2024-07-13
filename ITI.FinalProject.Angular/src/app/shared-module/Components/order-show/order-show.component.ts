import { IDisplayOrder } from '../../../../admin/DTOs/DisplayDTOs/IOrder';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { GenericService } from '../../../../admin/Services/generic.service';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { IAddOrder } from '../../../../merchant/DTOs/Insert DTOs/IAddOrder';
import { IUpdateOrder } from '../../../../merchant/DTOs/Update DTOs/IUpdateOrder';
import { FormsModule } from '@angular/forms';
import { OrderTypes } from '../../../../admin/Enums/OrderTypes';
import { OrderStatus } from '../../../../admin/Enums/OrderStatus';
import { Status } from '../../../../admin/Enums/Status';
import Swal from 'sweetalert2';
import { AuthService } from '../../Services/auth.service';

@Component({
  selector: 'app-order-show',
  standalone: true,
  imports: [RouterLink,CommonModule,FormsModule],
  templateUrl: './order-show.component.html',
  styleUrl: './order-show.component.css'
})
export class OrderShowComponent implements OnInit, OnDestroy {
  baseUrl:string="http://localhost:5241/api/";
  orders:IDisplayOrder[]=[]
  selectedEntries:number=5
  searchTerm:string=""
  currentPage:number=1
  totalOrders:number=0
  pagesNum:number=1
  startIndex:number=0
  endIndex:number=0
  orderStatus:number|null=null
  orderStatusName:string[]=[]
  oSub:any;

  isNotMerchant:boolean;
  constructor(public orderServ:GenericService<IDisplayOrder,IAddOrder,IUpdateOrder>, private authService:AuthService) {
    this.isNotMerchant = true;
  }

  ngOnDestroy(): void {
    if (this.oSub != undefined) {
      this.oSub.unsubscribe();
    }
  }

  ngOnInit(): void {
    this.getOrders();
    this.getOderStatus();
    (this.authService.getRole() == 'Merchant')? this.isNotMerchant = false : this.isNotMerchant = true;
  }

  getOderStatus(){
    let status = OrderStatus.New
    for(let i=0 ; i<11 ;i++)
    {
      this.orderStatusName.push(OrderStatus[status])
      status++;
    }
    console.log(this.orderStatusName);


  }

  StatusName(s:OrderStatus){
    return OrderStatus[s]
  }
  getOrders(){
    let getPageURL;
    if(this.orderStatus== null)
    {
      getPageURL=`${this.baseUrl}OrderPage?pageNumber=${this.currentPage}&pageSize=${this.selectedEntries}`
    }
    else{
      getPageURL=`${this.baseUrl}OrderPage?pageNumber=${this.currentPage}&pageSize=${this.selectedEntries}&orderStatus=${this.orderStatus}`
    }
    this.oSub = this.orderServ.GetPage(getPageURL).subscribe({
      next:(value:any)=>{
        console.log(value);

        this.orders=value.list;
        console.log(this.orders);

        this.totalOrders=value.totalCount;
        this.pagesNum=value.totalPages;

        this.startIndex=(this.currentPage-1)*this.selectedEntries
        this.endIndex= this.orders?.length + this.startIndex

      },
      error:(error)=> {
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

      },
    })
  }

  changeOrderStatus(i:number){
    if (i == -1) {
      this.orderStatus = null;
    }else{
      this.orderStatus=i;
    }
    this.getOrders()
  }

  onEntriesChange(){
    this.getOrders();
  }

  onSearchChange(){
    this.getOrders();

  }

  previousPage(){
    if(this.currentPage!=1)
    {
      this.currentPage--;
      this.getOrders()
    }

  }
  goToPage(pageIndex:number)
  {
    this.currentPage=pageIndex
    this.getOrders()
  }
  nextPage(){
    if(this.currentPage!=this.pagesNum)
      {
        this.currentPage++;
        this.getOrders()
      }
  }

}
