import { Status } from "../../Enums/Status";

export interface IBranchUpdate{

  id:number,
  name:string,
  status:Status,
  addingDate: Date,
  cityId: number,

}
