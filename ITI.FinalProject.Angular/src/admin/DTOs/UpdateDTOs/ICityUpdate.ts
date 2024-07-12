import { Status } from "../../Enums/Status";

export interface ICityUpdate{

  id: number,
  name:string,
  status: Status,
  normalShippingCost: number,
  pickupShippingCost: number,
  governorateId: number

}
