import { Status } from "../../Enums/Status";

export interface ICityInsert{

  name:string,
  status: Status,
  normalShippingCost: number,
  pickupShippingCost: number,
  governorateId: number

}
