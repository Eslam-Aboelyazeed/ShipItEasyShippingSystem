import { Injectable } from '@angular/core';
import { GenericService } from './generic.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { IAddOrder } from '../DTOs/InsertDTOs/IAddOrder';
import { IUpdateOrder } from '../DTOs/UpdateDTOs/IUpdateOrder';
import { IDisplayOrder } from '../DTOs/DisplayDTOs/IDisplayOrder';
import { map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class OrderService extends GenericService<
  IDisplayOrder,
  IAddOrder,
  IUpdateOrder
> {
  private apiUrl = 'http://localhost:5241/api/Orders';
  constructor(public http: HttpClient) {
    super(http);
    this.baseUrl = 'Orders';
  }
  getOrderStatuses(): Observable<any> {
    return this.http.get<IDisplayOrder[]>(this.apiUrl
    
    
    
    
  ).pipe(
      map(orders => {
        const statuses = {
          New: 0,
          Pending: 0,
          RepresentativeDelivered: 0,
          Delivered: 0,
          Unreachable: 0,
          Delayed: 0,
          PartiallyDelivered: 0,
          Cancelled: 0,
          RejectedWithPayment: 0,
          RejectedWithPartiallyPayment: 0,
          RejectedWithoutPayment: 0
        };
        console.log(orders);
        orders.forEach(order => {
          switch (order.status) {
            case 0:
              statuses.New++;
              break;
            case 1:
              statuses.Pending++;
              break;
            case 2:
              statuses.RepresentativeDelivered++;
              break;
            case 3:
              statuses.Delivered++;
              break;
            case 4:
              statuses.Unreachable++;
              break;
            case 5:
              statuses.Delayed++;
              break;
            case 6:
              statuses.PartiallyDelivered++;
              break;
            case 7:
              statuses.Cancelled++;
              break;
            case 8:
              statuses.RejectedWithPayment++;
              break;
            case 9:
              statuses.RejectedWithPartiallyPayment++;
              break;
            case 10:
              statuses.RejectedWithoutPayment++;
              break;
            default:
              break;
          }
        });

        return statuses;
      })
    );
  }
}
