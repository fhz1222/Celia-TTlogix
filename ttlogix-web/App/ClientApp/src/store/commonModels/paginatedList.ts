import { PaginationData } from "@/store/commonModels/paginationData";

export class PaginatedList<T> {
    items: Array<T>;
    pagination: PaginationData;

    // extension for Storege Detail
    totalQty: number;

    /*static fromDto<T>(dto:any):PaginatedList<T>{
        const pl:PaginatedList<T> = dto;
        pl.pagination = PaginationData.fromDto(dto.pagination);
        return pl;
    }*/
}