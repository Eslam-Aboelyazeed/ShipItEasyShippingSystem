import { OrderStatus } from "../../Enums/OrderStatus"

export interface IOrderNewUpdate{
    id:number
    orderStatus:OrderStatus 
    representativeId:string|null
}