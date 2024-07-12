import { OrderStatus } from '../../Enums/OrderStatus';
import { OrderTypes } from '../../Enums/OrderTypes';
import { PaymentTypes } from '../../Enums/PaymentTypes';
import { ShippingTypes } from '../../Enums/ShippingTypes';
import { IAddProduct } from './IAddProduct';

export interface IAddOrder {
  clientName: string;
  phone: string;
  phone2?: string;
  email?: string;
  notes?: string;
  villageAndStreet: string;
  shippingToVillage: boolean;
  orderMoneyReceived?: number;
  shippingMoneyReceived?: number;
  merchantId: string;
  governorateId: number;
  cityId: number;
  shippingId: number;
  branchId: number;
  RepresentativeId?: string | undefined;
  status: OrderStatus;
  type: OrderTypes;
  paymentType: PaymentTypes;
  shippingType: ShippingTypes;
  products: IAddProduct[];
}
