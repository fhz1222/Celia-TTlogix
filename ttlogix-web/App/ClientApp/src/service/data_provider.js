import EventBus from './bus.js'
import { saveData } from './file_service.js'
import storage from './session_storage.js'
import { notify } from "@kyvg/vue3-notification";

class DataProvider {
    apiAxios = null
    constructor(app, options) {
        this.apiAxios = options.axios
        this.i18n = options.i18n.global
        const self = this
        this.apiAxios.defaults.headers['Content-Type'] = 'application/json'
        this.apiAxios.defaults.headers['Accept'] = 'application/json'
        this.apiAxios.interceptors.request.use(function (config) {
            EventBus.$emit('load', true)
            return config
        }, function (error) {
            EventBus.$emit('load', false)
            return Promise.reject(error)
        })

        const isJson = (str) => {
            try {
                JSON.parse(str);
            } catch (e) {
                return false;
            }
            return true;
        }
        this.apiAxios.interceptors.response.use(function (response) {
            EventBus.$emit('load', false)
            return response
        }, function (err) {
            EventBus.$emit('load', false)
            if (err.response && err.response.data == 'UnsupportedAppVersion'){
                if (confirm("New version available. Do you want to refresh the app?")){
                    window.location.reload();
                }
            }
            else if (!err.response || (err.response.status != 400 && err.response.status != 401)) {
                const jsonError = !isJson(err) ? JSON.stringify(err) : err;
                let eObj = JSON.parse(jsonError);
                const isApiErr = err.response.data.title;
                if (isApiErr) eObj = err.response.data;
                if (!eObj.MessageKey && !eObj.title) {
                    notify({
                        title: self.i18n.t('error.connection'),
                        type: 'error',
                        group: 'permanent'
                    })
                } else {
                    if (eObj.title) {
                        notify({
                            title: eObj.type ? eObj.type : eObj.title,
                            text: eObj.title,
                            type: 'error',
                            group: 'permanent'
                        })
                    }
                }
            } else if (err.response && err.response.status == 401) {
                app.$auth.logout({
                    redirect: '/login'
                });
            }
            return Promise.reject(err)
        })

        this.setAccpetLang = (lang) => {
            self.apiAxios.defaults.headers['Accept-Language'] = lang
        }

        this.unifyErrors = (err) => {
            if (err.response && err.response.data) {
                return err.response.data;
            } else {
                return err
            }
        }

        this.notifyUnknown = () => {
            notify({
                title: self.i18n.t('error.ErrorOccured'),
                text: self.i18n.t('error.UnknownError'),
                type: 'error',
                group: 'permanent'
            })
        }

        this.errorHandlers = {
            simple(err, errorParams = {}) {
                const innErr = self.unifyErrors(err);
                if (Array.isArray(innErr)) {
                    innErr.forEach(eStr => {
                        let eObj = JSON.parse(eStr);
                        notify({
                            title: self.i18n.t('error.ErrorOccured'),
                            text: self.i18n.te(`error.${eObj.MessageKey}`) ? self.i18n.t(`error.${eObj.MessageKey}`, {...eObj.Arguments, ...errorParams}) : eObj.MessageKey,
                            type: 'error',
                            group: 'permanent'
                        })
                    })
                } else {
                    self.notifyUnknown();
                }
            },
            reports(err) {
                const innErr = self.unifyErrors(err);
                innErr.text().then((r) => {
                    let eObj = JSON.parse(r)
                    if ('MessageKey' in eObj) {
                        notify({
                            title: self.i18n.t('error.ErrorOccured'),
                            text: self.i18n.t(`error.${eObj.MessageKey}`, eObj.Arguments),
                            type: 'error',
                            group: 'permanent'
                        })
                    } else {
                        self.notifyUnknown();
                    }
                });
            },
            form(err) {
                if (err.response && err.response.data) {
                    if (Array.isArray(err.response.data)) {
                        self.errorHandlers.simple(err)
                    }
                    else {
                        notify({
                            title: self.i18n.t('error.ErrorOccured'),
                            text: self.i18n.t('error.form'),
                            type: 'error',
                            group: 'permanent'
                        })
                    }
                } else if (err) {
                    let eObj = JSON.parse(err)
                    if ('MessageKey' in eObj) {
                        notify({
                            title: self.i18n.t('error.ErrorOccured'),
                            text: self.i18n.t(`error.${eObj.MessageKey}`, eObj.Arguments),
                            type: 'error',
                            group: 'permanent'
                        })
                    } else {
                        self.notifyUnknown();
                    }
                }
            }
        }

        const settingsUrl = 'settings'
        this.settings = {
            async get() {
                try {
                    return (await self.apiAxios.get(`${settingsUrl}`)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            }
        } 

        const loadingsUrl = 'loadings'
        this.loadings = {
            async getLoadings(filter) {
                try {
                    return (await self.apiAxios.get(`${loadingsUrl}/getLoadings`, { params: filter })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getLoading(jobNo) {
                try {
                    return (await self.apiAxios.get(`${loadingsUrl}/getLoading`, { params: { jobNo: jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getLoadingDetails(jobNo) {
                try {
                    return (await self.apiAxios.get(`${loadingsUrl}/getLoadingDetails`, { params: { jobNo: jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getLoadingEntryList(customerCode) {
                try {
                    return (await self.apiAxios.get(`${loadingsUrl}/getLoadingEntryList`, { params: { customerCode: customerCode } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async postLoading(loadingAddDto) {
                try {
                    return (await self.apiAxios.post(`${loadingsUrl}`, loadingAddDto)).data
                }
                catch (err) {
                    self.errorHandlers.form(err)
                    throw err
                }
            },
            async batchCreateLoadingDetails(jobNo, orderNos) {
                try {
                    return (await self.apiAxios.post(`${loadingsUrl}/batchCreateLoadingDetails`, orderNos, { params: { jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async cancelLoading(jobNo) {
                try {
                    return (await self.apiAxios.post(`${loadingsUrl}/cancelLoading`, null, { params: { jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getBondedStockJobNosWithoutCommInv(jobNo) {
                try {
                    return (await self.apiAxios.get(`${loadingsUrl}/getBondedStockJobNosWithoutCommInv`, { params: { jobNo: jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async confirmLoading(jobNo) {
                try {
                    return (await self.apiAxios.post(`${loadingsUrl}/confirmLoading`, null, { params: { jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async truckArrival(jobNo) {
                try {
                    return (await self.apiAxios.post(`${loadingsUrl}/truckArrival`, null, { params: { jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async truckDeparture(jobNo) {
                try {
                    return (await self.apiAxios.post(`${loadingsUrl}/truckDeparture`, null, { params: { jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async setAllowForDispatch(jobNo, allowedForDispatch) {
                try {
                    return (await self.apiAxios.post(`${loadingsUrl}/setAllowForDispatch`, null, { params: { jobNo, allowedForDispatch } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async patchLoading(jobNo, loadingDto) {
                try {
                    return (await self.apiAxios.patch(`${loadingsUrl}/${jobNo}`, loadingDto)).data
                }
                catch (err) {
                    self.errorHandlers.form(err)
                    throw err
                }
            },
            async deleteLoadingDetails(jobNo, orderNos) {
                try {
                    return (await self.apiAxios.post(`${loadingsUrl}/deleteLoadingDetails`, {
                        jobNo: jobNo,
                        orderNos: orderNos
                    })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async addLoadingFromOutbound(loading, outJobNos) {
                try {
                    return (await self.apiAxios.post(`${loadingsUrl}/addLoadingFromOutbound`, {
                        loading: loading,
                        outJobNos: outJobNos
                    })).data
                }
                catch (err) {
                    self.errorHandlers.form(err)
                    throw err
                }
            },
        }
        const outboundsUrl = 'outbounds'
        this.outbounds = {
            async getOutbounds(filter) {
                try {
                    return (await self.apiAxios.get(`${outboundsUrl}/getOutbounds`, { params: filter })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getOutbound(jobNo) {
                try {
                    return (await self.apiAxios.get(`${outboundsUrl}/getOutbound`, { params: { jobNo: jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getOutboundDetails(jobNo) {
                try {
                    return (await self.apiAxios.get(`${outboundsUrl}/getOutboundDetails`, { params: { jobNo: jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async patchOutbound(jobNo, outboundDto) {
                try {
                    return (await self.apiAxios.patch(`${outboundsUrl}/${jobNo}`, outboundDto)).data
                }
                catch (err) {
                    self.errorHandlers.form(err)
                    throw err
                }
            },
            async getOutboundDetailsWithReceivedQty(jobNo) {
                try {
                    return (await self.apiAxios.get(`${outboundsUrl}/getOutboundDetailsWithReceivedQty`, { params: { jobNo: jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getOutboundPickableList(filter) {
                try {
                    return (await self.apiAxios.get(`${outboundsUrl}/getOutboundPickableList`, { params: filter })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async importEKanbanEUCPart(orderNo, factoryId, errorParams={}) {
                try {
                    return (await self.apiAxios.post(`${outboundsUrl}/importEKanbanEUCPart`, null, { params: { orderNo: orderNo, factoryId: factoryId } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err, errorParams)
                    throw err
                }
            },
            async createOutboundManual(outboundManualDto) {
                try {
                    return (await self.apiAxios.post(`${outboundsUrl}/createOutboundManual`, outboundManualDto)).data
                }
                catch (err) {
                    self.errorHandlers.form(err)
                    throw err
                }
            },
            async updateOutboundStatus(jobNo) {
                try {
                    return (await self.apiAxios.post(`${outboundsUrl}/updateOutboundStatus?jobNo=` + jobNo)).data
                }
                catch (err) {
                    self.errorHandlers.form(err)
                    throw err
                }
            },
            async cancelOutbound(jobNo) {
                try {
                    return (await self.apiAxios.post(`${outboundsUrl}/cancelOutbound`, null, { params: { jobNo: jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async complete(type, jobNo) {
                try {
                    return (await self.apiAxios.post(`${outboundsUrl}/complete${type}`, null, { params: { jobNo: jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async truckDeparture(jobNo) {
                try {
                    return (await self.apiAxios.post(`${outboundsUrl}/dispatchWarehouseTransfer`, null, { params: { jobNo: jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async cargoInTransit(jobNos) {
                try {
                    return (await self.apiAxios.post(`${outboundsUrl}/cargoInTransit`, { jobNos: jobNos })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async cancelAllocation(cancelAllocationDto) {
                try {
                    return (await self.apiAxios.post(`${outboundsUrl}/cancelAllocation`, cancelAllocationDto)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async splitOutbound(splitOutboundDto) {
                try {
                    return (await self.apiAxios.post(`${outboundsUrl}/splitOutbound`, splitOutboundDto )).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async splitOutboundByInboundJobNo(jobNo) {
                try {
                    return (await self.apiAxios.post(`${outboundsUrl}/splitOutboundByInboundJobNo`, null, { params: { jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async splitOutboundByOwnership(jobNo) {
                try {
                    return (await self.apiAxios.post(`${outboundsUrl}/splitOutboundByOwnership`, null, { params: { jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async releaseBondedStock(jobNo, outbound) {
                try {
                    return (await self.apiAxios.post(`${outboundsUrl}/releaseBondedStock`, outbound, { params: { jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getOutboundQRCodeImage(jobNo) {
                try {
                    return (await self.apiAxios.get(`${outboundsUrl}/getOutboundQRCodeImage`, { params: { jobNo: jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async addNewOutboundDetail(outboundDetailAddDto) {
                try {
                    return (await self.apiAxios.post(`${outboundsUrl}/addNewOutboundDetail`, outboundDetailAddDto)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async deleteOutboundDetail(jobNo, lineItem) {
                
                try {
                    return (await self.apiAxios.delete(`${outboundsUrl}/outboundDetail?jobNo=${jobNo}&lineItem=${lineItem}`)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async downloadEDTToCSV(jobNo) {
                try {
                    var resp = (await self.apiAxios.get(`${outboundsUrl}/downloadEDTToCSV`, {
                        params: {
                            jobNo: jobNo
                        },
                        responseType: 'blob',
                        headers: {
                            Accept: 'text/plain'
                        }
                    }))
                    let headerLine = resp.headers['Content-Disposition'] || resp.headers['content-disposition'];
                    const regExpFilename = /filename="?(?<filename>[^;"]*)/;
                    let fileName = regExpFilename.exec(headerLine)?.groups?.filename ?? null;
                    saveData(resp.data, fileName, "text/plain")
                }
                catch (err) {
                    self.errorHandlers.reports(err)
                    throw err
                }
            },
            async undoPicking(undoPickEntryDto) {
                try {
                    return (await self.apiAxios.post(`${outboundsUrl}/undoPicking`, undoPickEntryDto)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getAllowedOutboundCreationMethods() {
                try {
                    return (await self.apiAxios.get(`${outboundsUrl}/allowedOutboundCreationMethods`)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getOrderSummary(jobNo) {
                try {
                    return (await self.apiAxios.get(`${outboundsUrl}/getOrderSummary`, { params: { jobNo: jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            }
        }
        const deliveryUrl = 'Delivery'
        this.delivery = {
            async getDeliveryCustomerClients(customerCode) {
                try {
                    return (await self.apiAxios.get(`new-api/${deliveryUrl}/getDeliveryCustomerClients?customerCode=${customerCode}`)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            }
        }
        const invoicingUrl = 'InvoiceRequest'
        this.invoicing = {
            async getStatus(jobNo) {
                try {
                    return (await self.apiAxios.get(`new-api/${invoicingUrl}/status/${jobNo}`)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async block(jobNo) {
                try {
                    return (await self.apiAxios.post(`new-api/${invoicingUrl}/block/${jobNo}`)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async unblock(jobNo) {
                try {
                    return (await self.apiAxios.post(`new-api/${invoicingUrl}/unblock/${jobNo}`)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async requestNow(jobNo) {
                try {
                    return (await self.apiAxios.post(`new-api/${invoicingUrl}/requestNow/${jobNo}`)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async isActive() {
                try {
                    return (await self.apiAxios.get(`new-api/${invoicingUrl}/isActive`)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            
        }
        const inboundsUrl = 'inbounds'
        const inboundsApiUrl = 'inbound'
        this.inbounds = {
            async importASN(asnNo) {
                try {
                    return (await self.apiAxios.post(`${inboundsUrl}/importASN`, null, { params: { asnNo: asnNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async importFile(data, progressCallback) {
                try {
                    let formData = new FormData()
                    Object.keys(data).forEach(k => {
                        formData.append(k, data[k]);
                    })
                    

                    return (await self.apiAxios.post(`${inboundsUrl}/importFile`, formData, {
                        headers: {
                            'Content-Type':'multipart/form-data'
                        },
                        onUploadProgress: progressCallback
                    })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async createInboundManual(inboundManualDto) {
                try {
                    return (await self.apiAxios.post(`${inboundsUrl}/createInboundManual`, inboundManualDto)).data
                }
                catch (err) {
                    self.errorHandlers.form(err)
                    throw err
                }
            },
            async getInbounds(filter) {
                try {
                    return (await self.apiAxios.get(`${inboundsUrl}/getInbounds`, { params: filter })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            
            async getASNsToImport(filter) {
                try {
                    return (await self.apiAxios.get(`${inboundsUrl}/getASNsToImport`, { params: filter })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getASNDetails(asnNo) {
                try {
                    return (await self.apiAxios.get(`${inboundsUrl}/getASNDetails`, { params: { asnNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getInbound(jobNo) {
                try {
                    return (await self.apiAxios.get(`${inboundsUrl}/getInbound`, { params: { jobNo: jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getInboundDetails(jobNo) {
                try {
                    return (await self.apiAxios.get(`${inboundsUrl}/getInboundDetails`, { params: { jobNo: jobNo } })).data;
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async downloadIDTToCSV(jobNo) {
                try {
                    var resp = (await self.apiAxios.get(`${inboundsUrl}/getIDTAsCSV`, {
                        params: {
                            jobNo: jobNo
                        },
                        responseType: 'blob',
                        headers: {
                            Accept: 'text/plain'
                        }
                    }))
                    let headerLine = resp.headers['Content-Disposition'] || resp.headers['content-disposition'];
                    const regExpFilename = /filename="?(?<filename>[^;"]*)/;
                    let fileName = regExpFilename.exec(headerLine)?.groups?.filename ?? null;
                    saveData(resp.data, fileName, "text/plain")
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async increasePkgQty(addPkgQtyDto) {
                try {
                    return (await self.apiAxios.post(`${inboundsUrl}/increasePkgQty`, addPkgQtyDto)).data
                }
                catch (err) {
                    self.errorHandlers.form(err)
                    throw err
                }
            },
            async createInboundDetail(inboundDetail) {
                try {
                    return (await self.apiAxios.post(`${inboundsUrl}/createInboundDetail`, inboundDetail)).data
                }
                catch (err) {
                    self.errorHandlers.form(err)
                    throw err
                }
            },
            async modifyInboundDetail(inboundDetail) {
                try {
                    return (await self.apiAxios.post(`${inboundsUrl}/modifyInboundDetail`, inboundDetail)).data
                }
                catch (err) {
                    self.errorHandlers.form(err)
                    throw err
                }
            },
            async removePIDs(removePIDsDto) {
                try {
                    return (await self.apiAxios.post(`${inboundsUrl}/removePIDs`, removePIDsDto)).data
                }
                catch (err) {
                    self.errorHandlers.form(err)
                    throw err
                }
            },
            async cancelInbound(jobNo) {
                try {
                    return (await self.apiAxios.post(`${inboundsUrl}/cancelInbound`, null, { params: { jobNo: jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.form(err)
                    throw err
                }
            },
            async patchInbound(jobNo, inboundDto) {
                try {
                    return (await self.apiAxios.patch(`${inboundsUrl}/${jobNo}`, inboundDto)).data
                }
                catch (err) {
                    self.errorHandlers.form(err)
                    throw err
                }
            },
            async getOutstandingInboundsXlsReport() {
                try {
                    var resp = (await self.apiAxios.get(`${inboundsUrl}/getOutstandingInboundsXlsReport`, {
                        responseType: 'blob',
                        headers: {
                            Accept: 'application/octet-stream'
                        }
                    }))
                    let headerLine = resp.headers['Content-Disposition'] || resp.headers['content-disposition'];
                    const regExpFilename = /filename="?(?<filename>[^;"]*)/;
                    let fileName = regExpFilename.exec(headerLine)?.groups?.filename ?? null;
                    saveData(resp.data, fileName, "application/octet-stream")
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async undoPutaway(pids) {
                try {
                    return (await self.apiAxios.post(`new-api/${inboundsApiUrl}/undoPutaway`, pids)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
        }
        const inboundReversalUrl = 'InboundReversal'
        this.inboundReversal = {
            async isActive() {
                try {
                    return (await self.apiAxios.get(`new-api/${inboundReversalUrl}/isActive`)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            }
        }

        const ekanbansUrl = 'ekanbans'
        this.ekanbans = {
            async getEKanbanListForEurope(filter) {
                try {
                    return (await self.apiAxios.get(`${ekanbansUrl}/getEKanbanListForEurope`, { params: filter })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async hasEKanban(jobNo) {
                try {
                    return (await self.apiAxios.get(`${ekanbansUrl}/hasEKanban`, { params: { jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getEKanbanPartsStatusByOwnershipEHP(orderNo) {
                try {
                    return (await self.apiAxios.get(`${ekanbansUrl}/getEKanbanPartsStatusByOwnershipEHP`, { params: { orderNo: orderNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getEKanbanPartsStatusForEKanbanCPart(orderNo) {
                try {
                    return (await self.apiAxios.get(`${ekanbansUrl}/getEKanbanPartsStatusForEKanbanCPart`, { params: { orderNo: orderNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getEKanbanPartsStatusByOwnership(orderNo) {
                try {
                    return (await self.apiAxios.get(`${ekanbansUrl}/getEKanbanPartsStatusByOwnership`, { params: { orderNo: orderNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getEKanbanPartsStatusForCPart(orderNo) {
                try {
                    return (await self.apiAxios.get(`${ekanbansUrl}/getEKanbanPartsStatusForCPart`, { params: { orderNo: orderNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getEKanbanPartsStatusForCPartWithoutExt(orderNo) {
                try {
                    return (await self.apiAxios.get(`${ekanbansUrl}/getEKanbanPartsStatusForCPartWithoutExt`, { params: { orderNo: orderNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getEKanbanDataToDownload(orderNo) {
                try {
                    return (await self.apiAxios.get(`${ekanbansUrl}/getEKanbanDataToDownload`, { params: { orderNumber: orderNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async checkEKanbanFulfillable(orderNos) {
                try {
                    return (await self.apiAxios.post(`${ekanbansUrl}/checkEKanbanFulfillable`, {
                        orderNos: orderNos
                    })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async EKanbanCheck(orderNos) {
                try {
                    return (await self.apiAxios.post(`${ekanbansUrl}/EKanbanCheck`, { orderNos: orderNos })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async cancelEKanbans(orderNos) {
                try {
                    return (await self.apiAxios.post(`${ekanbansUrl}/cancelEKanbans`, { orderNos: orderNos })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            }
        }
        const supplierMastersUrl = 'supplierMasters'
        this.supplierMasters = {
            async getList(factoryId) {
                try {
                    return (await self.apiAxios.get(`${supplierMastersUrl}/getList`, { params: { factoryId: factoryId } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            }
        }
        const currenciesUrl = 'currencies'
        this.currencies = {
            async getCurrencies() {
                try {
                    return (await self.apiAxios.get(`${currenciesUrl}`)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            }
        }
        const warehousesUrl = 'warehouses'
        this.warehouses = {
            async getWarehouses() {
                try {
                    return (await self.apiAxios.get(`${warehousesUrl}`)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            }
        }
        const countriesUrl = 'countries'
        this.countries = {
            async getCountries() {
                try {
                    return (await self.apiAxios.get(`${countriesUrl}`)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            }
        }
        const packageTypesUrl = 'packageTypes'
        this.packageTypes = {
            async getPackageTypes() {
                try {
                    return (await self.apiAxios.get(`${packageTypesUrl}`)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            }
        }
        const pickingListsUrl = 'pickingLists'
        this.pickingLists = {
            async getPickingListWithUOM(jobNo, lineItem) {
                try {
                    return (await self.apiAxios.get(`${pickingListsUrl}/getPickingListWithUOM`, { params: { jobNo: jobNo, lineItem: lineItem } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getPickingDataToDownload(filter) {
                try {
                    return (await self.apiAxios.get(`${pickingListsUrl}/getPickingDataToDownload`, { params: filter })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async allocate(pickingListDtos) {
                try {
                    return (await self.apiAxios.post(`${pickingListsUrl}/allocate`, pickingListDtos)).data
                }
                catch (err) {
                    self.errorHandlers.form(err)
                    throw err
                }
            },
            async autoAllocate(jobNo, lineItem, seqNo) {
                try {
                    return (await self.apiAxios.post(`${pickingListsUrl}/autoAllocate`, {
                        jobNo: jobNo,
                        lineItem: lineItem,
                        seqNo: seqNo
                    })).data 
                }
                catch (err) {
                    self.errorHandlers.form(err)
                    throw err
                }
            },
            async unAllocate(pickingListDtos) {
                try {
                    
                    return (await self.apiAxios.post(`${pickingListsUrl}/unAllocate`, pickingListDtos)).data
                }
                catch (err) {
                    self.errorHandlers.form(err)
                    throw err
                }
            },
        }
        const customersUrl = 'customers'
        this.customers = {
            async getCustomers() {
                try {
                    return (await self.apiAxios.get(`${customersUrl}`)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            }
        }
        const storageUrl = 'storage'
        const newStorageUrl = 'new-api/StorageDetail'
        this.storage = {
            async getStoragePutawayList(inJobNo, lineItem = null) {
                try {
                    return (await self.apiAxios.get(`${storageUrl}/getStoragePutawayList`, { params: { inJobNo: inJobNo, lineItem: lineItem } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async updateBuyingPrice(updateBuyingPriceDto) {
                try {
                    return (await self.apiAxios.post(`${storageUrl}/updateBuyingPrice`, updateBuyingPriceDto)).data
                }
                catch (err) {
                    self.errorHandlers.form(err)
                    throw err
                }
            },
            async updateSellingPrice(updateBuyingPriceDto) {
                try {
                    return (await self.apiAxios.post(`${storageUrl}/updateSellingPrice`, updateBuyingPriceDto)).data
                }
                catch (err) {
                    self.errorHandlers.form(err)
                    throw err
                }
            },
            async getStorageDetailWithPartsInfoList(availableFilter) {
                try {
                    return (await self.apiAxios.get(`${newStorageUrl}/getStorageDetailWithPartsInfoList`, { params: { filter: availableFilter } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async hasBondedStock(outJobNo) {
                try {
                    return (await self.apiAxios.get(`${storageUrl}/hasBondedStock`, { params: { outJobNo: outJobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getStorageSupplierList(customerCode) {
                try {
                    return (await self.apiAxios.get(`${storageUrl}/getStorageSupplierList`, { params: { customerCode: customerCode } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async printStorageLabels(pid, printer, type, copies) {
                try {
                    return (await self.apiAxios.post(`${storageUrl}/printStorageLabels`, { pid, printer, type, copies })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getExternalQRStorageLabelsForInbound(inJobNo) {
                try {
                    return (await self.apiAxios.get(`${storageUrl}/getExternalQRStorageLabelsForInbound`, { params: { inJobNo: inJobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getStorageInJobNosList(customerCode, supplierId) {
                try {
                    return (await self.apiAxios.get(`${storageUrl}/getStorageInJobNosList`, { params: { customerCode: customerCode, supplierID: supplierId } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getSTFStorageDetailList(jobNo, inJobNo, supplierId) {
                try {
                    return (await self.apiAxios.get(`${storageUrl}/getSTFStorageDetailList`, { params: { jobNo: jobNo, inJobNo: inJobNo, supplierId: supplierId } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getStorageLabels(pid) {
                try {
                    return (await self.apiAxios.post(`${storageUrl}/getStorageLabels`, { pid } )).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            }
        }

        const storageGroupUrl = 'storage_group'
        this.storageGroup = {
            async getGroups(filter) {
                try {
                    return (await self.apiAxios.get(`${storageGroupUrl}/getGroups`, { params: filter })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async createGroup(qty, prefix) {
                try {
                    return (await self.apiAxios.get(`${storageGroupUrl}/createGroup`, { params: {qty, prefix} })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async deleteGroup(groupId) {
                try {
                    return (await self.apiAxios.delete(`${storageGroupUrl}/${groupId}`)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async transformGroup(groupId) {
                try {
                    return (await self.apiAxios.post(`${storageGroupUrl}/${groupId}/transform`)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async printLabels(pid, printer, type, copies) {
                try {
                    return (await self.apiAxios.post(`${storageGroupUrl}/printLabels`, { pid, printer, type, copies  })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async printPIDLabels(pid, printer, type) {
                try {
                    return (await self.apiAxios.post(`${storageGroupUrl}/printPIDLabels`, { pid, printer, type  })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async details(groupId) {
                try {
                    return (await self.apiAxios.get(`${storageGroupUrl}/details`, { params: {groupId}  })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getStorageLabelsForGIDs(gid) {
                try {
                    return (await self.apiAxios.post(`${storageGroupUrl}/getStorageLabelsForGIDs`, gid )).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getGroupLabels(gid) {
                try {
                    return (await self.apiAxios.post(`${storageGroupUrl}/getGroupLabels`, gid )).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            }
       }
        const partMastersUrl = 'partMasters'
        this.partMasters = {
            async getPartMasterListBySupplier(customerCode, supplierId) {
                try {
                    return (await self.apiAxios.get(`${partMastersUrl}/getPartMasterListBySupplier`, { params: { customerCode: customerCode, supplierID: supplierId } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getPartMasterList(filter) {
                try {
                    return (await self.apiAxios.get(`${partMastersUrl}/getPartMasterList`, { params: filter })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getPartMaster(customerCode, productCode1, supplierID) {
                try {
                    return (await self.apiAxios.get(`${partMastersUrl}/getPartMaster`, {
                        params: {
                            customerCode: customerCode,
                            productCode1: productCode1,
                            supplierID: supplierID
                        }
                    })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getUOMListWithDecimal(customerCode) {
                try {
                    return (await self.apiAxios.get(`${partMastersUrl}/getUOMListWithDecimal`, {
                        params: {
                            customerCode: customerCode,
                        }
                    })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async createPartMaster(partMaster) {
                try {
                    return (await self.apiAxios.post(`${partMastersUrl}/createPartMaster`, partMaster)).data
                }
                catch (err) {
                    self.errorHandlers.form(err)
                    throw err
                }
            },
            async updatePartMaster(partMaster) {
                try {
                    return (await self.apiAxios.patch(`${partMastersUrl}/updatePartMaster`, partMaster)).data
                }
                catch (err) {
                    self.errorHandlers.form(err)
                    throw err
                }
            },
            async iLogReadinessStatuses() {
                try {
                    return (await self.apiAxios.get(`${partMastersUrl}/iLogReadinessStatuses`)).data
                }
                catch (err) {
                    self.errorHandlers.form(err)
                    throw err
                }
            },
            async getPalletTypes() {
                try {
                    return (await self.apiAxios.get(`${partMastersUrl}/palletTypes`)).data
                }
                catch (err) {
                    self.errorHandlers.form(err)
                    throw err
                }
            },
            async getEllisPalletTypes() {
                try {
                    return (await self.apiAxios.get(`${partMastersUrl}/ELLISPalletTypes`)).data
                }
                catch (err) {
                    self.errorHandlers.form(err)
                    throw err
                }
            },
            async getUnloadingPoints(customerCode, supplierId) {
                try {
                    return (await self.apiAxios.get(`${partMastersUrl}/unloadingPointChoice`, { params: { customerCode: customerCode, supplierID: supplierId } })).data
                }
                catch (err) {
                    self.errorHandlers.form(err)
                    throw err
                }
            }
        }
        const inventoryUrl = 'inventory'
        this.inventory = {
            async getCustomerInventoryControlCodeMap(customerCode) {
                try {
                    return (await self.apiAxios.get(`${inventoryUrl}/getCustomerInventoryControlCodeMap`, { params: { customerCode } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getCustomerInventoryProductCodeMap(customerCode) {
                try {
                    return (await self.apiAxios.get(`${inventoryUrl}/getCustomerInventoryProductCodeMap`, { params: { customerCode } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
        }

        const priceMastersUrl = 'priceMasters'
        this.priceMasters = {
            async updatePriceMasterInbound(priceMasterInboundUpdateDto) {
                try {
                    return (await self.apiAxios.post(`${priceMastersUrl}/updatePriceMasterInbound`, priceMasterInboundUpdateDto)).data
                }
                catch (err) {
                    self.errorHandlers.form(err)
                    throw err
                }
            },
            async updatePriceMasterOutbound(priceMasterOutoundUpdateDto) {
                try {
                    return (await self.apiAxios.post(`${priceMastersUrl}/updatePriceMasterOutbound`, priceMasterOutoundUpdateDto)).data
                }
                catch (err) {
                    self.errorHandlers.form(err)
                    throw err
                }
            }
        }

        const labelPrintersUrl = 'labelPrinters'
        this.labelPrinters = {
            async getLabelPrinters() {
                try {
                    return (await self.apiAxios.get(`${labelPrintersUrl}`)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            }
        }

        const usersUrl = 'users'
        this.users = {
            async getUsers(filter) {
                try {
                    return (await self.apiAxios.get(`${usersUrl}`, { params: filter })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getUser(code) {
                try {
                    return (await self.apiAxios.get(`${usersUrl}/getUser`, { params: { code } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async patchUser(user) {
                var code = user.code;
                try {
                    return (await self.apiAxios.patch(`${usersUrl}`, user, { params: { code } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async toggleStatus(code) {
                try {
                    return (await self.apiAxios.post(`${usersUrl}/toggleStatus`, null, { params: { code } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async createUser(user) {
                try {
                    return (await self.apiAxios.post(`${usersUrl}`, user)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            }
        }

        const stockTransfersUrl = 'stockTransfers'
        this.stockTransfers = {
            async getStockTransferList(filter) {
                try {
                    return (await self.apiAxios.get(`${stockTransfersUrl}/getStockTransferList`, { params: filter })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getStockTransfer(jobNo) {
                try {
                    return (await self.apiAxios.get(`${stockTransfersUrl}/getStockTransfer`, { params: { jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getStockTransferDetails(jobNo) {
                try {
                    return (await self.apiAxios.get(`${stockTransfersUrl}/getStockTransferDetails`, { params: { jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getAllowedSTFImportMethods() {
                try {
                    return (await self.apiAxios.get(`${stockTransfersUrl}/allowedSTFImportMethods`)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getStockTransferSummary(jobNo) {
                try {
                    return (await self.apiAxios.get(`${stockTransfersUrl}/getStockTransferSummary`, { params: { jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async createStockTransfer(customerCode) {
                try {
                    return (await self.apiAxios.post(`${stockTransfersUrl}`, null, { params: { customerCode: customerCode } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async addStockTransferDetailByPID(dto) {
                try {
                    return (await self.apiAxios.post(`${stockTransfersUrl}/addStockTransferDetailByPID`, dto)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async deleteStockTransferDetailByPID(dto) {
                try {
                    return (await self.apiAxios.post(`${stockTransfersUrl}/deleteStockTransferDetailByPID`, dto)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async splitByInboundDate(jobNo) {
                try {
                    return (await self.apiAxios.post(`${stockTransfersUrl}/splitByInboundDate`, null, { params: { jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async updateStockTransfer(jobNo, stockTransfer) {
                try {
                    return (await self.apiAxios.patch(`${stockTransfersUrl}`, stockTransfer, { params: { jobNo: jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async downloadEDTToCSV(jobNo) {
                try {
                    var resp = (await self.apiAxios.get(`${stockTransfersUrl}/downloadEDTToCSV`, {
                        params: {
                            jobNo: jobNo
                        },
                        responseType: 'blob',
                        headers: {
                            Accept: 'text/plain'
                        }
                    }))
                    let headerLine = resp.headers['Content-Disposition'] || resp.headers['content-disposition'];
                    const regExpFilename = /filename="?(?<filename>[^;"]*)/;
                    let fileName = regExpFilename.exec(headerLine)?.groups?.filename ?? null;
                    saveData(resp.data, fileName, "text/plain")
                }
                catch (err) {
                    self.errorHandlers.reports(err)
                    throw err
                }
            },
            async importEKanbanEUCPart(orderNo, errorParams={}) {
                try {
                    return (await self.apiAxios.post(`${stockTransfersUrl}/importEKanbanEUCPart`, null, { params: { orderNo: orderNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err, errorParams)
                    throw err
                }
            },
            async importEKanbanEUCPartMulti(orderNos) {
                try {
                    return (await self.apiAxios.post(`${stockTransfersUrl}/importEKanbanEUCPartMulti`, { orderNos: orderNos })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async importEStockTransfer(orderNo) {
                try {
                    return (await self.apiAxios.post(`${stockTransfersUrl}/importEStockTransfer`, null, { params: { orderNo: orderNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async importEStockTransferMulti(orderNos) {
                try {
                    return (await self.apiAxios.post(`${stockTransfersUrl}/importEStockTransferMulti`, { orderNos: orderNos })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async deleteStockTransferDetail(jobNo, lineItem) {
                try {
                    return (await self.apiAxios.post(`${stockTransfersUrl}/deleteStockTransferDetail`, null, { params: { jobNo, lineItem } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async cancel(jobNo) {
                try {
                    return (await self.apiAxios.post(`${stockTransfersUrl}/cancel`, null, { params: { jobNo: jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async complete(jobNo) {
                try {
                    return (await self.apiAxios.post(`${stockTransfersUrl}/complete`, null, { params: { jobNo: jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            }
        }

        const stockTransferReversalUrl = 'StockTransferReversal'
        this.stockTransferReversal = {
            async isActive() {
                try {
                    return (await self.apiAxios.get(`new-api/${stockTransferReversalUrl}/isActive`)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            }
        }

        const eStockTransfersUrl = 'eStockTransfers'
        this.eStockTransfers = {
            async EStockTransferCheck(orderNos) {
                try {
                    return (await self.apiAxios.post(`${eStockTransfersUrl}/EStockTransferCheck`, { orderNos: orderNos })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getEStockTransferList(filter) {
                try {
                    return (await self.apiAxios.get(`${eStockTransfersUrl}/getEStockTransferList`, { params: filter })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async hasAnyEStockTransferDiscrepancy(jobNo) {
                try {
                    return (await self.apiAxios.get(`${eStockTransfersUrl}/hasAnyEStockTransferDiscrepancy`, { params: { jobNo: jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            }
        }

        const accessGroupsUrl = 'accessGroups'
        this.accessGroups = {
            async get(filter) {
                try {
                    return (await self.apiAxios.get(`${accessGroupsUrl}`, { params: filter })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getWithPaging(filter) {
                try {
                    var result = (await self.apiAxios.get(`${accessGroupsUrl}`, { params: filter })).data;
                    return {
                        data: result,
                        pageNo: 1,
                        pageSize: result.length || 1,
                        total: result.length || 0
                    }
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getAccessGroup(code) {
                try {
                    return (await self.apiAxios.get(`${accessGroupsUrl}/getAccessGroup`, { params: { code } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async patchAccessGroup(code,group) {
                try {
                    return (await self.apiAxios.patch(`${accessGroupsUrl}`, group, { params: { code } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getPrivilegesTree(groupCode) {
                try {
                    return (await self.apiAxios.get(`${accessGroupsUrl}/getPrivilegesTree`, { params: { groupCode } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async updatePrivilegesTree(groupCode,privTree) {
                try {
                    return (await self.apiAxios.post(`${accessGroupsUrl}/updatePrivilegesTree`, privTree, { params: { groupCode } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async toggleStatus(code) {
                try {
                    return (await self.apiAxios.post(`${accessGroupsUrl}/toggleStatus`, null, { params: { code } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async createAccessGroup(group) {
                try {
                    return (await self.apiAxios.post(`${accessGroupsUrl}`, group)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            }
        }
        
        const versionUrl = 'version'
        this.version = {
            async get() {
                try {
                    return (await self.apiAxios.get(`${versionUrl}`)).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            }
        }
        const locksUrl = 'locks'
        this.locks = {
            async tryLock(jobNo, moduleName) {
                try {
                    return (await self.apiAxios.post(`${locksUrl}/lock`, null, { params: {jobNo, moduleName, clientId: storage.clientId} })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async getLock(jobNo) {
                try {
                    return (await self.apiAxios.get(`${locksUrl}/lock`, { params: { jobNo } })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
            async tryUnlock(jobNo) {
                try {
                    return (await self.apiAxios.post(`${locksUrl}/unlock`, null, { params: {jobNo, clientId: storage.clientId} })).data
                }
                catch (err) {
                    self.errorHandlers.simple(err)
                    throw err
                }
            },
        }
        this.ilogIntegration = {
            async isILogEnabled() {
                try {
                    return !!(await self.apiAxios.get('new-api/ILogIntegration/isActiveForWarehouse')).data;
                }
                catch {
                    return false;
                }
            },
            async releaseOutboundForPicking(jobNo) {
                return (await self.apiAxios.post('new-api/ilogpickingrequest/requestOutboundPicking', {jobNo: jobNo})).data
            },
            async releaseLoadingForPicking(jobNo) {
                return (await self.apiAxios.post('new-api/ilogpickingrequest/requestLoadingPicking', {jobNo: jobNo})).data
            },
            async incompleteILogOutbounds(jobNo){
                return (await self.apiAxios.get('new-api/ilogpickingrequest/incompleteILogOutbounds', { params: {loadingJobNo: jobNo}})).data
            }
        }
    }
}

const DataProviderPlugin = {
    install(app, options) {
        const dp = new DataProvider(app, options)
        app.config.globalProperties.$dataProvider = dp
        app.provide('DP', dp);
    }
}

export default DataProviderPlugin;