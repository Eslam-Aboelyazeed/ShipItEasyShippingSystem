import { OrderStatus } from '../../Enums/OrderStatus';
import { OrderTypes } from '../../Enums/OrderTypes';
import { PaymentTypes } from '../../Enums/PaymentTypes';
import { IDisplayProduct } from './IProduct';

export interface IDisplayOrder {
  id: number;
  clientName: string;
  date: Date;
  phone: string;
  phone2?: string;
  email?: string;
  notes?: string;
  totalPrice: number;
  totalWeight: number;
  villageAndStreet: string;
  shippingToVillage: boolean;
  merchantName: string;
  governorateName: string;
  cityName: string;
  branchName: string;
  shippingType: string;
  representativeName: string;
  orderMoneyReceived?: number;
  shippingMoneyReceived?: number;
  shippingCost: number;
  status: OrderStatus;
  type: OrderTypes;
  companyProfit:number | undefined;
  paymentType: PaymentTypes;
  products: IDisplayProduct[];
}
