import { OrderStatus } from '../../Enums/OrderStatus';

export interface IDisplayProduct {
  id: string;
  name: string;
  weight: number;
  price: number;
  quantity: number;
  clientName: string;
  statusNote?: string;
  status: OrderStatus;
}
