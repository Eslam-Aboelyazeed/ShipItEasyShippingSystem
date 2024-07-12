import { OrderStatus } from '../../Enums/OrderStatus';
export interface IAddProduct {
  name: string;
  weight: number;
  quantity: number;
  price: number;
  statusNote?: string;
  orderId: number;
  ProductStatus: OrderStatus;
}
