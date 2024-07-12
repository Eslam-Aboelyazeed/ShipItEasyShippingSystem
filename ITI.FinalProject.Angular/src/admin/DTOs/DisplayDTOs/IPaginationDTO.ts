export interface IPaginationDTO<T extends object>  {
    TotalCount:number,
    TotalPages:number,
    List:T[]
}