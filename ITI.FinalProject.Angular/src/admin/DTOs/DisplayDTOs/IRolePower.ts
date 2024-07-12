import { PowerTypes } from '../../Enums/PowerTypes';

export interface IRolePower {
  roleId: string;
  roleName: string;
  power: PowerTypes;
  timeOfAddtion: Date;
}
