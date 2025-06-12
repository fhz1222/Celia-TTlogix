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
    import { UomSettings } from '../../store/models/settings/uom';
    import { UomSettingsFilters } from '../../store/models/settings/uomFilters';

    // import widgets and modals
    import ProcessingPopup from '@/widgets/ProcessingPopup.vue';
    import SectionMenu from '@/widgets/SectionMenu.vue';
    import TableSortFilterHeader from '@/widgets/TableSortFilterHeader.vue';
    import ModifyUomPopup from './modals/ModifyUom.vue'

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
    const list_UOM = computed(() => store.state.settings.uom);
    const filters_UOM = ref(new UomSettingsFilters());
    const loadingList = ref(false);
    const getUomStatus = store.getters.getUomStatus;
    const getUomType = store.getters.getUomType;

    // functions
    const reloadList = async () => {
        loadingList.value = true;
        filters_UOM.value.status = filters_UOM.value.status == 3 ? null : filters_UOM.value.status;
        filters_UOM.value.type = filters_UOM.value.type == 3 ? null : filters_UOM.value.type;
        await store.dispatch(ActionTypes.SETT_LOAD_UOM_LIST, { filters: filters_UOM.value }).catch(() => loadingList.value = false);
        loadingList.value = false;
    }
    const modifyUom = (uom: UomSettings | null = null) => {
        modifyUomModal.value.on = true;
        modifyUomModal.value.title = !uom ? t('settings.uom.addUom') : t('settings.uom.modifyUom');
        modifyUomModal.value.customProps = { ...uom };
    }
    const updateUom = async (updatingUom: UomSettings, addNext: boolean = false) => {
        if (!addNext) closeModal(modifyUomModal.value);
        waitModal.value.on = true;
        if (!updatingUom.code) {
            updatingUom.type = updatingUom.type ?? 1;
            await store.dispatch(ActionTypes.SETT_ADD_UOM, { uom: updatingUom }).catch(() => waitModal.value.on = false);
        } else {
            await store.dispatch(ActionTypes.SETT_UPDATE_UOM, { uom: updatingUom }).catch(() => waitModal.value.on = false);
        }
        closeModal(waitModal.value);
        reloadList();
    }
    const toggleUom = async (uom: UomSettings) => {
        waitModal.value.on = true;
        const updatedUom = { ...uom };
        updatedUom.status = updatedUom.status == 0 ? 1 : 0;
        await updateUom(updatedUom)
        closeModal(waitModal.value);
    }

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const waitModal = ref(new Modal(500, t('generic.processing'), false));
    const modifyUomModal = ref(new Modal(500, '', true));

    // Menu Action
    const menuAction = (menuFunction: Function) => { menuFunction() };
    // Sorting
    const sort = (by: string) => { sortingFn.sort(by, filters_UOM.value.sorting); reloadList(); };
    // Filtering
    const applyFilters = (column: string, value: any, load: boolean) => { filteringFn.applyFilters(column, value, filters_UOM.value); if (load) reloadList(); };

    // initially launched functions
    await store.dispatch(ActionTypes.LOAD_DICT);
    const uomStatus = store.state.config.dictionaries.uomStatus;
    const uomType = store.state.config.dictionaries.uomType;

    // section Menu
    const menuItems_UOM: Array<MenuItem> = [
        { state: null, stateParams: null, title: 'addNew', function: () => modifyUom(), icon: 'la-plus-circle' },
        { state: null, stateParams: null, title: 'refresh', function: reloadList, icon: 'la-redo-alt', float: 'end' },
    ];

    // table related stuff
    const columns_UOM: Array<TableColumn> = [
        { alias: 'code', columnType: 'string', dropdown: null, sortable: true, f1: '', f2: null, resetTo: null },
        { alias: 'name', columnType: 'string', dropdown: null, sortable: true, f1: '', f2: null, resetTo: null },
        { alias: 'type', columnType: 'dropdown', dropdown: uomType, sortable: true, f1: '3', f2: null, resetTo: '3' },
        { alias: 'status', columnType: 'dropdown', dropdown: uomStatus, sortable: true, f1: '3', f2: null, resetTo: '3' }
    ];

    // init the screen
    onActivated(() => {
        reloadList();
        bus.on("refresh", reloadList)
    });
    onDeactivated(() => bus.off("refresh", reloadList))
</script>
<template>
    <section-menu :menu-items="menuItems_UOM" t_orig="inboundReversal.operation" @action="menuAction" />
    <div class="container-fluid">
        <div class="row my-3">
            <div class="col-md-12">
                <table class="table table-striped table-hover filtered-sorted-table">
                    <table-sort-filter-header :columns=columns_UOM
                                              :sorting=filters_UOM.sorting
                                              t_core="settings.uom"
                                              @sort="sort"
                                              @apply-filters="applyFilters"
                                              :sortable=true
                                              :filtreable=true />
                    <tbody v-if="!loadingList && list_UOM.length > 0">
                        <tr v-for="uom, i in list_UOM" :key="i" class="table-row">
                            <td>{{uom.code}}</td>
                            <td>{{uom.name}}</td>
                            <td>{{getUomType(uom.type)}}</td>
                            <td>{{getUomStatus(uom.status)}}</td>
                            <div class="table-buttons" v-if="uom.type != 0">
                                <button class="btn btn-primary me-2 btn-sm" @click="modifyUom(uom)"><i class="las la-pen"></i> {{t('modify')}}</button>
                                <button class="btn btn-sm btn-primary me-2" @click="toggleUom(uom)"><i :class="['las',uom.status == 1 ? 'la-ban' : 'la-redo-alt']"></i> {{uom.status == 1 ? t('operation.general.deactivate') : t('operation.general.reactivate')}}</button>
                            </div>
                        </tr>
                    </tbody>
                </table>
                <div class="text-center pt-5 main-action-icons-small" v-if="!loadingList && list_UOM.length == 0">
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
    <modify-uom-popup :modalBase="modifyUomModal" @close="closeModal(modifyUomModal)" @proceed="updateUom" />
    <processing-popup :modalBase=waitModal />
</template>