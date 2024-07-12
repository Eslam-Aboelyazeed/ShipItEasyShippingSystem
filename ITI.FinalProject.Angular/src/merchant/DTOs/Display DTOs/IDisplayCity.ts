import { Status } from '../../Enums/Status';

export interface IDisplayCity {
  id: number;
  name: string;
  status: Status;
  normalShippingCost: number;
  pickupShippingCost: number;
  governorateId: number;
}
