import { Status } from "../../Enums/Status";

export interface IBranch{

  id:number,
  name:string,
  status:Status,
  addingDate: Date,
  cityId: number,

}
