import { PaginationData } from "../commonModels/paginationData";

// Paging 
export function goToPage(page: number, pagination: PaginationData){ 
    pagination.pageNumber = page;
}
export function setItemsPerPage(itemsPerPage: number, pagination: PaginationData){ 
    pagination.itemsPerPage = itemsPerPage;
    pagination.pageNumber = 1;
}