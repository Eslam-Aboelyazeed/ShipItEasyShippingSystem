import { ShippingTypes } from '../../Enums/ShippingTypes';

export interface IDisplayShipping {
  id: number;
  price: number;
  shippingType: ShippingTypes;
}
