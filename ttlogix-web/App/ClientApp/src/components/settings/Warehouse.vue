<script lang="ts" setup>

    import bus from '@/event_bus.js'
    // import vue stuff
    import { useStore } from '@/store';
    import { computed, ref, onActivated, onDeactivated, watch } from 'vue';
    import { ActionTypes } from '@/store/action-types';
    import { useI18n } from 'vue-i18n';
    import { useRoute } from 'vue-router';

    // import types
    import { MenuItem, TableColumn } from '@/store/commonModels/config';
    import { Modal } from '@/store/commonModels/modal';
    import { Warehouse } from '../../store/models/settings/warehouse';
    import { WarehouseFilters } from '../../store/models/settings/warehouseFilters';

    // import widgets and modals
    import ProcessingPopup from '@/widgets/ProcessingPopup.vue';
    import SectionMenu from '@/widgets/SectionMenu.vue';
    import TableSortFilterHeader from '@/widgets/TableSortFilterHeader.vue';
    import ModifyWarehousePopup from './modals/ModifyWarehouse.vue'

    // import common logic
    import * as sortingFn from '@/store/commonFunctions/sorting';
    import * as filteringFn from '@/store/commonFunctions/filtering';
    import * as miscFn from '@/store/commonFunctions/miscellaneous';
    import * as routerFn from '@/router';

    // init vue stuff
    const store = useStore();
    const route = useRoute();
    const { t } = useI18n({ useScope: 'global' });

    // local vars
    const list_WHS = computed(() => store.state.settings.warehouse);
    const filters_WHS = ref(new WarehouseFilters());
    const loadingList = ref(false);
    const getWarehouseStatus = store.getters.getWarehouseStatus;
    const getWarehouseType = store.getters.getWarehouseType;

    // functions
    const reloadList = async () => {
        loadingList.value = true;
        filters_WHS.value.status = filters_WHS.value.status == 3 ? null : filters_WHS.value.status;
        filters_WHS.value.type = filters_WHS.value.type == 3 ? null : filters_WHS.value.type;
        await store.dispatch(ActionTypes.SETT_LOAD_WAREHOUSE_LIST, { filters: filters_WHS.value })
            .catch(() => loadingList.value = false);
        loadingList.value = false;
    }
    const modifyWarehouse = (warehouse: Warehouse | null = null) => {
        modifyWarehouseModal.value.on = true;
        modifyWarehouseModal.value.title = !warehouse ? t('settings.warehouse.addWarehouse') : t('settings.warehouse.modifyWarehouse');
        modifyWarehouseModal.value.customProps = warehouse?.code;
    }
    const addWarehouse = async (updatingWarehouse: Warehouse, addNext: boolean) => {
        if (!addNext) closeModal(modifyWarehouseModal.value);
        waitModal.value.on = true;
        updatingWarehouse.type = updatingWarehouse.type ?? 1;
        updatingWarehouse.status = 1;
        await store.dispatch(ActionTypes.SETT_ADD_WAREHOUSE, { warehouse: updatingWarehouse }).catch(() => closeModal(waitModal.value));
        closeModal(waitModal.value);
        reloadList();
    }
    const updateWarehouse = async (updatingWarehouse: Warehouse) => {
        closeModal(modifyWarehouseModal.value);
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.SETT_UPDATE_WAREHOUSE, { warehouse: updatingWarehouse }).catch(() => closeModal(waitModal.value));
        closeModal(waitModal.value);
        reloadList();
    }
    const toggleWarehouse = async (code: string) => {
        waitModal.value.on = true;
        let updatedWarehouse = new Warehouse();
        await store.dispatch(ActionTypes.SETT_LOAD_WAREHOUSE, { code: code })
                   .then((whs) => updatedWarehouse = whs)
                   .catch(() => closeModal(waitModal.value));
        updatedWarehouse.status = updatedWarehouse.status == 0 ? 1 : 0;
        await updateWarehouse(updatedWarehouse);
        closeModal(waitModal.value);
    }

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const waitModal = ref(new Modal(500, t('generic.processing'), false));
    const modifyWarehouseModal = ref(new Modal(700, '', true));

    // Menu Action
    const menuAction = (menuFunction: Function) => { menuFunction() };
    // Sorting
    const sort = (by: string) => { sortingFn.sort(by, filters_WHS.value.sorting); reloadList(); };
    // Filtering
    const applyFilters = (column: string, value: any, load: boolean) => { filteringFn.applyFilters(column, value, filters_WHS.value); if (load) reloadList(); };

    // initially launched functions
    await store.dispatch(ActionTypes.LOAD_DICT);
    const warehouseStatus = store.state.config.dictionaries.warehouseStatus;
    const warehouseType = store.state.config.dictionaries.warehouseType;

    // section Menu
    const menuItems_WHS: Array<MenuItem> = [
        { state: null, stateParams: null, title: 'addNew', function: () => modifyWarehouse(), icon: 'la-plus-circle' },
        { state: null, stateParams: null, title: 'refresh', function: reloadList, icon: 'la-redo-alt', float: 'end' },
    ];

    // table related stuff
    const columns_WHS: Array<TableColumn> = [
        { alias: 'code',  columnType: 'string',  dropdown: null, sortable: true, f1: '', f2: null, resetTo: null },
        { alias: 'name',  columnType: 'string',  dropdown: null, sortable: true, f1: '', f2: null, resetTo: null },
        { alias: 'type',  columnType: 'dropdown',dropdown: warehouseType, sortable: true, f1: '3', f2: null, resetTo: '3' },
        { alias: 'status',columnType: 'dropdown',dropdown: warehouseStatus, sortable: true, f1: '3', f2: null, resetTo: '3' }
    ];

    // init the screen
    onActivated(() => {
        reloadList();
        bus.on("refresh", reloadList)
    });
    onDeactivated(() => bus.off("refresh", reloadList))
</script>
<template>
    <section-menu :menu-items="menuItems_WHS" t_orig="inboundReversal.operation" @action="menuAction" />
    <div class="container-fluid">
        <div class="row my-3">
            <div class="col-md-12">
                <table class="table table-striped table-hover filtered-sorted-table">
                    <table-sort-filter-header :columns=columns_WHS
                                              :sorting=filters_WHS.sorting
                                              t_core="settings.warehouse"
                                              @sort="sort"
                                              @apply-filters="applyFilters"
                                              :sortable=true
                                              :filtreable=true />
                    <tbody v-if="!loadingList && list_WHS.length > 0">
                        <tr v-for="w, i in list_WHS" :key="i" class="table-row">
                            <td>{{w.code}}</td>
                            <td>{{w.name}}</td>
                            <td>{{getWarehouseType(w.type)}}</td>
                            <td>{{getWarehouseStatus(w.status)}}</td>
                            <div class="table-buttons" v-if="w.type != 0">
                                <button class="btn btn-primary me-2 btn-sm" @click="modifyWarehouse(w)"><i class="las la-pen"></i> {{t('modify')}}</button>
                                <button class="btn btn-sm btn-primary me-2" @click="toggleWarehouse(w.code)"><i :class="['las',w.status == 1 ? 'la-ban' : 'la-redo-alt']"></i> {{w.status == 1 ? t('operation.general.deactivate') : t('operation.general.reactivate')}}</button>
                            </div>
                        </tr>
                    </tbody>
                </table>
                <div class="text-center pt-5 main-action-icons-small" v-if="!loadingList && list_WHS.length == 0">
                    <h5><i class="las la-ban"></i> {{t('generic.noRecords')}}</h5>
                </div>
                <div v-if="loadingList" class="bars1-wrapper">
                    <div id="bars1">
                        <span></span>
                        <span></span>
                        <span></span>
                        <span></span>
                        <span></span>
                    </div>
                    <h5>{{t('generic.loading')}}</h5>
                </div>
            </div>
        </div>
    </div>
    <modify-warehouse-popup :modalBase="modifyWarehouseModal" @close="closeModal(modifyWarehouseModal)" @add="addWarehouse" @update="updateWarehouse" />
    <processing-popup :modalBase=waitModal />
</template>