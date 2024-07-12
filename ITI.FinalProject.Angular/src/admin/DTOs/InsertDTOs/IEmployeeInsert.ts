import { Status } from "../../Enums/Status";

export interface IEmployeeInsert{
    
    fullName:string;
    address:string;
    phoneNumber:string;
    userName:string;
    email:string;
    passwordHash:string;
    branchId:number;
    role:string;
    status:Status
}
