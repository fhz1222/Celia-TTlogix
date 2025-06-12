<script lang="ts" setup>
    // import vue stuff
    import { ref, watch, toRaw } from 'vue';
    import { store } from '@/store';
    import { useI18n } from 'vue-i18n';
    
    // import types
    import { Modal } from '@/store/commonModels/modal';
    import { ActionTypes } from '@/store/action-types';
    import { Error } from '@/store/commonModels/errors';
    import { ReversibleStockTransferItem } from '@/store/models/reversibleStockTransferItem'
    import { ReversibleStockTransferItemFilters, ReversibleStockTransferItemFiltersLocal } from '@/store/models/reversibleStockTransferItemFilters'
    import AwaitingButton from '@/widgets/AwaitingButton.vue'
    import ErrorPopup from '@/widgets/ErrorPopup.vue'
    import { TableColumn } from '@/store/commonModels/config';
    import { Sorting } from '@/store/commonModels/Sorting';

    // import widgets and modals
    import ModalWidget from '@/widgets/Modal3.vue';
    import TableSortFilterHeader from '@/widgets/TableSortFilterHeader.vue';

    // import common logic
    import * as sortingFn from '@/store/commonFunctions/sorting';
    import * as filteringFn from '@/store/commonFunctions/filtering';
    import * as miscFn from '@/store/commonFunctions/miscellaneous';
    import * as routerFn from '@/router';

    // props & emits
    const props = defineProps<{ modalBase: Modal; stfJobNo: string }>();
    const emit = defineEmits<{(e: 'close'): void, (e: 'addItems', stfJobNos:Array<string>): void }>();

    // local vars
    const loading = ref(false);
    const reversibleStockTransferItemsBase = ref(new Array<ReversibleStockTransferItem>());
    const reversibleStockTransferItems = ref(new Array<ReversibleStockTransferItem>());
    const selectedItems = ref(Array<string>());
    const RSIFilters = ref(new ReversibleStockTransferItemFilters());
    const RSIFiltersLocal = ref(new ReversibleStockTransferItemFiltersLocal());
    const { t } = useI18n({ useScope: 'global' });
    const sorting_RSI_ITEMS = ref(new Sorting());

    // functions
    const loadReversibleItems = async () => {
        loading.value = true;
        await store.dispatch(ActionTypes.STR_LOAD_REVERSIBLE_STOCK_TRANSFER_ITEMS, { filters: RSIFilters.value })
            .then((reversibleItems: Array<ReversibleStockTransferItem>) => {
                reversibleStockTransferItems.value = reversibleStockTransferItemsBase.value = reversibleItems;
                loading.value = false;
            })
    }
    const select = (pid: string, selected: boolean) => {
        if (selected) {
            selectedItems.value.push(pid);
        } else {
            var i = selectedItems.value.length
            while (i--) {
                if (selectedItems.value[i]==pid) { 
                    selectedItems.value.splice(i, 1);
                } 
            }
        }
    }
    const proceed = async () => {
        emit('addItems', selectedItems.value);
        resetModal();
    }
    const toggleAllRows = (newState: boolean) => {
        selectedItems.value = [];
        reversibleStockTransferItems.value.forEach((rsti) => { rsti.selected = newState; if (newState) selectedItems.value.push(rsti.pid) });
    }

    // Sorting
    const sort = (by: string, norevert: boolean = false) => {
        sorting_RSI_ITEMS.value.by = by;
        sorting_RSI_ITEMS.value.descending = norevert ? sorting_RSI_ITEMS.value.descending : !sorting_RSI_ITEMS.value.descending;
        reversibleStockTransferItems.value = sortingFn.sortLocally(sorting_RSI_ITEMS.value, reversibleStockTransferItems.value);
    }
    // Filtering
    const applyFilters = (column: string, value: any, load: boolean) => {
        filteringFn.applyFilters(column, value, RSIFiltersLocal.value);
        reversibleStockTransferItems.value = filteringFn.applyFiltersLocally(reversibleStockTransferItemsBase.value, column, value, RSIFiltersLocal.value);
        if (sorting_RSI_ITEMS.value.by) sort(sorting_RSI_ITEMS.value.by,true);
    }

    watch(() => props.modalBase.on, (nVal, oVal) => {
        if (nVal && nVal != oVal) {
            RSIFilters.value.stfJobNo = props.stfJobNo;
            selectedItems.value = new Array<string>();
            loadReversibleItems();
        }
    });
    const cancel = async () => {
        emit('close');
        resetModal();
    }
    const resetModal = () => { reversibleStockTransferItems.value = new Array<ReversibleStockTransferItem>() }

    // table related stuff
    const columns_RSI: Array<TableColumn> = [
        { alias:'',            columnType:'toggleAll',dropdown:null,sortable:false,f1:null,f2:null,resetTo:null },
        { alias:'no',          columnType:null,       dropdown:null,sortable:false,f1:null,f2:null,resetTo:null },
        { alias:'pid',         columnType:'string',   dropdown:null,sortable:true, f1:'',  f2:null,resetTo:null },
        { alias:'productCode', columnType:'string',   dropdown:null,sortable:true, f1:'',  f2:null,resetTo:null },
        { alias:'locationCode',columnType:'string',   dropdown:null,sortable:true, f1:'',  f2:null,resetTo:null },
        { alias:'qty',         columnType:null,       dropdown:null,sortable:true, f1:'',  f2:null,resetTo:null }
    ];

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const errorModal = ref(new Modal(500, 'Error', false));
</script>

<template>
    <modal-widget :modalBase=modalBase @close="emit('close', false)">
        <template name="title" v-slot:title>
            <h5>{{modalBase.title}}</h5>
        </template>
        <template name="body" v-slot:body>
            <div class="row pb-2 px-3">
                <div class="col-md-12 scrollable">
                    <table class="table table-striped table-hover filtered-sorted-table">
                        <table-sort-filter-header :columns=columns_RSI
                                                  :sorting=sorting_RSI_ITEMS
                                                  t_core="stockTransferReversal.details.items.grid"
                                                  @sort="sort"
                                                  @apply-filters="applyFilters"
                                                  @toggleAll="toggleAllRows"
                                                  :sortable=true
                                                  :filtreable=true />
                        <tbody v-if="!loading && reversibleStockTransferItems.length > 0">
                            <tr v-for="revii, i in reversibleStockTransferItems" :key="i" class="table-row" @click="revii.selected = !revii.selected;select(revii.pid, revii.selected)">
                                <td><input type="checkbox" v-model="revii.selected" /></td>
                                <td>{{i + 1}}</td>
                                <td>{{revii.pid}}</td>
                                <td>{{revii.productCode}}</td>
                                <td>{{revii.locationCode}}</td>
                                <td>{{revii.qty}}</td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="text-center pt-5 main-action-icons-small" v-if="!loading && reversibleStockTransferItems.length == 0">
                        <h5><i class="las la-ban"></i> No records found :(</h5>
                    </div>
                    <div v-if="loading" class="bars1-wrapper">
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
        </template>
        <template name="footer" v-slot:footer>
            <div class="row">
                <div class="col-md-12 text-end">
                    <button class="btn btn-sm btn-secondary me-2" @click="cancel">{{ t('operation.general.cancel') }}</button>
                    <!--AwaitingButton @proceed="(callback) => { proceed().finally(callback) }"
                                    :btnText="'operation.general.ok'"
                                    :btnIcon="'las la-disk'"
                                    :btnClass="'btn-primary'"
                                    :enabled="!loading"> 
                    </AwaitingButton-->
                    <button class="btn btn-sm btn-primary me-2" @click="proceed">{{ t('operation.general.add') }}</button>
                </div>
            </div>
        </template>
    </modal-widget>
    <error-popup :modalBase=errorModal @close="closeModal(errorModal)" />
</template>