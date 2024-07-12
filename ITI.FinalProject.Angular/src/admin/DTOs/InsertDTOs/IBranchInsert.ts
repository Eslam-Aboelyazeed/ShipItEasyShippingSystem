import { Status } from "../../Enums/Status";

export interface IBranchInsert{

  name:string,
  status:Status,
  addingDate: Date,
  cityId: number,

}
