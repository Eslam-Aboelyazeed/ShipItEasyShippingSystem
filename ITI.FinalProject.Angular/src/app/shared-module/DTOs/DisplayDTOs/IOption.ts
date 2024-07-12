export interface IOption{
    id: number | string,
    name: string,
    dependentId: number | undefined,
    dependentIds:number[] | undefined
}