import { Status } from '../../Enums/Status';

export interface IDisplayBranch {
  id: number;
  name: string;
  status: Status;
  addingDate: Date;
  cityId: number;
}
