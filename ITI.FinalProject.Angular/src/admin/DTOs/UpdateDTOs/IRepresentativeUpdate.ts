import { DiscountType } from "../../Enums/DiscountType";
import { Status } from "../../Enums/Status";

export interface IRepresentativeUpdate{
    id:string,
    userFullName:string,
    email: string,
    userBranchId:number,
    governorateIds:number[],
    userPhoneNo:string,
    userAddress:string,
    discountType:DiscountType,
    companyPercentage:number,
    userStatus:Status,
    oldPassword:string | undefined,
    newPassword:string | undefined
}
