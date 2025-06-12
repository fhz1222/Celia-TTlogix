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
    import { ProductCode } from '../../store/models/settings/productCode';
    import { ProductCodeFilters } from '../../store/models/settings/productCodeFilters';

    // import widgets and modals
    import ProcessingPopup from '@/widgets/ProcessingPopup.vue';
    import SectionMenu from '@/widgets/SectionMenu.vue';
    import TableSortFilterHeader from '@/widgets/TableSortFilterHeader.vue';
    import ModifyProductCodePopup from './modals/ModifyProductCode.vue'

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
    const list_PC = computed(() => store.state.settings.productCode);
    const filters_PC = ref(new ProductCodeFilters());
    const loadingList = ref(false);
    const getProductCodeStatus = store.getters.getProductCodeStatus;
    const getProductCodeType = store.getters.getProductCodeType;

    // functions
    const reloadList = async () => {
        loadingList.value = true;
        filters_PC.value.status = filters_PC.value.status == 3 ? null : filters_PC.value.status;
        filters_PC.value.type = filters_PC.value.type == 3 ? null : filters_PC.value.type;
        await store.dispatch(ActionTypes.SETT_LOAD_PC_LIST, { filters: filters_PC.value })
            .catch(() => loadingList.value = false);
        loadingList.value = false;
    }
    const modifyProductCode = (productCode: ProductCode | null = null) => {
        modifyProductCodeModal.value.on = true;
        modifyProductCodeModal.value.title = !productCode ? t('settings.productCode.addProductCode') : t('settings.productCode.modifyProductCode');
        modifyProductCodeModal.value.customProps = { ...productCode };
    }
    const updateProductCode = async (updatingProductCode: ProductCode, addNext: boolean = false) => {
        if (!addNext) closeModal(modifyProductCodeModal.value);
        waitModal.value.on = true;
        if (!updatingProductCode.code) {
            updatingProductCode.type = updatingProductCode.type ?? 1;
            await store.dispatch(ActionTypes.SETT_ADD_PC, { productCode: updatingProductCode }).catch(() => waitModal.value.on = false);
        } else {
            await store.dispatch(ActionTypes.SETT_UPDATE_PC, { productCode: updatingProductCode }).catch(() => waitModal.value.on = false);
        }
        closeModal(waitModal.value);
        reloadList();
    }
    const toggleProductCode = async (productCode: ProductCode) => {
        waitModal.value.on = true;
        const updatedProductCode = { ...productCode };
        updatedProductCode.status = updatedProductCode.status == 0 ? 1 : 0;
        await updateProductCode(updatedProductCode)
        closeModal(waitModal.value);
    }

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const waitModal = ref(new Modal(500, t('generic.processing'), false));
    const modifyProductCodeModal = ref(new Modal(500, '', true));

    // Menu Action
    const menuAction = (menuFunction: Function) => { menuFunction() };
    // Sorting
    const sort = (by: string) => { sortingFn.sort(by, filters_PC.value.sorting); reloadList(); };
    // Filtering
    const applyFilters = (column: string, value: any, load: boolean) => { filteringFn.applyFilters(column, value, filters_PC.value); if (load) reloadList(); };

    // initially launched functions
    await store.dispatch(ActionTypes.LOAD_DICT);
    const productCodeStatus = store.state.config.dictionaries.productCodeStatus;
    const productCodeType = store.state.config.dictionaries.productCodeType;

    // section Menu
    const menuItems_PC: Array<MenuItem> = [
        { state: null, stateParams: null, title: 'addNew', function: () => modifyProductCode(), icon: 'la-plus-circle' },
        { state: null, stateParams: null, title: 'refresh', function: reloadList, icon: 'la-redo-alt', float: 'end' },
    ];

    // table related stuff
    const columns_PC: Array<TableColumn> = [
        { alias: 'code', columnType: 'string', dropdown: null, sortable: true, f1: '', f2: null, resetTo: null },
        { alias: 'name', columnType: 'string', dropdown: null, sortable: true, f1: '', f2: null, resetTo: null },
        { alias: 'type', columnType: 'dropdown', dropdown: productCodeType, sortable: true, f1: '3', f2: null, resetTo: '3' },
        { alias: 'status', columnType: 'dropdown', dropdown: productCodeStatus, sortable: true, f1: '3', f2: null, resetTo: '3' }
    ];

    // init the screen
    onActivated(() => {
        reloadList();
        bus.on("refresh", reloadList)
    });
    onDeactivated(() => bus.off("refresh", reloadList))
</script>
<template>
    <section-menu :menu-items="menuItems_PC" t_orig="inboundReversal.operation" @action="menuAction" />
    <div class="container-fluid">
        <div class="row my-3">
            <div class="col-md-12">
                <table class="table table-striped table-hover filtered-sorted-table">
                    <table-sort-filter-header :columns=columns_PC
                                              :sorting=filters_PC.sorting
                                              t_core="settings.productCode"
                                              @sort="sort"
                                              @apply-filters="applyFilters"
                                              :sortable=true
                                              :filtreable=true />
                    <tbody v-if="!loadingList && list_PC.length > 0">
                        <tr v-for="pc, i in list_PC" :key="i" class="table-row">
                            <td>{{pc.code}}</td>
                            <td>{{pc.name}}</td>
                            <td>{{getProductCodeType(pc.type)}}</td>
                            <td>{{getProductCodeStatus(pc.status)}}</td>
                            <div class="table-buttons" v-if="pc.type != 0">
                                <button class="btn btn-primary me-2 btn-sm" @click="modifyProductCode(pc)"><i class="las la-pen"></i> {{t('modify')}}</button>
                                <button class="btn btn-sm btn-primary me-2" @click="toggleProductCode(pc)"><i :class="['las',pc.status == 1 ? 'la-ban' : 'la-redo-alt']"></i> {{pc.status == 1 ? t('operation.general.deactivate') : t('operation.general.reactivate')}}</button>
                            </div>
                        </tr>
                    </tbody>
                </table>
                <div class="text-center pt-5 main-action-icons-small" v-if="!loadingList && list_PC.length == 0">
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
    <modify-product-code-popup :modalBase="modifyProductCodeModal" @close="closeModal(modifyProductCodeModal)" @proceed="updateProductCode" />
    <processing-popup :modalBase=waitModal />
</template>