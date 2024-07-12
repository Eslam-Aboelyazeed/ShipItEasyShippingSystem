import { OrderStatus } from '../../Enums/OrderStatus';
import { OrderTypes } from '../../Enums/OrderTypes';
import { PaymentTypes } from '../../Enums/PaymentTypes';
import { ShippingTypes } from '../../Enums/ShippingTypes';
import { IDisplayProduct } from './IDisplayProduct';

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
  representativeName: string;
  orderMoneyReceived?: number;
  shippingMoneyReceived?: number;
  shippingCost: number;
  status: OrderStatus;
  type: OrderTypes;
  paymentType: PaymentTypes;
  companyProfit:number | undefined;
  products: IDisplayProduct[];
  shippingType: ShippingTypes;
}
