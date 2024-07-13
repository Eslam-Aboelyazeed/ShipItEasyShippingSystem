import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { OrderService } from '../../Services/order.service';
import { IDisplayOrder } from '../../DTOs/DisplayDTOs/IDisplayOrder';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { OrderStatus } from '../../Enums/OrderStatus';
import { OrderTypes } from '../../Enums/OrderTypes';
import { PaymentTypes } from '../../Enums/PaymentTypes';
import { ShippingTypes } from '../../Enums/ShippingTypes';
import Swal from 'sweetalert2';

export function getOrderStatusText(status: OrderStatus): string {
  return OrderStatus[status];
}

export function getOrderTypeText(type: OrderTypes): string {
  return OrderTypes[type];
}

export function getPaymentTypeText(paymentType: PaymentTypes): string {
  return PaymentTypes[paymentType];
}

export function getShippingTypeText(shippingType: ShippingTypes): string {
  return ShippingTypes[shippingType];
}

@Component({
  selector: 'app-order-report',
  standalone: true,
  imports: [CommonModule, FormsModule],
  providers: [CurrencyPipe],
  templateUrl: './order-report.component.html',
  styleUrls: ['./order-report.component.css'],
})
export class OrderReportComponent implements OnInit, OnDestroy {

  baseURL:string="http://localhost:5241/api/";
  data: IDisplayOrder[] = [];
  searchTerm: string = '';
  currentPage: number = 1;
  selectedEntries: number = 5;
  totalPages: number = 0;
  totalCount: number = 0;
  startIndex: number = 0;
  endIndex: number = 0;
  filteredData: IDisplayOrder[] = [];
  pagedData: IDisplayOrder[] = [];
  orderStatuses = Object.values(OrderStatus).filter(
    (value) => typeof value === 'string'
  );
  selectedStatus: OrderStatus | '' = '';
  fromDate: string | null = null;
  toDate: string | null = null;
  private destroy$ = new Subject<void>();
  url: string = `${this.baseURL}Orderpage?pageNumber=1&pageSize=${this.selectedEntries}`;

  getStatusText(status: OrderStatus): string {
    return OrderStatus[status];
  }

  getTypeText(type: OrderTypes): string {
    return OrderTypes[type];
  }

  getPaymentTypeText(paymentType: PaymentTypes): string {
    return PaymentTypes[paymentType];
  }

  getShippingTypeText(shippingType: ShippingTypes): string {
    return ShippingTypes[shippingType];
  }

  constructor(
    private orderService: OrderService,
    private currencyPipe: CurrencyPipe
  ) {}

  ngOnInit() {
    this.loadOrders();
  }

  loadOrders() {
    this.orderService.GetPage(this.url).subscribe({
      next: (response: any) => {
        console.log(response);
        if (response && response.list && Array.isArray(response.list)) {
          this.data = response.list.map((order: IDisplayOrder) => ({
            ...order,
            statusText: getOrderStatusText(order.status),
            typeText: getOrderTypeText(order.type),
            paymentTypeText: getPaymentTypeText(order.paymentType),
            shippingTypeText: getShippingTypeText(order.shippingType),
          }));
          this.totalPages =
            response.totalPages ||
            Math.ceil(response.totalCount / this.selectedEntries);
          this.totalCount = response.totalCount;
          this.startIndex = (this.currentPage - 1) * this.selectedEntries + 1;
          this.endIndex = Math.min(
            this.currentPage * this.selectedEntries,
            this.totalCount
          );
        } else {
          console.error('Unexpected API response format:', response);
        }

        this.updateTable();
      },
      error: (error) => {
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
    });
  }

  onEntriesChange() {
    this.currentPage = 1;
    this.url = `${this.baseURL}Orderpage?pageNumber=1&pageSize=${this.selectedEntries}`;
    this.loadOrders();
    this.url = this.baseURL+'Orderpage';
  }

  onSearchChange() {
    this.currentPage = 1;
    this.loadOrders();
  }

  searchOrders() {
    console.log('Searching orders...');
    this.currentPage = 1;
    this.url = `${this.baseURL}Orderpage?pageNumber=${this.currentPage}&pageSize=${this.selectedEntries}`;

    if (this.selectedStatus !== null) {
      const statusValue: unknown =
        OrderStatus[this.selectedStatus as unknown as keyof typeof OrderStatus];
      this.url += `&orderStatus=${statusValue}`;
      console.log(statusValue);
    }

    if (this.fromDate) {
      this.url += `&startDate=${encodeURIComponent(this.fromDate)}`;
    }

    if (this.toDate) {
      this.url += `&endDate=${encodeURIComponent(this.toDate)}`;
    }

    this.loadOrders();
    this.url = this.baseURL+'Orderpage';
  }

  resetFilters(){
    this.selectedEntries = 5;
    this.selectedStatus = '';
    this.fromDate = null;
    this.toDate = null;
    this.url = `${this.baseURL}Orderpage?pageNumber=1&pageSize=${this.selectedEntries}`;
    this.loadOrders();
  }

  previousPage() {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.url += `?pageNumber=${this.currentPage}&pageSize=${this.selectedEntries}`;
      this.loadOrders();
      this.url = this.baseURL+'Orderpage';
    }
  }

  nextPage() {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.url = this.baseURL+'Orderpage';
      this.url += `?pageNumber=${this.currentPage}&pageSize=${this.selectedEntries}`;
      this.loadOrders();
      this.url = this.baseURL+'Orderpage';
    }
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.url += `?pageNumber=${this.currentPage}&pageSize=${this.selectedEntries}`;
      this.loadOrders();
      this.url = this.baseURL+'Orderpage';
    }
  }

  updateTable() {
    this.filteredData = this.data || [];
    this.pagedData = this.filteredData;
  }

  trackById(index: number, item: IDisplayOrder): number {
    return item.id;
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  formatPrice(price: number): string {
    return (
      this.currencyPipe.transform(price ?? 0, 'LE', 'symbol', '1.2-2') ?? ''
    );
  }
}
