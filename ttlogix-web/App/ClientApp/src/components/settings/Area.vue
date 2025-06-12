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
    import { Area } from '../../store/models/settings/area';
    import { AreaFilters } from '../../store/models/settings/areaFilters';
    import { WarehouseFilters } from '../../store/models/settings/warehouseFilters';
    import { AreaTypeFilters } from '../../store/models/settings/areaTypeFilters';

    // import widgets and modals
    import ProcessingPopup from '@/widgets/ProcessingPopup.vue';
    import SectionMenu from '@/widgets/SectionMenu.vue';
    import TableSortFilterHeader from '@/widgets/TableSortFilterHeader.vue';
    import ModifyAreaPopup from './modals/ModifyArea.vue'
    
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
    const list_AREA = computed(() => store.state.settings.area);
    const filters_AREA = ref(new AreaFilters());
    filters_AREA.value.whsCode = '';
    const filters_WHS = ref(new WarehouseFilters());
    const filters_ATYP = ref(new AreaTypeFilters());
    const loadingList = ref(false);
    const getAreaStatus = store.getters.getAreaStatus;

    // functions
    const reloadList = async () => {
        loadingList.value = true;
        filters_AREA.value.status = filters_AREA.value.status == 3 ? null : filters_AREA.value.status;
        await store.dispatch(ActionTypes.SETT_LOAD_AREA_LIST, { filters: filters_AREA.value }).catch(() => loadingList.value = false);
        loadingList.value = false;
    }
    const modifyArea = (area: Area | null = null) => {
        modifyAreaModal.value.on = true;
        modifyAreaModal.value.title = !area ? t('settings.area.addNewArea') : t('settings.area.modifyArea');
        modifyAreaModal.value.customProps = { ...area } ?? null;
    }
    const addArea = async (updatingArea: Area, addNext: boolean) => {
        if (!addNext) closeModal(modifyAreaModal.value);
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.SETT_ADD_AREA, { area: updatingArea }).catch(() => closeModal(waitModal.value));
        closeModal(waitModal.value);
        reloadList();
    }
    const updateArea = async (updatingArea: Area) => {
        closeModal(modifyAreaModal.value);
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.SETT_UPDATE_AREA, { area: updatingArea }).catch(() => closeModal(waitModal.value));
        closeModal(waitModal.value);
        reloadList();
    }
    const toggleArea = async (code: string, whsCode: string) => {
        waitModal.value.on = true;
        let updatedArea = new Area();
        await store.dispatch(ActionTypes.SETT_LOAD_AREA, { code: code, whsCode: whsCode })
                   .then((a) => updatedArea = a)
                   .catch(() => closeModal(waitModal.value));
        updatedArea.status = updatedArea.status == 1 ? 0 : 1;
        await updateArea(updatedArea);
    }

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const waitModal = ref(new Modal(500, t('generic.processing'), false));
    const modifyAreaModal = ref(new Modal(500, '', true));
    
    // Menu Action
    const menuAction = (menuFunction: Function) => { menuFunction() };
    // Sorting
    const sort = (by: string) => { sortingFn.sort(by, filters_AREA.value.sorting); reloadList(); };
    // Filtering
    const applyFilters = (column: string, value: any, load: boolean) => { filteringFn.applyFilters(column, value, filters_AREA.value); if (load) reloadList(); };

    // initially launched functions
    await store.dispatch(ActionTypes.LOAD_DICT);
    const areaStatus = store.state.config.dictionaries.areaStatus;

    // section Menu
    const menuItems_AREA:Array<MenuItem> = [
        { state:null, stateParams:null, title:'addNew',  function:() => modifyArea(), icon:'la-plus-circle' },
        { state:null, stateParams:null, title:'refresh', function:reloadList, icon:'la-redo-alt', float: 'end' },
    ];

    // table related stuff
    const columns_AREA: Array<TableColumn> = [
        { alias: 'code',   columnType: 'string',  dropdown: null, sortable: true, f1: '', f2: null, resetTo: null },
        { alias: 'name',   columnType: 'string',  dropdown: null, sortable: true, f1: '', f2: null, resetTo: null },
        { alias: 'type',   columnType: 'string',  dropdown: null, sortable: true, f1: '', f2: null, resetTo: null },
        { alias: 'whsCode',columnType: 'string',  dropdown: null, sortable: true, f1: '', f2: null, resetTo: null },
        { alias: 'status', columnType: 'dropdown',dropdown: areaStatus, sortable: true, f1: '3', f2: null, resetTo: '3' }
    ];

    // init the screen
    onActivated(() => {
        reloadList();
        bus.on("refresh", reloadList)
    });
    onDeactivated(() => bus.off("refresh", reloadList))
</script>
<template>
    <section-menu :menu-items="menuItems_AREA" t_orig="inboundReversal.operation" @action="menuAction" />
    <div class="container-fluid">
        <div class="row my-3">
            <div class="col-md-12">
                <table class="table table-striped table-hover filtered-sorted-table">
                    <table-sort-filter-header :columns=columns_AREA
                                              :sorting=filters_AREA.sorting
                                              t_core="settings.area"
                                              @sort="sort"
                                              @apply-filters="applyFilters"
                                              :sortable=true
                                              :filtreable=true />
                    <tbody v-if="!loadingList && list_AREA.length > 0">
                        <tr v-for="a, i in list_AREA" :key="i" class="table-row">
                            <td>{{a.code}}</td>
                            <td>{{a.name}}</td>
                            <td>{{a.type}}</td>
                            <td>{{a.whsCode}}</td>
                            <td>{{getAreaStatus(a.status)}}</td>
                            <div class="table-buttons">
                                <button class="btn btn-primary me-2 btn-sm" @click="modifyArea(a)"><i class="las la-pen"></i> {{t('modify')}}</button>
                                <button class="btn btn-sm btn-primary me-2" @click="toggleArea(a.code, a.whsCode)"><i :class="['las',a.status == 1 ? 'la-ban' : 'la-redo-alt']"></i> {{a.status == 1 ? t('operation.general.deactivate') : t('operation.general.reactivate')}}</button>
                            </div>
                        </tr>
                    </tbody>
                </table>
                <div class="text-center pt-5 main-action-icons-small" v-if="!loadingList && list_AREA.length == 0">
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
    <modify-area-popup :modalBase="modifyAreaModal" @close="closeModal(modifyAreaModal)" @update="updateArea" @add="addArea" />
    <processing-popup :modalBase=waitModal />
</template>