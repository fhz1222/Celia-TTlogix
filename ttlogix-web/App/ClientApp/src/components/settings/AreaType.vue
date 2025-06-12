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
    import { AreaType } from '../../store/models/settings/areaType';
    import { AreaTypeFilters } from '../../store/models/settings/areaTypeFilters';

    // import widgets and modals
    import ProcessingPopup from '@/widgets/ProcessingPopup.vue';
    import SectionMenu from '@/widgets/SectionMenu.vue';
    import TableSortFilterHeader from '@/widgets/TableSortFilterHeader.vue';
    import ModifyAreaTypePopup from './modals/ModifyAreaType.vue'
    
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
    const customerCode = ref<string>(route.params.customerCode?.toString());
    const list_ATYP = computed(() => store.state.settings.areaType);
    const filters_ATYP = ref(new AreaTypeFilters());
    const loadingList = ref(false);
    const getAreaTypeStatus = store.getters.getAreaTypeStatus;
    const getAreaTypeType = store.getters.getAreaTypeType;

    // functions
    const reloadList = async () => {
        loadingList.value = true;
        filters_ATYP.value.status = filters_ATYP.value.status == 3 ? null : filters_ATYP.value.status;
        filters_ATYP.value.type = filters_ATYP.value.type == 3 ? null : filters_ATYP.value.type;
        await store.dispatch(ActionTypes.SETT_LOAD_AREATYPE_LIST, { filters: filters_ATYP.value })
                   .catch(() => loadingList.value = false);
        loadingList.value = false;
    }
    const modifyAreaType = (areaType: AreaType | null = null) => {
        modifyAreaTypeModal.value.on = true;
        modifyAreaTypeModal.value.title = !areaType ? t('settings.areaType.addAreaType') : t('settings.areaType.modifyAreaType');
        modifyAreaTypeModal.value.customProps = { ...areaType };
    }
    const updateAreaType = async (updatingAreaType: AreaType, addNext: boolean = false) => {
        closeModal(modifyAreaTypeModal.value);
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.SETT_UPDATE_AREATYPE, { areaType: updatingAreaType }).catch(() => waitModal.value.on = false);
        closeModal(waitModal.value);
        reloadList();
    }
    const addAreaType = async (updatingAreaType: AreaType, addNext: boolean = false) => {
        if (!addNext) closeModal(modifyAreaTypeModal.value);
        waitModal.value.on = true;
        updatingAreaType.type = updatingAreaType.type ?? 1;
        await store.dispatch(ActionTypes.SETT_ADD_AREATYPE, { areaType: updatingAreaType }).catch(() => waitModal.value.on = false);
        closeModal(waitModal.value);
        reloadList();
    }
    const toggleAreaType = async (areaType: AreaType) => {
        waitModal.value.on = true;
        const updatedAreaType = { ...areaType };
        updatedAreaType.status = updatedAreaType.status == 0 ? 1 : 0;
        await updateAreaType(updatedAreaType)
        closeModal(waitModal.value);
    }

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const waitModal = ref(new Modal(500, t('generic.processing'), false));
    const modifyAreaTypeModal = ref(new Modal(400, '', true));
    
    // Menu Action
    const menuAction = (menuFunction: Function) => { menuFunction() };
    // Sorting
    const sort = (by: string) => { sortingFn.sort(by, filters_ATYP.value.sorting); reloadList(); };
    // Filtering
    const applyFilters = (column: string, value: any, load: boolean) => { filteringFn.applyFilters(column, value, filters_ATYP.value); if (load) reloadList(); };

    // initially launched functions
    await store.dispatch(ActionTypes.LOAD_DICT);
    const areaTypeStatus = store.state.config.dictionaries.areaTypeStatus;
    const areaTypeType = store.state.config.dictionaries.areaTypeType;

    // section Menu
    const menuItems_ATYP:Array<MenuItem> = [
        { state: null, stateParams: null, title: 'addNew', function: () => modifyAreaType(), icon:'la-plus-circle' },
        { state:null, stateParams:null, title:'refresh', function:reloadList, icon:'la-redo-alt', float: 'end' },
    ];

    // table related stuff
    const columns_ATYP: Array<TableColumn> = [
        { alias: 'code',  columnType: 'string',  dropdown: null, sortable: true, f1: '', f2: null, resetTo: null },
        { alias: 'name',  columnType: 'string',  dropdown: null, sortable: true, f1: '', f2: null, resetTo: null },
        { alias: 'type',  columnType: 'dropdown',dropdown: areaTypeType,  sortable: true, f1: '3', f2: null, resetTo: '3' },
        { alias: 'status',columnType: 'dropdown',dropdown: areaTypeStatus,sortable: true, f1: '3', f2: null, resetTo: '3' } 
    ];

    // init the screen
    onActivated(() => {
        reloadList();
        bus.on("refresh", reloadList)
    });
    onDeactivated(() => bus.off("refresh", reloadList))
</script>
<template>
    <section-menu :menu-items="menuItems_ATYP" t_orig="inboundReversal.operation" @action="menuAction" />
    <div class="container-fluid">
        <div class="row my-3">
            <div class="col-md-12">
                <table class="table table-striped table-hover filtered-sorted-table">
                    <table-sort-filter-header :columns=columns_ATYP
                                              :sorting=filters_ATYP.sorting
                                              t_core="settings.areaType"
                                              @sort="sort"
                                              @apply-filters="applyFilters"
                                              :sortable=true
                                              :filtreable=true />
                    <tbody v-if="!loadingList && list_ATYP.length > 0">
                        <tr v-for="at, i in list_ATYP" :key="i" class="table-row">
                            <td>{{at.code}}</td>
                            <td>{{at.name}}</td>
                            <td>{{getAreaTypeType(at.type)}}</td>
                            <td>{{getAreaTypeStatus(at.status)}}</td>
                            <div class="table-buttons" v-if="at.type != 0">
                                <button class="btn btn-primary me-2 btn-sm" @click="modifyAreaType(at)"><i class="las la-pen"></i> {{t('modify')}}</button>
                                <button class="btn btn-sm btn-primary me-2" @click="toggleAreaType(at)"><i :class="['las',at.status == 1 ? 'la-ban' : 'la-redo-alt']"></i> {{at.status == 1 ? t('operation.general.deactivate') : t('operation.general.reactivate')}}</button>
                            </div>
                        </tr>
                    </tbody>
                </table>
                <div class="text-center pt-5 main-action-icons-small" v-if="!loadingList && list_ATYP.length == 0">
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
    <modify-area-type-popup :modalBase="modifyAreaTypeModal" @close="closeModal(modifyAreaTypeModal)" @update="updateAreaType" @add="addAreaType" />
    <processing-popup :modalBase=waitModal />
</template>