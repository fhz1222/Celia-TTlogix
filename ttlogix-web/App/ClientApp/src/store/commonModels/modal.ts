import { Dictionary } from "../commonFunctions/miscellaneous";

export class Modal {
    constructor(width: number | string | null = null, title: string | null = null, closeable: boolean = true){
        this.width = width ? width : 500;
        this.title = title ? title : '';
        this.customProps = null;
        this.on = false;
        this.closeonx = closeable;
        this.closeonesc = closeable;
        this.closeonclick = closeable;
        this.showfooter = true;
        this.fnMod = {on: false, type: null, message: null, callback: null};
    }

    width: number | string;
    title: string;
    customProps: any;
    on: boolean;
    closeonx: boolean;
    closeonesc: boolean;
    closeonclick: boolean;
    showfooter: boolean;
    fnMod: ErrorConfirm;
} 

class ErrorConfirm{
    on: boolean;
    type: string | null;
    message: string | null;
    callback: (() => Promise<void>) | null;
}