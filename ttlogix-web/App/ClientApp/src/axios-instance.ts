import { notify } from "@kyvg/vue3-notification";
import axios, { AxiosInstance, AxiosResponse } from 'axios';
import { Error } from "./store/commonModels/errors";
import qs from 'qs'
import app_version from './app_version.js'

const createAxios = (baseURL: string, headers: string) => {
    const newInstance = axios.create({
        baseURL: baseURL,
        headers: {
            'Accept': headers,
            'AppVersion': app_version
        }
    });

    newInstance.interceptors.response.use((response) => response,
        ( 
            (err) => {
                // TODO: consider logging all types of errors like so (line below) to have complete list and come up with a proper handling.
                
                if(err.response.data.status === 401){
                    notify({
                        title: 'You\'re either logged out or not authorized to perform this action',
                        type: 'error'
                    });
                } else if(err.response.data.status === 404){
                    notify({
                        title: 'Error '+ err.response.data.status +'. Server was unable to perform this action.',
                        type: 'error'
                    });
                } else {
                    return Promise.reject(err);
                }
            }
        )
    );
    return newInstance;
 }

const $axios:AxiosInstance = createAxios('api','application/json');
$axios.defaults.paramsSerializer = (params) => {
    return qs.stringify(params, {
        arrayFormat: "repeat",
        encode: true,
        allowDots: true,
        skipNulls: true
    });
}
export default { $axios };