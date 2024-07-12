import { ShippingTypes } from '../../Enums/ShippingTypes';

export interface IUpdateShipping {
  id: number;
  price: number;
  shippingType: ShippingTypes;
}
