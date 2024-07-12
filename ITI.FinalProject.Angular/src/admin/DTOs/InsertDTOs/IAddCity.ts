import { Status } from "../../Enums/Status";

export interface IAddCity{
    name:string;
    status:Status;
    normalShippingCost:number;
    pickupShippingCost:number;
    governorateId:number;
}