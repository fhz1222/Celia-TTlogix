<script lang="ts" setup>

    import bus from '@/event_bus.js'
    // import vue stuff
    import { useStore } from '@/store';
    import { computed, ref, onActivated, onDeactivated } from 'vue';
    import { ActionTypes } from '@/store/action-types';
    import { useI18n } from 'vue-i18n';

    // import types
    import { MenuItem, TableColumn } from '@/store/commonModels/config';
    import { Modal } from '@/store/commonModels/modal';
    import { StockTransferReversal } from '@/store/models/stockTransferReversal';
    import { StockTransferReversalFilters } from '@/store/models/stockTransferReversalFilters';

    // import widgets and modals
    import TableSortFilterHeader from '@/widgets/TableSortFilterHeader.vue';
    import Paging from '@/widgets/Paging.vue';
    import AddStockTransferReversal from './modals/AddStockTransferReversal.vue'
    import ProcessingPopup from '@/widgets/ProcessingPopup.vue';
    import SectionMenu from '@/widgets/SectionMenu.vue';
    import ConfirmPopup from '@/widgets/ConfirmPopup.vue';
    
    // import common logic
    import * as pagingFn from '@/store/commonFunctions/paging';
    import * as sortingFn from '@/store/commonFunctions/sorting';
    import * as filteringFn from '@/store/commonFunctions/filtering';
    import * as miscFn from '@/store/commonFunctions/miscellaneous';
    import * as routerFn from '@/router';

    // init vue stuff
    const store = useStore();
    const { t } = useI18n({ useScope: 'global' });

    // local vars
    const filters_STR = ref(new StockTransferReversalFilters());
    filters_STR.value.whsCode = store.state.whsCode;
    const list_STR = computed(() => store.state.stockTransferReversalList);
    const customersDict: { [id: string]: string } = {};
    const loadingList = ref(false);
    const getStockTransferReversalStatus = store.getters.getStockTransferReversalStatus;
    
    // functions
    const reloadList = async () => {
        loadingList.value = true;
        if (typeof (filters_STR.value.statuses) != 'object') {
            if (filters_STR.value.statuses == '10') filters_STR.value.statuses = null;
            if (filters_STR.value.statuses == '2') filters_STR.value.statuses = [0, 1];
        }
        await store.dispatch(ActionTypes.STR_LOAD_LIST, { filters: filters_STR.value});
        loadingList.value = false;
    }
    const goToReversal = (jobNo: string) => { 
        goTo('stockTransferReversal-detail', { jobNo: jobNo } );
    };
    const canCancel = (rev: StockTransferReversal) => {
        return rev.status < 2;
    };

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const waitModal = ref(new Modal(500, t('generic.processing'), false));
    const confirmModal = ref(new Modal(600, t('generic.pleaseConfirm'), false));
    
    // Menu Action
    const menuAction = (menuFunction: Function) => { menuFunction() };
    // GoTo
    const goTo = (state: string, params?:any) => { return routerFn.goTo(state, params); }
</script>
<template>
    <div>
        <div class="row mt-4 pb-2 px-3">
            <div class="col-md-6">
                <div class="card">
                    <div class="card-header"><h5>Customer modules</h5></div>
                    <div class="card-body">
                        <button class="btn btn-primary" @click="goTo('company-profile')"><i class="las la-industry"></i> Company Profile</button>
                        <button class="btn btn-primary ms-4" @click="goTo('customer')"><i class="las la-user-tie"></i> Customer</button>
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="card">
                    <div class="card-header"><h5>System Registration</h5></div>
                    <div class="card-body">
                        <button class="btn btn-primary" @click="goTo('sato-printer')"><i class="las la-qrcode"></i> SATO Printer</button>
                    </div>
                </div>
            </div>
        </div>
        <div class="row mt-3 px-3">
            <div class="col-md-12">
                <div class="card">
                    <div class="card-header"><h5>Application Registration</h5></div>
                    <div class="card-body">
                        <button class="btn btn-primary" @click="goTo('area')"><i class="las la-map-marked-alt"></i> Area</button>
                        <button class="btn btn-primary ms-3" @click="goTo('area-type')"><i class="las la-map-marker"></i> Area Type</button>
                        <button class="btn btn-primary ms-3" @click="goTo('control-code')"><i class="las la-industry"></i> Control Code</button>
                        <button class="btn btn-primary ms-3" @click="goTo('location')"><i class="las la-pallet"></i> Location</button>
                        <button class="btn btn-primary ms-3" @click="goTo('product-code')"><i class="las la-boxes"></i> Product Code</button>
                        <button class="btn btn-primary ms-3" @click="goTo('package-type')"><i class="las la-box"></i> Package Type</button>
                        <button class="btn btn-primary ms-3" @click="goTo('uom')"><i class="las la-ruler-horizontal"></i> UOM</button>
                        <button class="btn btn-primary ms-3" @click="goTo('warehouse')"><i class="las la-warehouse"></i> Warehouse</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <processing-popup :modalBase=waitModal />
    <confirm-popup :modalBase=confirmModal @close="closeModal(confirmModal)" />
</template>