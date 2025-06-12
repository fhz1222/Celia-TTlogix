<template>
    <div class="wrap">
        <!-- Page title -->
        <div class="row toolbar-new">
            <div class="col-md-8">
                <div class="mt-3 me-2 float-start loading-status-indicator" :style="{'background-color': statusColor}"></div>
                <h2 class="mt-3 mb-3 display-6 float-start">{{$t('loading.detail.loadingJobNo__', {'jobNo': jobNo}) }}</h2>
            </div>
            <div class="col-md-4 text-end pe-4 main-action-icons">
                <div class="d-inline-block link-box">
                    <a href="#" @click.stop.prevent="refresh()" class="text-center">
                        <div><i class="las la-redo-alt"></i></div>
                        <div>Refresh</div>
                    </a>
                </div>
            </div>
        </div>

        <div class="" v-if="!locked">
            {{ lockingMesage }}
        </div>
        <div v-else>
            <!-- Outbound header -->
            <div class="loading-detail">
                <loading-header v-model="header" :errors="errors"
                                @save="(callback) => { save().finally(callback) }"
                                @cancel="cancel()"
                                @confirm="confirm()"
                                @releaseForPicking="modal = 'request_picking'" />
            </div>

            <!-- Reports -->
            <div class="card mt-4 mb-5">
                <div class="card-header">
                    <h5>{{$t('loading.detail.documentation')}}</h5>
                </div>
                <div class="card-body">
                    <button class="btn btn-sm btn-primary me-2" @click="report('LoadingReport')"><i class="las la-file-invoice"></i> 1. {{$t('loading.report.loadingReport')}}</button>
                    <button class="btn btn-sm btn-primary me-2" @click="report('DeliveryDocketReport')"><i class="las la-file-invoice"></i> 2. {{$t('loading.report.deliveryDocket')}}</button>
                    <button class="btn btn-sm btn-primary me-2" @click="report('DeliveryDocketCombinedReport')"><i class="las la-file-invoice"></i> 2.1. {{$t('loading.report.deliveryDocketCombined')}}</button>
                    <button class="btn btn-sm btn-primary me-2" @click="report('PickingInstructionReport')"><i class="las la-file-invoice"></i> 3. {{$t('loading.report.pickingInstruction')}}</button>
                    <button class="btn btn-sm btn-primary" @click="report('OutboundReport')"><i class="las la-file-invoice"></i> 4. {{$t('loading.report.outboundReport')}}</button>
                </div>
            </div>

            <!-- Loading Details -->
            <div class="card mt-4">
                <div class="card-header">
                    <h5 class="float-start">{{$t('loading.detail.loadingDetails')}}</h5>

                    <div v-if="incompleteILogOutbounds.length > 0" class="float-end alert alert-danger mb-0 px-1 py-0">There are outbounds with completed Picking Requests that are not fully picked.</div>

                </div>
                <div class="card-body loading-details-table">

                    <!-- Table -->
                    <table class="table table-sm table-striped table-hover align-middle selectable" v-if="details.length">
                        <thead>
                            <tr>
                                <th colspan="11" class="no-border"></th>
                                <th colspan="3" style="text-align: center; border-bottom: 1px solid grey">{{$t('loading.detail.outboundList_NoOfPallets')}}</th>
                            </tr>
                            <tr>
                                <th><input type="checkbox" v-model="selectAll" @click.stop="selectDeselectAll()" /></th>
                                <th class="num">{{$t('loading.detail.outboundList_No')}}</th>
                                <th class="pointer" @click.prevent.stop="sort(row => row.orderNo,'orderNo')">
                                    {{$t('loading.detail.outboundList_OrderNo')}}
                                    <i v-if="sortedBy == 'orderNo'" :class="sortIcon()"></i>
                                </th>
                                <th class="pointer" @click.prevent.stop="sort(row => row.outJobNo,'outJobNo')">
                                    {{$t('loading.detail.outboundList_OutboundJobNo')}}
                                    <i v-if="sortedBy == 'outJobNo'" :class="sortIcon()"></i>
                                </th>
                                <th class="pointer" @click.prevent.stop="sort(row => row.supplierID,'supplierID')">
                                    {{$t('loading.detail.outboundList_SupplierID')}}
                                    <i v-if="sortedBy == 'supplierID'" :class="sortIcon()"></i>
                                </th>
                                <th class="pointer" @click.prevent.stop="sort(row => row.companyName,'companyName')">
                                    {{$t('loading.detail.outboundList_SupplierName')}}
                                    <i v-if="sortedBy == 'companyName'" :class="sortIcon()"></i>
                                </th>
                                <th class="pointer" @click.prevent.stop="sort(row => row.xDock,'xDock')">
                                    {{$t('loading.detail.outboundList_XDock')}}
                                    <i v-if="sortedBy == 'xDock'" :class="sortIcon()"></i>
                                </th>
                                <th class="pointer" @click.prevent.stop="sort(row => row.etd,'etd')">
                                    {{$t('loading.detail.outboundList_ETD')}}
                                    <i v-if="sortedBy == 'etd'" :class="sortIcon()"></i>
                                </th>
                                <th class="pointer" @click.prevent.stop="sort(row => row.commInvNo,'commInvNo')">
                                    {{$t('loading.detail.outboundList_CommInvNo')}}
                                    <i v-if="sortedBy == 'commInvNo'" :class="sortIcon()"></i>
                                </th>
                                <th class="pointer" @click.prevent.stop="sort(row => row.addedDate,'addedDate')">
                                    {{$t('loading.detail.outboundList_AddedDate')}}
                                    <i v-if="sortedBy == 'addedDate'" :class="sortIcon()"></i>
                                </th>
                                <th class="pointer" @click.prevent.stop="sort(row => row.status,'status')">
                                    {{$t('loading.detail.outboundList_Status')}}
                                    <i v-if="sortedBy == 'status'" :class="sortIcon()"></i>
                                </th>
                                <th class="pointer num" @click.prevent.stop="sort(row => row.noOfPalletsEHP,'noOfPalletsEHP')">
                                    {{$t('loading.detail.outboundList_EHP')}}
                                    <i v-if="sortedBy == 'noOfPalletsEHP'" :class="sortIcon()"></i>
                                </th>
                                <th class="pointer num" @click.prevent.stop="sort(row => row.noOfPalletsSupplier,'noOfPalletsSupplier')">
                                    {{$t('loading.detail.outboundList_Sup')}}
                                    <i v-if="sortedBy == 'noOfPalletsSupplier'" :class="sortIcon()"></i>
                                </th>
                                <th class="pointer num" @click.prevent.stop="sort(row => row.noOfPallet,'noOfPallet')">
                                    {{$t('loading.detail.outboundList_Total')}}
                                    <i v-if="sortedBy == 'noOfPallet'" :class="sortIcon()"></i>
                                </th>
                                <th></th>
                            </tr>

                        </thead>
                        <tbody>
                            <tr v-for="(row, index) in details"
                                :key="index"
                                :class="{'selected': isSelected(row)}"
                                @click.prevent.stop="onClick(row)"
                                @dblclick.prevent.stop="goToOutbound(row.outJobNo)">
                                <td>
                                    <input type="checkbox" @click.stop="onClick(row)" v-model="detailsToRemove" :value="row.orderNo" />
                                </td>
                                <td class="num">{{index + 1}}</td>
                                <td @click.right.prevent="copyToClipboard(row.orderNo); ctooltip($event, index)"><div></div>{{row.orderNo}}</td>
                                <td @click.right.prevent="copyToClipboard(row.outJobNo); ctooltip($event, index)">{{row.outJobNo}}</td>
                                <td @click.right.prevent="copyToClipboard(row.supplierID); ctooltip($event, index)">{{row.supplierID}}</td>
                                <td @click.right.prevent="copyToClipboard(row.companyName); ctooltip($event, index)">{{row.companyName}}</td>
                                <td style="color: red; font-weight: bold">{{row.xDock ? "XDock" : ""}}</td>
                                <td @click.right.prevent="copyToClipboard(row.etd); ctooltip($event, index)">
                                    <date-time :value="row.etd" nbsp />
                                </td>
                                <td @click.right.prevent="copyToClipboard(row.commInvNo); ctooltip($event, index)">{{row.commInvNo}}</td>
                                <td @click.right.prevent="copyToClipboard(row.addedDate); ctooltip($event, index)">
                                    <date :value="row.addedDate" nbsp />
                                </td>
                                <td @click.right.prevent="copyToClipboard($t('outbound.status.' + row.outboundStatus)); ctooltip($event, index)">{{$t('outbound.status.' + row.outboundStatus)}}</td>
                                <td class="num">{{row.noOfPalletsEHP}}</td>
                                <td class="num">{{row.noOfPalletsSupplier}}</td>
                                <td class="num">{{row.noOfPallet}}</td>
                                <td class="action-icon">
                                    <i class="las la-search" @click.prevent.stop="goToOutbound(row.outJobNo)"></i>
                                    <!--<button class="btn btn-primary btn-sm" @click.prevent.stop="goToOutbound(row.outJobNo)">{{$t('loading.operation.view')}}</button>-->
                                </td>
                            </tr>
                        </tbody>
                    </table>

                    <div class="cursor-tooltip" style="position:absolute" id="tooltip" v-show="cursorTooltipOn">{{$t('operation.general.copied')}}</div>

                    <!-- Butons -->
                    <div class="row mt-3">
                        <div class="col-md-6 text-start">
                            <button class="btn btn-sm btn-primary me-2" @click="add()" :disabled="header.status == 7 || header.status == 5">
                                <i class="las la-plus-circle"></i> {{$t('loading.operation.add')}}
                            </button>
                            <div style="display: inline-block" v-tooltip="detailsToRemove.length ? null : $t('loading.operation.selectDetailsToRemove')">
                                <button @click="remove()"
                                        class="btn btn-sm btn-danger"
                                        :disabled="(header.status == 7 || header.status == 5) || (header.status != 7 && header.status != 5 && detailsToRemove.length == 0)">
                                    <i class="las la-trash-alt"></i> {{$t('loading.operation.remove')}}
                                </button>
                            </div>
                        </div>
                        <div class="col-md-6">

                        </div>
                    </div>

                </div>
            </div>

        </div>

        <!-- Features -->
        <pdf-preview v-if="modal == 'report'" :name="pdfName" :pdf="pdf" @close="pdfName = pdf = null; modal = null"></pdf-preview>
        <add-detail-feature v-if="modal == 'add'" :header="header" @done="modal = null, refresh()" @close="modal = null"></add-detail-feature>
        <confirm-feature v-if="modal == 'confirm'" :jobNo="header.jobNo" @done="modal = null, refresh()" @close="modal = null"></confirm-feature>
        <cancel-feature v-if="modal == 'cancel'" :jobNo="header.jobNo" @done="modal = null, refresh()" @close="modal = null"></cancel-feature>
        <confirm v-if="modal == 'request_picking'" @close="modal = null" @ok="releaseForPicking();">
            {{ $t('loading.operation.areYouSureYouWantToRequestPicking') }}
        </confirm>
    </div>

</template>
<script>
    import LoadingHeader from './loading/Header'
    import AddDetailFeature from './loading/AddDetail.vue'
    import ConfirmFeature from './loading/Confirm'
    import CancelFeature from './loading/Cancel'
    import Date from '@/widgets/Date'
    import DateTime from '@/widgets/DateTime'
    import PDFPreview from '@/widgets/PDFPreview'
    import loadingColor from '@/service/loading_color.js'
    import { setIntervalAsync } from 'set-interval-async/dynamic'
    import { clearIntervalAsync } from 'set-interval-async'
    import { defineComponent } from 'vue';
import { onRouteExit } from '@/route-exit-jobs'
export default defineComponent({
        components: {CancelFeature, ConfirmFeature, LoadingHeader, AddDetailFeature, Date, DateTime, 'pdf-preview': PDFPreview },
        props: ['jobNo'],
        async created() {
            this.lockingMesage = this.$t('document.locking')
            const lockJob = async() => {
                const r = await this.$dataProvider.locks.tryLock(this.jobNo, "Loading");
                if (r) {
                    this.changed = false;
                    this.locked = true;
                }
                else {
                    this.locked = false;
                    this.changed = false;
                    const r1 = await this.$dataProvider.locks.getLock(this.jobNo)
                    this.lockingMesage = this.$t('document.locked', { userCode: r1.userCode})
                }
                return r;
            }
            if (await lockJob()) this.refresh();
            this.lockInterval = setIntervalAsync(lockJob, 30000)
            onRouteExit(this.$route.path, async () => {
                if (this.lockInterval) await clearIntervalAsync(this.lockInterval);
                await this.$dataProvider.locks.tryUnlock(this.jobNo)
            })
        },
        data() {
            return {
                lockInterval: null,
                pdfName: null,
                pdf: null,
                header: null,
                details: [],
                errors: [],
                modal: null,
                alert: null,
                detailsToRemove: [],
                statusColor: null,
                locked: false,
                lockingMesage: "",
                incompleteILogOutbounds: [],
                selectAll: false,
                sortedBy: "",
                cursorTooltipOn: false
            }
        },
        methods: {
            refresh() {
                this.$dataProvider.loadings.getLoading(this.jobNo)
                    .then(resp => {
                        this.header = resp
                    this.statusColor = loadingColor(this.header)
                })
                this.$dataProvider.loadings.getLoadingDetails(this.jobNo).then(resp => {
                    this.details = resp
                })
                this.detailsToRemove = [];
                this.$dataProvider.ilogIntegration.isILogEnabled().then(x => {
                    if (x){
                        this.$dataProvider.ilogIntegration.incompleteILogOutbounds(this.jobNo).then(outbounds => {
                            this.incompleteILogOutbounds = outbounds
                        })
                    }
                });
            },
            goToOutbound(jobNo) {
                this.$router.push({ name: 'outbound_detail', params: { jobNo } })
            },
            save() {
                this.errors = []
                return this.$dataProvider.loadings.patchLoading(this.jobNo, this.header).then(() => {
                    //this.$emit('done', resp.data)
                    this.statusColor = loadingColor(this.header)
                    this.$notify({
                        title: this.$t('success.Done'),
                        text: this.$t('success.Saved'),
                        type: 'success',
                        group: 'temporary'
                    })
                }).catch(e => {
                    if (e.response && e.response.data.errors) {
                        this.errors = e.response.data.errors
                    }
                })
            },
            confirm() {
                this.errors = []
                this.$dataProvider.loadings.patchLoading(this.jobNo, this.header).then(() => {
                    this.statusColor = loadingColor(this.header)
                    this.modal = 'confirm'
                }).catch(e => {
                    if (e.response && e.response.data.errors) {
                        this.errors = e.response.data.errors
                    }
                })
            },
            cancel() {
                this.modal = 'cancel'
            },
            remove() {
                this.errors = []
                this.$dataProvider.loadings.deleteLoadingDetails(this.jobNo, this.detailsToRemove).then(() => {
                    this.$notify({
                        title: this.$t('success.Done'),
                        text: this.$t('success.ItemsDeleted'),
                        type: 'success',
                        group: 'temporary'
                    })
                    this.refresh()
                }).finally(() => this.detailsToRemove = [])
            },
            add() {
                this.modal = 'add'
            },
            isSelected(row) {
                return this.detailsToRemove.includes(row.orderNo)
            },
            onClick(row) {
                var index = this.detailsToRemove.indexOf(row.orderNo);
                if (index !== -1) {
                    this.detailsToRemove.splice(index, 1);
                }
                else {
                    this.detailsToRemove.push(row.orderNo);
                }
                this.selectAll = this.detailsToRemove.length == this.details.length ? true : false;
            },
            selectDeselectAll() {
                if (this.detailsToRemove.length < this.details.length) {
                    this.detailsToRemove = [];
                    this.details.forEach((detail) => { this.detailsToRemove.push(detail.orderNo) });
                } else {
                     this.detailsToRemove = [];
                }
            },
            async releaseForPicking() {
                const resp = await this.$dataProvider.ilogIntegration.releaseLoadingForPicking(this.jobNo);
                resp.finally(this.modal = null);
                if (resp.length === 0){
                    this.$notify({
                        title: this.$t('error.CouldNotCreatePRQ'),
                        text: this.$t('error.NothingToSendToiLog'),
                        type: 'error',
                        group: 'permanent'
                    })
                }
                else {
                    this.$notify({
                        title: this.$t('success.Done'),
                        text: `Created ${resp.length} requests. ${resp.reduce((a, x) => a + x.palletsCount, 0)} pallets requested in total.`,
                        type: 'success',
                        group: 'temporary'
                    })
                }
            },
            report(type) {
                this.modal = 'report'
                //todo move this to data provider
                this.$dataProvider.apiAxios.get(`loadings/get${type}`,
                {
                    params: {
                        jobNo: this.jobNo
                    },
                    responseType: 'blob',
                    headers: {
                        Accept: 'application/pdf'
                    }
                }).then(resp => {
                    this.pdf = resp.data;
                    let headerLine = resp.headers['Content-Disposition'] || resp.headers['content-disposition'];
                    const regExpFilename = /filename="?(?<filename>[^;"]*)/;
                    this.pdfName = regExpFilename.exec(headerLine)?.groups?.filename ?? null;
                }).catch((e) => {
                    this.$dataProvider.errorHandlers.reports(e)
                    this.modal = this.pdf = this.pdfName = null
                });
            },
            sort(selector, by, mode) {
                if (mode) this.sortedAsc = mode == 'asc';
                else if (this.sortedBy != by) this.sortedAsc = true;
                else this.sortedAsc = !this.sortedAsc;
                this.sortedBy = by;
                this.details = this.details.sort((a, b) => {
                    if (typeof (selector(a)) === 'string' || selector(a) instanceof String) {
                        return selector(a).localeCompare(selector(b)) * (this.sortedAsc ? 1 : -1);
                    } else {
                        var x = selector(a); var y = selector(b); var d = this.sortedAsc ? -1 : 1;
                        return ((x < y) ? d : ((x > y) ? (-d) : 0));
                    }
                });
            }, sortIcon() {
                return this.sortedAsc ? 'las la-sort-amount-up-alt' : 'las la-sort-amount-down';
            },
            copyToClipboard(text) {
                this.cursorTooltipOn = true;
                const textArea = document.createElement("textarea");
                textArea.value = text;
                document.body.appendChild(textArea);
                textArea.focus();
                textArea.select();
                try {
                    document.execCommand('copy');
                } catch (err) {
                    console.error('Unable to copy to clipboard', err);
                }
                document.body.removeChild(textArea);
            },
            ctooltip(event,line) {
                let ttip = document.getElementById("tooltip");
                ttip.style.left = event.clientX - 40 + 'px';
                ttip.style.top = (((line + 1) * 29) + 3) + 'px';
                setTimeout(() => { this.cursorTooltipOn = false; }, "1000");
            }
        }
    })
</script>
<style lang="scss">
    .tabs {
        .tab-content {
            max-height: 400px;
            overflow: auto;
        }
    }

    h1 {
        padding-bottom: 20px;
    }

    .loading-detail-title {
        padding-bottom: 3px;
        margin-bottom: 10px;
        padding-left: 10px;
        border-bottom: 15px solid;
    }

    .loading-detail-table {
        tr {
            border: none;
            cursor: pointer;

            &.selected {
                background-color: #e0e0e0;

                &:hover {
                    background-color: #b5b5b5;
                }
            }
        }
    }
    .loading-details-table{
        position: relative;
    }
    .cursor-tooltip {
        position: absolute;
        padding: 5px 10px;
        background-color: forestgreen;
        color: #fff;
        border-radius: 3px;
    }
</style>