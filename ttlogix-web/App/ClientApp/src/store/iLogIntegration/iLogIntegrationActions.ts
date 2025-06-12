// import vue stuff
import { AxiosInstance } from 'axios'
import axios from '@/axios-instance'

// import types
import { CsvFile } from './models/csvFile'

let apiAxios: AxiosInstance = axios.$axios;

export async function isEnabled(): Promise<boolean> {
    return (await apiAxios.get('new-api/iLogIntegration/isActiveForWarehouse')).data;
}

export async function enable(): Promise<void> {
    let enable = await apiAxios.post('new-api/iLogIntegration/enable');
}

export async function disable(): Promise<void> {
    let disable = await apiAxios.post('new-api/iLogIntegration/disable');
}

export async function getStockFile(): Promise<CsvFile> {
    let file = await apiAxios.get('new-api/StorageDetail/getILogStockSynchronizationCSV');
    const headerLine = file.headers['Content-Disposition'] || file.headers['content-disposition'];
    const regExpFilename = /filename="?(?<filename>[^;"]*)/;
    let csvName = regExpFilename.exec(headerLine)?.groups?.filename ?? null;
    let result: CsvFile = { csv: file.data, name: csvName ? csvName : 'stock_' + (new Date()).toString() + '.csv' };
    return result;
}
