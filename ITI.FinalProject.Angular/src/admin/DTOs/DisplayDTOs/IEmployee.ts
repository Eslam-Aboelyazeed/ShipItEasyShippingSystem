import { Status } from "../../Enums/Status";

export interface IEmployee{

    id:string;
    fullName:string;
    address:string;
    phoneNumber:string;
    userName:string;
    email:string;
    passwordHash:string;
    branch:string;
    role:string;
    status:Status
}
