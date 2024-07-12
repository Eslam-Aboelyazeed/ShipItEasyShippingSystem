import { Status } from '../../Enums/Status';

export interface IUpdateBranch {
  id: number;
  name: string;
  status: Status;
  addingDate: Date;
  cityId: number;
}
