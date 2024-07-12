import { ShippingTypes } from '../../Enums/ShippingTypes';

export interface IAddShipping {
  price: number;
  shippingType: ShippingTypes;
}
