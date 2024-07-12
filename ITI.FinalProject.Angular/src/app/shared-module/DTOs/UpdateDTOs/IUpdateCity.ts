import { Status } from '../../Enums/Status';

export interface IUpdateCity {
  id: number;
  name: string;
  status: Status;
  normalShippingCost: number;
  pickupShippingCost: number;
  governorateId: number;
}
