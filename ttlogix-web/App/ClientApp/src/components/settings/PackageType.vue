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
    import { PackageType } from '../../store/models/settings/packageType';
    import { PackageTypeFilters } from '../../store/models/settings/packageTypeFilters';

    // import widgets and modals
    import ProcessingPopup from '@/widgets/ProcessingPopup.vue';
    import SectionMenu from '@/widgets/SectionMenu.vue';
    import TableSortFilterHeader from '@/widgets/TableSortFilterHeader.vue';
    import ModifyPackageTypePopup from './modals/ModifyPackageType.vue'

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
    const list_PTYP = computed(() => store.state.settings.packageType);
    const filters_PTYP = ref(new PackageTypeFilters());
    const loadingList = ref(false);
    const getPackageTypeStatus = store.getters.getPackageTypeStatus;
    const getPackageTypeType = store.getters.getPackageTypeType;

    // functions
    const reloadList = async () => {
        loadingList.value = true;
        filters_PTYP.value.status = filters_PTYP.value.status == 3 ? null : filters_PTYP.value.status;
        filters_PTYP.value.type = filters_PTYP.value.type == 3 ? null : filters_PTYP.value.type;
        await store.dispatch(ActionTypes.SETT_LOAD_PACKTYPE_LIST, { filters: filters_PTYP.value })
                   .catch(() => loadingList.value = false);
        loadingList.value = false;
    }
    const modifyPackageType = (packageType: PackageType | null = null) => {
        modifyPackageTypeModal.value.on = true;
        modifyPackageTypeModal.value.title = !packageType ? t('settings.packageType.addPackageType') : t('settings.packageType.modifyPackageType');
        modifyPackageTypeModal.value.customProps = { ...packageType };
    }
    const updatePackageType = async (updatingPackageType: PackageType, addNext: boolean = false) => {
        if (!addNext) closeModal(modifyPackageTypeModal.value);
        waitModal.value.on = true;
        if (!updatingPackageType.code) {
            updatingPackageType.type = updatingPackageType.type ?? 1;
            await store.dispatch(ActionTypes.SETT_ADD_PACKAGETYPE, { packageType: updatingPackageType }).catch(() => waitModal.value.on = false);
        } else {
            await store.dispatch(ActionTypes.SETT_UPDATE_PACKAGETYPE, { packageType: updatingPackageType }).catch(() => waitModal.value.on = false);
        }
        closeModal(waitModal.value);
        reloadList();
    }
    const togglePackageType = async (packageType: PackageType) => {
        waitModal.value.on = true;
        const updatedPackageType = { ...packageType };
        updatedPackageType.status = updatedPackageType.status == 0 ? 1 : 0;
        await updatePackageType(updatedPackageType)
        closeModal(waitModal.value);
    }

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const waitModal = ref(new Modal(500, t('generic.processing'), false));
    const modifyPackageTypeModal = ref(new Modal(500, '', true));

    // Menu Action
    const menuAction = (menuFunction: Function) => { menuFunction() };
    // Sorting
    const sort = (by: string) => { sortingFn.sort(by, filters_PTYP.value.sorting); reloadList(); };
    // Filtering
    const applyFilters = (column: string, value: any, load: boolean) => { filteringFn.applyFilters(column, value, filters_PTYP.value); if (load) reloadList(); };

    // initially launched functions
    await store.dispatch(ActionTypes.LOAD_DICT);
    const packageTypeStatus = store.state.config.dictionaries.packageTypeStatus;
    const packageTypeType = store.state.config.dictionaries.packageTypeType;

    // section Menu
    const menuItems_PTYP: Array<MenuItem> = [
        { state: null, stateParams: null, title: 'addNew', function: () => modifyPackageType(), icon: 'la-plus-circle' },
        { state: null, stateParams: null, title: 'refresh', function: reloadList, icon: 'la-redo-alt', float: 'end' },
    ];

    // table related stuff
    const columns_PTYP: Array<TableColumn> = [
        { alias: 'code', columnType: 'string', dropdown: null, sortable: true, f1: '', f2: null, resetTo: null },
        { alias: 'name', columnType: 'string', dropdown: null, sortable: true, f1: '', f2: null, resetTo: null },
        { alias: 'type', columnType: 'dropdown', dropdown: packageTypeType, sortable: true, f1: '3', f2: null, resetTo: '3' },
        { alias: 'status', columnType: 'dropdown', dropdown: packageTypeStatus, sortable: true, f1: '3', f2: null, resetTo: '3' }
    ];

    // init the screen
    onActivated(() => {
        reloadList();
        bus.on("refresh", reloadList)
    });
    onDeactivated(() => bus.off("refresh", reloadList))
</script>
<template>
    <section-menu :menu-items="menuItems_PTYP" t_orig="inboundReversal.operation" @action="menuAction" />
    <div class="container-fluid">
        <div class="row my-3">
            <div class="col-md-12">
                <table class="table table-striped table-hover filtered-sorted-table">
                    <table-sort-filter-header :columns=columns_PTYP
                                              :sorting=filters_PTYP.sorting
                                              t_core="settings.packageType"
                                              @sort="sort"
                                              @apply-filters="applyFilters"
                                              :sortable=true
                                              :filtreable=true />
                    <tbody v-if="!loadingList && list_PTYP.length > 0">
                        <tr v-for="pt, i in list_PTYP" :key="i" class="table-row">
                            <td>{{pt.code}}</td>
                            <td>{{pt.name}}</td>
                            <td>{{getPackageTypeType(pt.type)}}</td>
                            <td>{{getPackageTypeStatus(pt.status)}}</td>
                            <div class="table-buttons" v-if="pt.type != 0">
                                <button class="btn btn-primary me-2 btn-sm" @click="modifyPackageType(pt)"><i class="las la-pen"></i> {{t('modify')}}</button>
                                <button class="btn btn-sm btn-primary me-2" @click="togglePackageType(pt)"><i :class="['las',pt.status == 1 ? 'la-ban' : 'la-redo-alt']"></i> {{pt.status == 1 ? t('operation.general.deactivate') : t('operation.general.reactivate')}}</button>
                            </div>
                        </tr>
                    </tbody>
                </table>
                <div class="text-center pt-5 main-action-icons-small" v-if="!loadingList && list_PTYP.length == 0">
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
    <modify-package-type-popup :modalBase="modifyPackageTypeModal" @close="closeModal(modifyPackageTypeModal)" @proceed="updatePackageType" />
    <processing-popup :modalBase=waitModal />
</template>