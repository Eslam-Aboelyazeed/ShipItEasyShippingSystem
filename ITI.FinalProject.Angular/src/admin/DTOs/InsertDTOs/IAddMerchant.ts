import { Status } from '../../Enums/Status';
import { IAddSpecialPackage } from './IAddSpecialPackage';

export interface IAddMerchant {
  storeName?: string;
  userName: string;
  passwordHash: string;
  email: string;
  address: string;
  phoneNumber: string;
  merchantPayingPercentageForRejectedOrders: number;
  specialPickupShippingCost?: number;
  status: Status;
  cityID: number;
  governorateID: number;
  branchId: number;
  specialPackages: IAddSpecialPackage[];
}
