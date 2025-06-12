import { Range } from "../commonModels/Range";
import { toRaw } from 'vue';

// Filtering
export function applyFilters<T>(column: string, value: any, filters: T){ 
    switch ((typeof value).toString()){
        case 'string' || 'number': {
            let keyString = column as keyof T;
            //@ts-ignore
            filters[keyString] = value;
            //@ts-ignore
            if (filters.pagination) filters.pagination.pageNumber = 1;
            break;
        }
        case 'object': {
            let keyRange = column as keyof Range
            //@ts-ignore
            filters[keyRange] = {from: value.from, to: value.to};
            //@ts-ignore
            if (filters.pagination) filters.pagination.pageNumber = 1;
            break; 
        }
    }
}

export function applyFiltersLocally<T>(arrayToFilter: Array<T>, column: string, value: any, filters: T) {
    var result: Array<any> = new Array<any>();
    switch ((typeof value).toString()) {
        case 'string' || 'number': {
            const _filters = Object.fromEntries(Object.entries(JSON.parse(JSON.stringify(toRaw(filters)))).filter(([_, v]) => v != null && v != ''));
            //@ts-ignore
            result = arrayToFilter.filter(item => Object.keys(_filters).every(key => item[key].includes(_filters[key])))
            break;
        }
        case 'object': {
            // TODO
            break;
        }
    }
    return result;
}