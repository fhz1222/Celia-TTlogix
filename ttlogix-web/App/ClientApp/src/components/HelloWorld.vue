<template>
    <div class="wrap hello">
        <h1>Temporary test page</h1>


        <br /><br />
        <button v-on:click="getSettings">Get Settings</button>

        <label>Query param:  </label><input type="text" v-model="queryParameter" />
        <br /><br />

        <button v-on:click="getOutbounds">Get Outbounds</button>
        <button v-on:click="getSupplierList">Get Supplier List</button>
        <button v-on:click="getEKanbanForEurope">Get EKanbans for Europe</button>
        <button v-on:click="getWarehouses">Get WHSs</button>
        <button v-on:click="getOutboundDetails">Outbound details</button>
        <button v-on:click="getOutboundDetailsWithReceivedQty">Outbound details w/rec qty</button>
        <button v-on:click="getPickingListWithUOM">Picking List With UOM</button>
        <button v-on:click="getEKanbanPartsStatusByOwnershipEHP">getEKanbanPartsStatusByOwnershipEHP</button>
        <button v-on:click="getEKanbanPartsStatusForEKanbanCPart">getEKanbanPartsStatusForEKanbanCPart</button>
        <button v-on:click="getEKanbanPartsStatusByOwnership">getEKanbanPartsStatusByOwnership</button>
        <button v-on:click="getEKanbanPartsStatusForCPart">getEKanbanPartsStatusForCPart</button>
        <button v-on:click="getEKanbanPartsStatusForCPartWithoutExt">getEKanbanPartsStatusForCPartWithoutExt</button>
        <br />
        <br />
        <button v-on:click="importEKanbanEUCPart">importEKanbanEUCPart</button>
        <br />
        <br />
        <button v-on:click="postOutbound">createOutboundManual</button>
        <button v-on:click="cancelOutbound">cancelOutbound</button>
        <button v-on:click="completeOutboundEurope">completeOutboundEurope</button>
        <button v-on:click="cargoInTransit">cargoInTransit</button>
        <button v-on:click="cancelAllocation">cancelAllocation</button>
        <button v-on:click="splitOutbound">splitOutbound</button>
        <button v-on:click="releaseBondedStock">releaseBondedStock</button>
        <button v-on:click="getPickingDataToDownload">getPickingDataToDownload</button>
        <button v-on:click="downloadEDTToCSV">downloadEDTToCSV</button>
        <button v-on:click="getEKanbanDataToDownload">getEKanbanDataToDownload</button>
        <button v-on:click="getOutboundQRCodeImage">getOutboundQRCodeImage</button>
        <button v-on:click="getOutboundPickableList">getOutboundPickableList</button>
        <button v-on:click="autoAllocate">autoAllocate</button>
        <button v-on:click="checkEKanbanFulfillable">checkEKanbanFulfillable</button>
        <button v-on:click="hasEKanban">hasEKanban</button>
        <br />
        <button v-on:click="getGroupsAll">get All Groups</button>
        <button v-on:click="getGroups">get 001 Group</button>

        <br />
        <br />
        <button v-on:click="getLoadings">getLoadings</button>
        <button v-on:click="getLoading">getLoading</button>
        <button v-on:click="getLoadingDetails">getLoadingDetails</button>
        <button v-on:click="getLoadingEntryList">getLoadingEntryList</button>
        <button v-on:click="postLoading">createLoading</button>
        <button v-on:click="getDeliveryDocketReport">getDeliveryDocketReport</button>
        <button v-on:click="confirmTruckArrival">confirmTruckArrival</button>
        <br />
        <br />
        <button v-on:click="getASNDetails">getASNDetails</button>
        <button v-on:click="getCurrencies">getCurrencies</button>
        <button v-on:click="getInbound">getInbound</button>
        <button v-on:click="getInboundDetails">getInboundDetails</button>
        <button v-on:click="getIDTToCSV">getIDTToCSV</button>

        <br />
        <br />
        <button v-on:click="getStockTransferSummary">getStockTransferSummary</button>

        <!--<button v-on:click="createPartMaster">createPartMaster</button>
    <button v-on:click="updatePartMaster">updatePartMaster</button>-->


        <h3>{{error}}</h3>
        <br />
        <span>{{testData}}</span>

        <pdf-preview v-if="modal == 'report'" :name="pdfName" :pdf="pdf" @close="pdfName = pdf = null; modal = null"></pdf-preview>

    </div>
</template>

<script>
    import PDFPreview from '@/widgets/PDFPreview'

    import { defineComponent } from 'vue';
export default defineComponent({
        components: { 'pdf-preview': PDFPreview },
        data() {
            return {
                token: null,
                login: null,
                pass: null,
                queryParameter: "",
                error: null,
                testData: null,
                imageSrc: "",
                pdfName: null,
                pdf: null,
                modal: null,
            }
        },
        methods: {
            getGroups() {
                this.$dataProvider.accessGroups.get({ code: '001' } )
                    .then((result) => {
                        this.testData = result;
                    });
            },
            getGroupsAll() {
                this.$dataProvider.accessGroups.get()
                    .then((result) => {
                        this.testData = result;
                    });
            },
            getSettings() {
                this.$dataProvider.settings.get()
                    .then((result) => {
                        this.testData = result;
                    });
            },
            getDeliveryDocketReport() {
                this.modal = 'report',
                    this.$dataProvider.loadings.getDeliveryDocketReport(this.queryParameter)
                        .then(resp => {
                            this.pdf = resp.pdf;
                            this.pdfName = resp.pdfName;
                        }).catch((e) => {

                            if (e.response.data) {
                                e.response.data.text().then((r) => {
                                    this.error = r;
                                });
                            }
                        });
            },
            getLoadings() {
                this.$dataProvider.loadings.getLoadings({ orderBy: 'JobNo' })
                    .then((result) => {
                        this.testData = result;
                    });
            },
            getLoading() {
                this.$dataProvider.loadings.getLoading(this.queryParameter)
                    .then((result) => {
                        this.testData = result;
                    });
            },
            getLoadingDetails() {
                this.$dataProvider.loadings.getLoadingDetails(this.queryParameter)
                    .then((result) => {
                        this.testData = result;
                    });
            },
            getLoadingEntryList() {
                this.$dataProvider.loadings.getLoadingEntryList(this.queryParameter)
                    .then((result) => {
                        this.testData = result;
                    });
            },
            postLoading() {
                this.$dataProvider.loadings.postLoading({
                    customerCode: 'PLS',
                    //wHSCode: 'PL',
                    refNo: '111',
                    remark: 'aaa',
                    eTD: '2020-10-29',
                }).then((resp) => {
                    this.error = null
                    this.token = resp.token
                }).catch((e) => {

                    if (e.response && e.response.data) {
                        this.error = e.response.data.message;
                    }
                });
            },
            getASNDetails() {
                this.$dataProvider.inbounds.getASNDetails(this.queryParameter)
                    .then((result) => {
                        this.testData = result;
                    });
            },
            getCurrencies() {
                this.$dataProvider.currencies.getCurrencies()
                    .then((result) => {
                        this.testData = result;
                    });
            },
            getStockTransferSummary() {
                this.$dataProvider.stockTransfers.getStockTransferSummary(this.queryParameter)
                    .then((result) => {
                        this.testData = result;
                    });
            },
            getInbound() {
                this.$dataProvider.inbounds.getInbound(this.queryParameter)
                    .then((result) => {
                        this.testData = result;
                    });
            },
            getInboundDetails() {
                this.$dataProvider.inbounds.getInboundDetails(this.queryParameter)
                .then((result) => {
                    this.testData = result;
                });
            },
            getOutbounds() {
                this.$dataProvider.outbounds.getOutbounds({ orderBy: 'JobNo' })
                    .then((result) => {
                        this.testData = result;
                    });
            },
            getSupplierList() {
                this.$dataProvider.supplierMasters.getList('PLY')
                    .then((result) => {
                        this.testData = result;
                    });
            },
            hasEKanban() {
                this.$dataProvider.ekanbans.hasEKanban(this.queryParameter)
                    .then((result) => {
                        this.testData = result;
                    });
            },
            getEKanbanForEurope() {
                this.$dataProvider.ekanbans.getEKanbanListForEurope()
                    .then((result) => {
                        this.testData = result;
                    });
            },
            getWarehouses() {
                this.$dataProvider.warehouses.getWarehouses()
                    .then((result) => {
                        this.testData = result;
                    });
            },
            getOutboundDetails() {
                this.$dataProvider.outbounds.getOutboundDetails(this.queryParameter)
                    .then((result) => {
                        this.testData = result;
                    });
            },
            getOutboundDetailsWithReceivedQty() {
                this.$dataProvider.outbounds.getOutboundDetailsWithReceivedQty(this.queryParameter)
                    .then((result) => {
                        this.testData = result;
                    });
            },
            getPickingListWithUOM() {
                this.$dataProvider.pickingLists.getPickingListWithUOM(this.queryParameter)
                    .then((result) => {
                        this.testData = result;
                    });
            },
            getOutboundPickableList() {
                this.$dataProvider.outbounds.getOutboundPickableList({
                    outboundTransType: 2,
                    customerCode: 'PLV',
                    supplierID: 504085,
                    WHSCode: 'PL',
                    partMasterStatus: 1
                })
                    .then((result) => {
                        this.testData = result;
                    });
            },

            /// eKanban
            getEKanbanPartsStatusByOwnershipEHP() {
                if (this.queryParameter) {
                    this.$dataProvider.ekanbans.getEKanbanPartsStatusByOwnershipEHP(this.queryParameter)
                        .then((result) => {
                            this.testData = result;
                        });
                }
            },
            getEKanbanPartsStatusForEKanbanCPart() {
                if (this.queryParameter) {
                    this.$dataProvider.ekanbans.getEKanbanPartsStatusForEKanbanCPart(this.queryParameter)
                        .then((result) => {
                            this.testData = result;
                        });
                }
            },
            getEKanbanPartsStatusByOwnership() {
                if (this.queryParameter) {
                    this.$dataProvider.ekanbans.getEKanbanPartsStatusByOwnership(this.queryParameter)
                        .then((result) => {
                            this.testData = result;
                        });
                }
            },
            getEKanbanPartsStatusForCPart() {
                if (this.queryParameter) {
                    this.$dataProvider.ekanbans.getEKanbanPartsStatusForCPart(this.queryParameter)
                        .then((result) => {
                            this.testData = result;
                        });
                }
            },
            getEKanbanPartsStatusForCPartWithoutExt() {
                if (this.queryParameter) {
                    this.$dataProvider.ekanbans.getEKanbanPartsStatusForCPartWithoutExt(this.queryParameter)
                        .then((result) => {
                            this.testData = result;
                        });
                }
            },

            importEKanbanEUCPart() {
                this.$dataProvider.outbounds.importEKanbanEUCPart('TTK1402030002').then((resp) => {
                    this.error = null
                    this.token = resp.token
                }).catch((e) => {

                    if (e.response && e.response.data) {
                        this.error = e.response.data.message;
                    }
                });
            },
            createPartMaster() {
                this.$dataProvider.partMasters.createPartMaster({
                    customerCode: 'PLY',
                    supplierID: '509376',
                    productCode1: 'PRODCO10',
                    description: 'desc',
                    uom: 'UOM0053',
                    width: 100,
                    length: 100,
                    height: 100,
                    netWeight: 20,
                    grossWeight: 30,
                    packageType: 'PKG0001',
                    orderLot: 20,
                    originCountry: 'PL',
                    status: 1,
                }).then((resp) => {
                    this.error = null
                    this.token = resp.token
                }).catch((e) => {

                    if (e.response && e.response.data) {
                        this.error = e.response.data.message;
                    }
                });
            },
            updatePartMaster() {
                this.$dataProvider.partMasters.updatePartMaster({
                    customerCode: 'PLY',
                    supplierID: '509376',
                    productCode1: 'PRODCO10',
                    description: 'desc',
                    uom: 'UOM0053',
                    width: 200,
                    length: 300,
                    height: 400,
                    netWeight: 10,
                    grossWeight: 80,
                    packageType: 'PKG0001',
                    orderLot: 80,
                    originCountry: 'PL',
                    status: 1,
                }).then((resp) => {
                    this.error = null
                    this.token = resp.token
                }).catch((e) => {

                    if (e.response && e.response.data) {
                        this.error = e.response.data.message;
                    }
                });
            },

            postOutbound() {
                this.$dataProvider.outbounds.createOutboundManual({
                    manualType: 1,
                    jobNo: '123456768',
                    customerCode: 'PLS',
                    wHSCode: 'PL',
                    //oSNo: '',
                    //refNo: '',
                    eTD: '2020-10-29',
                    transType: 0,
                    //newWHSCode: '',
                    status: 0,
                }).then((resp) => {
                    this.error = null
                    this.token = resp.token
                }).catch((e) => {

                    if (e.response && e.response.data) {
                        this.error = e.response.data.message;
                    }
                });
            },

            cancelOutbound() {
                this.$dataProvider.outbounds.cancelOutbound(this.queryParameter).then((resp) => {
                    this.error = null
                    this.token = resp.token
                }).catch((e) => {

                    if (e.response && e.response.data) {
                        this.error = e.response.data.message;
                    }
                });
            },
            completeOutboundEurope() {
                this.$dataProvider.outbounds.completeOutboundEurope([this.queryParameter]).then((resp) => {
                    this.error = null
                    this.token = resp.token
                }).catch((e) => {

                    if (e.response && e.response.data) {
                        this.error = e.response.data.message;
                    }
                });
            },
            confirmTruckArrival() {
                this.$dataProvider.loadings.truckArrival(this.queryParameter).then(() => {
                    console.log('success')
                })
                    .catch(e => console.log(e))
            },
            cargoInTransit() {
                this.$dataProvider.outbounds.cargoInTransit([this.queryParameter]).then((resp) => {
                    this.error = null
                    this.token = resp.token
                }).catch((e) => {

                    if (e.response && e.response.data) {
                        this.error = e.response.data.message;
                    }
                });
            },
            cancelAllocation() {
                this.$dataProvider.outbounds.cancelAllocation(
                    {
                        jobNo: 'OUT20200900626',
                        lineItem: 1,
                        itemsToCancel: [{ lineItem: 1, seqNo: 1 }, { lineItem: 1, seqNo: 2 }]
                    }).then((resp) => {
                        this.error = null
                        this.token = resp.token
                    }).catch((e) => {

                        if (e.response && e.response.data) {
                            this.error = e.response.data.message;
                        }
                    });
            },
            splitOutbound() {
                this.$dataProvider.outbounds.splitOutbound(this.queryParameter, true)
                    .catch((e) => {
                        if (e.response && e.response.data) {
                            this.error = e.response.data.message;
                        }
                    });
            },
            releaseBondedStock() {
                this.$dataProvider.outbounds.releaseBondedStock([this.queryParameter]).then((resp) => {
                    this.error = null
                    this.token = resp.token
                }).catch((e) => {

                    if (e.response && e.response.data) {
                        this.error = e.response.data.message;
                    }
                });
            },
            getPickingDataToDownload() {
                this.$dataProvider.pickingLists.getPickingDataToDownload({ jobNos: this.queryParameter })
                    .then((response) => {
                        let blob = new Blob([response], { type: 'text/plain' });
                        let link = document.createElement('a');
                        link.href = window.URL.createObjectURL(blob);
                        link.download = "doc.txt";
                        link.click();
                    });
            },
            downloadEDTToCSV() {
                this.$dataProvider.outbounds.downloadEDTToCSV({ jobNo: this.queryParameter })
                    .then((response) => {
                        let blob = new Blob([response], { type: 'text/plain' });
                        let link = document.createElement('a');
                        link.href = window.URL.createObjectURL(blob);
                        link.download = "doc.txt";
                        link.click();
                    });
            },
            getEKanbanDataToDownload() {
                this.$dataProvider.ekanbans.getEKanbanDataToDownload(this.queryParameter)
                    .then((response) => {

                        let blob = new Blob([response], { type: 'text/plain' });
                        let link = document.createElement('a');
                        link.href = window.URL.createObjectURL(blob);
                        link.download = "doc.txt";
                        link.click();
                    });
            },
            getOutboundQRCodeImage() {
                this.$dataProvider.outbounds.getOutboundQRCodeImage(this.queryParameter)
                    .then((response) => {
                        this.imageSrc = "data:image/bmp;base64," + response;
                    });
            },
            autoAllocate() {
                this.$dataProvider.pickingLists.autoAllocate(this.queryParameter, 1)
                    .then((result) => {
                        this.testData = result;
                    });
            },
            checkEKanbanFulfillable() {
                this.$dataProvider.ekanbans.checkEKanbanFulfillable([this.queryParameter])
                    .then((result) => {
                        this.testData = result;
                    });
            },
            getIDTToCSV() {
                this.$dataProvider.inbounds.downloadIDTToCSV(this.queryParameter)
            },
        }


    })
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
    h3 {
        margin: 40px 0 0;
    }

    ul {
        list-style-type: none;
        padding: 0;
    }

    li {
        display: inline-block;
        margin: 0 10px;
    }

    a {
        color: #42b983;
    }

    button {
        margin: 5px;
    }
</style>
