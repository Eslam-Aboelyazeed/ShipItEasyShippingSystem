import { Status } from '../../Enums/Status';

export interface IAddBranch {
  name: string;
  status: Status;
  addingDate: Date;
  cityId: number;
}
