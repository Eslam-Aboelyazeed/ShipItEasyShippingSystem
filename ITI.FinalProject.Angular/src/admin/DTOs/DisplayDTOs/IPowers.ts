import { Tables } from "../../Enums/Tables";

export interface IPowers {
    tableName:Tables,
    create:boolean,
    delete:boolean,
    update:boolean,
    read:boolean
}