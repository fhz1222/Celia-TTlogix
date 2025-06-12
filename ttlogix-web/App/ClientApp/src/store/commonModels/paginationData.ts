export class PaginationData {
    pageNumber: number;
    totalPages: number;
    totalCount: number;
    itemsPerPage: number;
    hasPreviousPage: boolean;
    hasNextPage: boolean;

    /*static fromDto(dto:any):PaginationData {
        const pd:PaginationData = dto;
        pd.itemsPerPage = dto.pageSize;
        return pd;
    }*/
}