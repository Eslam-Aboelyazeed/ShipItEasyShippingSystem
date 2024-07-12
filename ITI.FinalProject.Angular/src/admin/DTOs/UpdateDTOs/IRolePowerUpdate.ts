import { PowerTypes } from "../../Enums/PowerTypes";
import { IPowers } from "../DisplayDTOs/IPowers";

export interface IRolePowerUpdate {
    roleId:string,
    roleName:string,
    powers: IPowers[]
}