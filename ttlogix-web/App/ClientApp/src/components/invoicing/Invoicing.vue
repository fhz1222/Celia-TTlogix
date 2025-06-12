<script lang="ts" setup>
    // import vue stuff
    import { useStore } from '../../store';
    import { computed, ref } from 'vue';
    import { ActionTypes } from '../../store/action-types';
    import moment from 'moment';

    // import types
    import { Customer } from '../../store/models/customer';
    import { InvoiceBatchFilters } from '../../store/models/invoiceBatchFilters';
    import { Modal } from '../../store/commonModels/modal';

    // import widgets and modals
    import ProcessingPopup from '../../widgets/ProcessingPopup.vue';
    import ConfirmPopup from '../../widgets/ConfirmPopup.vue';
    import AwaitingButton from '../../widgets/AwaitingButton.vue';
    import EditModal from './EditModal.vue';
    import PDFPreview from '../../widgets/PDFPreview.vue'

    // import common logic
    import { useI18n } from 'vue-i18n';
    import { saveBlob } from '../../service/file_service';

    // init vue stuff
    const store = useStore();
    const {t} = useI18n({useScope: 'global'});

    // local vars
    const filters_INVC = ref(new InvoiceBatchFilters());
    const filter_DDST = ref('');
    const list_INVC = computed(() => store.state.invoiceRequestList);
    const flow = computed(() => store.state.invoiceRequestFlow);
    const factories = computed(() => store.state.factories);
    const loadingList = ref(false);
    const priceManagement = ref(false);
    const customClearance = ref(false);
    const factoryException = ref(false);

    // Functions 
    const reloadList = async () => {
        if (!filters_INVC.value.factoryID) return;
        filters_INVC.value.DDSTNumber = filter_DDST.value;
        expanded.value = {};
        loadingList.value = true;
        await store.dispatch(ActionTypes.INVC_LOAD_LIST, { filters: filters_INVC.value }).catch((e: Error) => {
            loadingList.value = false;
        });
        loadingList.value = false;
    };
    const confirm = async (batchId: number) => {
        const v = list_INVC.value.items.find(x => x.batchId == batchId)
        const confirm = () => {
            confirmModal.value.title = 'Send an invoice';
            confirmModal.value.fnMod.message = `Do you really want to confirm batch ${v?.batchNumber} ?`
            confirmModal.value.on = true;
            confirmModal.value.fnMod.callback = async () => { await confirmExecute(batchId) };
        }
        if (flow.value == 'CustomsClearance' && !v?.truckDepartureHour) {
            editingModal.value = {
                batchId,
                comment: v?.comment ?? "",
                truckDeparture: v?.truckDepartureHour ?? 9,
                disabled: false,
                onSave: async (batchId, comment, truckDeparture, callback) => {
                    await saveAction(batchId, comment, truckDeparture)
                    callback()
                    confirm()
                }
            }
        }
        else {
            confirm()
        }


    };
    const confirmExecute = async (batchId: number) => {
        closeModal(confirmModal.value);
        waitModal.value.on = true;
        waitModal.value.title = 'Working...';
        await store.dispatch(ActionTypes.CONFIRM_INVC, { batchId }).catch((e: Error) => {
            waitModal.value.on = false;
        });
        closeModal(waitModal.value)
        reloadList();
    };
    const reject = async (batchId: number) => {
        const v = list_INVC.value.items.find(x => x.batchId == batchId)
        confirmModal.value.title = 'Rejecting an invoice';
        confirmModal.value.fnMod.message = `Do you really want to reject batch ${v?.batchNumber} ?`
        confirmModal.value.on = true;
        confirmModal.value.fnMod.callback = async () => { await rejectExecute(batchId) };
    };
    const rejectExecute = async (batchId: number) => {
        closeModal(confirmModal.value);
        waitModal.value.on = true;
        waitModal.value.title = 'Working...';
        await store.dispatch(ActionTypes.REJECT_INVC, { batchId }).catch((e: Error) => {
            waitModal.value.on = false;
        });
        closeModal(waitModal.value)
        reloadList();
    };
    const changeFactory = (customer:Customer) => {
        filters_INVC.value.factoryID = customer?.code;
        factoryException.value = priceManagement.value == false && customClearance.value == false ? true : false;
        reloadList();
    };
    const downloadInvoice = async (invoiceId: number) => {
        const resp = await store.dispatch(ActionTypes.INVC_GET_INVOICE_PDF, { invoiceId: invoiceId.toString() });
        const headerLine = resp.headers['Content-Disposition'] || resp.headers['content-disposition'];
        const regExpFilename = /filename="?(?<filename>[^;"]*)/;
        const fileName = regExpFilename.exec(headerLine)?.groups?.filename ?? null;
        if (fileName?.endsWith(".pdf")){
            pdfModal.value = {pdfName: fileName, pdf: resp.data}
        }	
        else {
            saveBlob(resp.data, fileName || 'file');
        }
    }
        
    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const waitModal = ref(new Modal(500, '', false));
    const confirmModal = ref(new Modal(600, 'Please confirm', false));
    const pdfModal = ref<{pdfName: string, pdf: Blob}|null>();

    // initially launched functions
    store.dispatch(ActionTypes.LOAD_DICT);
    store.dispatch(ActionTypes.INVC_LOAD_FACTORIES);
    const expanded = ref<{[key: string]: boolean}>({})

    const loadMore = async () => {
        await store.dispatch(ActionTypes.INVC_LOAD_MORE, { filters: filters_INVC.value});
    }

    const editingModal = ref<{
        batchId: number,
        comment: string,
        truckDeparture: number,
        disabled: boolean,
        onSave: (batchId: number, comment: string, truckDeparture: number, callback: () => void) => Promise<void>
    } | null>(null);

    const edit = (batchId: number, disabled: boolean) => {
        const v = list_INVC.value.items.find(x => x.batchId == batchId)
        editingModal.value = {
            batchId,
            comment: v?.comment ?? "",
            truckDeparture: v?.truckDepartureHour ?? 9,
            disabled,
            onSave: async (batchId, comment, truckDeparture, callback) => {
                await saveAction(batchId, comment, truckDeparture)
                callback()
                await reloadList()
            }
        }
    }

    var saveAction = async (batchId: number, comment: string, truckDeparture: number) => {
        await store.dispatch(ActionTypes.INVC_UPDATE_BATCH, {batchId, comment, hour: truckDeparture })
        editingModal.value = null
        await reloadList()
    }
</script>
<template>
    <div class="invoicing">
        <div class="row pb-2 pt-2">
            <div class="col-md-2 multiselect-as-select mt-2 ps-4">
                <v-select :options="factories"
                          @update:modelValue="changeFactory"
                          placeholder="Select a factory">
                    <template v-slot:selected-option="option">
                        {{ option.code + ' - ' + option.name }}
                    </template>
                    <template v-slot:option="option">
                        {{option.code}} - {{option.name}}
                    </template>
                </v-select>
            </div>
            <div class="col-md-2 mt-2 ps-4" v-if="filters_INVC.factoryID">
                <div class="d-flex flex-row gap-2 align-items-center">
                    <span class="fw-bold">DD/ST:</span>
                    <input type="text" placeholder="DD/ST Number" class="form-control form-control-sm" v-model="filter_DDST">
                </div>
            </div>
            <div class="col-md-1 mt-2 ps-4" v-if="filters_INVC.factoryID">
                <button class="btn btn-sm btn-primary"
                        @click.stop="reloadList()">
                    Apply Filters
                </button>
            </div>
                <div class="col-md-7 text-end pe-5 main-action-icons">
                    <div class="d-inline-block link-box pointer" v-if="filters_INVC.factoryID">
                        <div @click.stop.prevent="reloadList()" class="text-center">
                            <div><i class="las la-redo-alt"></i></div>
                            <div>{{t('operation.general.refresh')}}</div>
                        </div>
                    </div>
                </div>
            </div>
        <div class="row px-2 mt-4" v-if="filters_INVC.factoryID && !loadingList && list_INVC.items.length != 0">
            <div class="col-md-1 ps-3"><strong>Batch Number</strong></div>
            <div class="col-md-2 text-end"><strong>Supplier</strong></div>
            <div class="col-md-2 text-end"><strong>Total</strong></div>
            <div class="col-md-2 text-end">
                <div class="d-inline-block" v-tooltip="'Earliest ETD in case of multiple DDs'">
                    <strong>Loading ETD</strong>                
                </div>
            </div>
            <div class="col-md-2 text-end"><strong v-if="flow == 'CustomsClearance'">For Customs Agency</strong></div>
            <div class="col-md-3"></div>
        </div>
        <div class="row p-2">
            <div class="col-md-12">
                <div v-if="filters_INVC.factoryID && !loadingList && list_INVC.items.length != 0">
                    <div class="card border mb-2" v-for="iu in list_INVC.items" :key="iu.batchId" @click="expanded[iu.batchId] = !expanded[iu.batchId]" style="cursor: pointer;">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-md-1 pt-1">
                                    <span class="ms-2">
                                        {{ iu.batchNumber }}
                                    </span>
                                </div>
                                <div class="col-md-2 text-end pt-1">
                                    {{ iu.supplierName }}
                                </div>
                                <div class="col-md-2 text-end pt-1">
                                    {{ iu.invoices.reduce((acc, x) => acc + x.value, 0).toFixed(2) }} {{ iu.invoices[0].currency }}
                                </div>
                                <div class="col-md-2 text-end pt-1">
                                    {{ iu.loadingEtd ? moment(iu.loadingEtd).format("DD/MM/YYYY HH:mm") : '' }}
                                </div>
                                <div class="col-md-2 text-end">
                                    <template  v-if="flow == 'CustomsClearance'">
                                        <input type="text" class="form-control form-control-sm d-inline me-2" style="max-width: 3.5rem;" :value="iu.truckDepartureHour ? `${iu.truckDepartureHour.toString().padStart(2, '0')}:00` : ''" disabled />
                                        <button class="btn btn-secondary btn-sm"
                                            @click.stop="edit(iu.batchId, false)">
                                            <i class="las la-clock"></i>
                                            <i v-if="iu.comment" class="las la-comment"></i>
                                            <i v-else class="las la-comment-alt"></i>
                                        </button>
                                    </template>
                                </div>
                                <div class="col-md-3 text-end">
                                    <span class="me-3" :class="iu.status == 'Rejected' ? 'text-danger': 'text-success'" v-if="iu.status == 'Approved' || iu.status == 'Rejected'"><strong>{{ t(`invoicing.status.${iu.status}`) }}</strong></span>
                                    <template v-if="iu.status == 'PendingApproval'">
                                        <button class="btn btn-success btn-sm me-2"
                                                @click.stop="confirm(iu.batchId)">
                                                <i class="la la-check-circle"></i>
                                                Approve
                                            </button>
                                            <button class="btn btn-danger btn-sm me-2"
                                            @click.stop="reject(iu.batchId)">
                                            <i class="la la-times-circle"></i>
                                            Reject
                                        </button>
                                    </template>
                                </div>
                            </div>
                            <div class="row mt-3" v-if="expanded[iu.batchId]" @click.stop="''" style="cursor: default;">
                                <div class="col-md-7">
                                    <div class="ms-2"><strong>Details</strong></div>
                                    <div class="card ms-2">
                                        <div class="card-body bg-light">
                                            <table class="table-stripped w-100 dd-table">
                                                <template v-for="ir in iu.jobs" :key="ir.jobNo">
                                                    <tr>
                                                        <td class="p-0">
                                                            <template v-if="ir.deliveryDocket">
                                                                <strong>Delivery Docket:</strong> {{ ir.deliveryDocket }}, &nbsp;
                                                            </template>
                                                            <strong>Job No:</strong> {{ ir.jobNo }}
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td v-if="ir.details.length > 0">
                                                            <table class="table table-striped table-hover asn-table">
                                                                <thead>
                                                                    <tr>
                                                                        <th>ASN</th>
                                                                        <th>Product Code</th>
                                                                        <th>Qty</th>
                                                                        <th>PO/SA</th>
                                                                        <th>PO/SA Line</th>
                                                                    </tr>
                                                                </thead>
                                                                <tbody>
                                                                    <tr v-for="asn in ir.details" :key="asn.asnNo">
                                                                        <td>{{ asn.asnNo }}</td>
                                                                        <td>{{ asn.productCode }}</td>
                                                                        <td>{{ asn.qty }}</td>
                                                                        <td>{{ asn.poNumber }}</td>
                                                                        <td>{{ asn.poLineNo }}</td>
                                                                    </tr>
                                                                </tbody>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </template>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-5">
                                    <div><strong>Invoices</strong></div>
                                    <div class="card me-2">
                                        <div class="card-body bg-light">
                                            <table class="table table-striped table-hover">
                                                <thead>
                                                    <tr>
                                                        <th>Invoice No</th>
                                                        <th class="text-end">Total Value</th>
                                                        <th class="text-end"></th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <tr v-for="invoice in iu.invoices" :key="invoice.invoiceNumber">
                                                        <td class="pt-2">{{ invoice.invoiceNumber }}</td>
                                                        <td class="text-end pt-2">{{ invoice.value.toFixed(2) }} {{ invoice.currency }}</td>
                                                        <td class="big-glyph text-end">
                                                            <button :disabled="iu.status == 'Rejected'" class="btn btn-sm btn-primary" @click.stop="downloadInvoice(invoice.fileId)">Download <i class="pointer las la-download"></i></button>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="text-center pt-5 main-action-icons-small" v-if="filters_INVC.factoryID && !loadingList && list_INVC.items.length == 0">
                    <h5><i class="las la-ban"></i> No records found :(</h5>
                </div>
                <div class="text-center pt-5 main-action-icons-small" v-if="!filters_INVC.factoryID && !loadingList">
                    <h5><i class="las la-list-alt"></i> Select a customer to load the records</h5>
                </div>
                <div v-if="loadingList" class="bars1-wrapper">
                    <div id="bars1">
                        <span></span>
                        <span></span>
                        <span></span>
                        <span></span>
                        <span></span>
                    </div>
                    <h5>Loading...</h5>
                </div>
            </div>
        </div>

        <div class="text-center mt-3 mb-2">
            <div v-if="filters_INVC.factoryID && !loadingList && list_INVC && list_INVC.items.length < list_INVC.pagination.totalCount">
                <AwaitingButton @proceed="(callback) => { loadMore().finally(callback) }"
                                 :btnText="'operation.dynamicTable.loadMore'"
                                 :btnIcon="'las la-chevron-down'"
                                 :btnClass="'btn-primary'">
                </AwaitingButton>
                <br />
                <span class="me-2" v-html="t('operation.dynamicTable.showingBatch', {dataLength:list_INVC.items.length, total:list_INVC.pagination.totalCount })"></span>
            </div>
        </div>
    </div>
    <processing-popup :modalBase=waitModal />
    <ConfirmPopup :modalBase=confirmModal @close="closeModal(confirmModal)" />
    <EditModal @close="editingModal = null" :data="editingModal"></EditModal>
    <PDFPreview v-if="pdfModal != null" :name="pdfModal.pdfName" :pdf="pdfModal.pdf" @close="pdfModal = null" title="Preview"></PDFPreview>
</template> 