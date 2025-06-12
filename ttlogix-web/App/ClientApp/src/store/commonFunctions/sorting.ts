import { Sorting } from "../commonModels/Sorting";

// Sorting
export function sort(by: string, sorting:Sorting){ 
    sorting.by = by;
    sorting.descending = sorting.descending != null ? !sorting.descending : false;
}

// Local sorting (without reloading data from the server)
export function sortLocally(sorting:Sorting, arrayToSort:Array<any>){
    const by:string = sorting.by ?? '';
    const result = arrayToSort.sort((a, b) => {
        const d = sorting.descending ? 1 : -1;
        if (typeof (a[by]) === 'string' || a[by] instanceof String) {
            return a[by].localeCompare(b[by]) * (-d);
        } else {
            return ((a[by] < b[by]) ? d : ((a[by] > b[by]) ? (-d) : 0));
        }
    });
    return result;
}

