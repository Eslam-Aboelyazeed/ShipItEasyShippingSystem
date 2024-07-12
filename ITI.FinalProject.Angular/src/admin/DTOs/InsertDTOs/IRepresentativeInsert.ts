import { DiscountType } from "../../Enums/DiscountType";
import { Status } from "../../Enums/Status";

export interface IRepresentativeInsert{

    userFullName:string,
    email: string,
    password:string,
    userBranchId:number,
    governorateIds:number[],
    userPhoneNo:string,
    userAddress:string,
    discountType:DiscountType,
    companyPercentage:number,
    userStatus:Status,
}
