import { OrderStatus } from "../../admin/Enums/OrderStatus";

export interface IDeliveredOrderUpdate{
    id: number,
    orderStatus: OrderStatus,
    orderMoneyReceived: number | undefined,
    shippingMoneyReceived: number | undefined,
    notes: string | undefined
}