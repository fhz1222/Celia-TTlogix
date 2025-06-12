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
    import { ControlCode } from '../../store/models/settings/controlCode';
    import { ControlCodeFilters } from '../../store/models/settings/controlCodeFilters';

    // import widgets and modals
    import ProcessingPopup from '@/widgets/ProcessingPopup.vue';
    import SectionMenu from '@/widgets/SectionMenu.vue';
    import TableSortFilterHeader from '@/widgets/TableSortFilterHeader.vue';
    import ModifyControlCodePopup from './modals/ModifyControlCode.vue'

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
    const list_CC = computed(() => store.state.settings.controlCode);
    const filters_CC = ref(new ControlCodeFilters());
    const loadingList = ref(false);
    const getControlCodeStatus = store.getters.getControlCodeStatus;
    const getControlCodeType = store.getters.getControlCodeType;

    // functions
    const reloadList = async () => {
        loadingList.value = true;
        filters_CC.value.status = filters_CC.value.status == 3 ? null : filters_CC.value.status;
        filters_CC.value.type = filters_CC.value.type == 3 ? null : filters_CC.value.type;
        await store.dispatch(ActionTypes.SETT_LOAD_CC_LIST, { filters: filters_CC.value })
            .catch(() => loadingList.value = false);
        loadingList.value = false;
    }
    const modifyControlCode = (controlCode: ControlCode | null = null) => {
        modifyControlCodeModal.value.on = true;
        modifyControlCodeModal.value.title = !controlCode ? t('settings.controlCode.addControlCode') : t('settings.controlCode.modifyControlCode');
        modifyControlCodeModal.value.customProps = { ...controlCode };
    }
    const updateControlCode = async (updatingControlCode: ControlCode, addNext: boolean = false) => {
        if (!addNext) closeModal(modifyControlCodeModal.value);
        waitModal.value.on = true;
        if (!updatingControlCode.code) {
            updatingControlCode.type = updatingControlCode.type ?? 1;
            await store.dispatch(ActionTypes.SETT_ADD_CC, { controlCode: updatingControlCode }).catch(() => waitModal.value.on = false);
        } else {
            await store.dispatch(ActionTypes.SETT_UPDATE_CC, { controlCode: updatingControlCode }).catch(() => waitModal.value.on = false);
        }
        closeModal(waitModal.value);
        reloadList();
    }
    const toggleControlCode = async (controlCode: ControlCode) => {
        waitModal.value.on = true;
        const updatedControlCode = { ...controlCode };
        updatedControlCode.status = updatedControlCode.status == 0 ? 1 : 0;
        await updateControlCode(updatedControlCode)
        closeModal(waitModal.value);
    }

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const waitModal = ref(new Modal(500, t('generic.processing'), false));
    const modifyControlCodeModal = ref(new Modal(500, '', true));

    // Menu Action
    const menuAction = (menuFunction: Function) => { menuFunction() };
    // Sorting
    const sort = (by: string) => { sortingFn.sort(by, filters_CC.value.sorting); reloadList(); };
    // Filtering
    const applyFilters = (column: string, value: any, load: boolean) => { filteringFn.applyFilters(column, value, filters_CC.value); if (load) reloadList(); };

    // initially launched functions
    await store.dispatch(ActionTypes.LOAD_DICT);
    const controlCodeStatus = store.state.config.dictionaries.controlCodeStatus;
    const controlCodeType = store.state.config.dictionaries.controlCodeType;

    // section Menu
    const menuItems_CC: Array<MenuItem> = [
        { state: null, stateParams: null, title: 'addNew', function: () => modifyControlCode(), icon: 'la-plus-circle' },
        { state: null, stateParams: null, title: 'refresh', function: reloadList, icon: 'la-redo-alt', float: 'end' },
    ];

    // table related stuff
    const columns_CC: Array<TableColumn> = [
        { alias: 'code', columnType: 'string', dropdown: null, sortable: true, f1: '', f2: null, resetTo: null },
        { alias: 'name', columnType: 'string', dropdown: null, sortable: true, f1: '', f2: null, resetTo: null },
        { alias: 'type', columnType: 'dropdown', dropdown: controlCodeType, sortable: true, f1: '3', f2: null, resetTo: '3' },
        { alias: 'status', columnType: 'dropdown', dropdown: controlCodeStatus, sortable: true, f1: '3', f2: null, resetTo: '3' }
    ];

    // init the screen
    onActivated(() => {
        reloadList();
        bus.on("refresh", reloadList)
    });
    onDeactivated(() => bus.off("refresh", reloadList))
</script>
<template>
    <section-menu :menu-items="menuItems_CC" t_orig="inboundReversal.operation" @action="menuAction" />
    <div class="container-fluid">
        <div class="row my-3">
            <div class="col-md-12">
                <table class="table table-striped table-hover filtered-sorted-table">
                    <table-sort-filter-header :columns=columns_CC
                                              :sorting=filters_CC.sorting
                                              t_core="settings.controlCode"
                                              @sort="sort"
                                              @apply-filters="applyFilters"
                                              :sortable=true
                                              :filtreable=true />
                    <tbody v-if="!loadingList && list_CC.length > 0">
                        <tr v-for="cc, i in list_CC" :key="i" class="table-row">
                            <td>{{cc.code}}</td>
                            <td>{{cc.name}}</td>
                            <td>{{getControlCodeType(cc.type)}}</td>
                            <td>{{getControlCodeStatus(cc.status)}}</td>
                            <div class="table-buttons" v-if="cc.type != 0">
                                <button class="btn btn-primary me-2 btn-sm" @click="modifyControlCode(cc)"><i class="las la-pen"></i> {{t('modify')}}</button>
                                <button class="btn btn-sm btn-primary me-2" @click="toggleControlCode(cc)"><i :class="['las',cc.status == 1 ? 'la-ban' : 'la-redo-alt']"></i> {{cc.status == 1 ? t('operation.general.deactivate') : t('operation.general.reactivate')}}</button>
                            </div>
                        </tr>
                    </tbody>
                </table>
                <div class="text-center pt-5 main-action-icons-small" v-if="!loadingList && list_CC.length == 0">
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
    <modify-control-code-popup :modalBase="modifyControlCodeModal" @close="closeModal(modifyControlCodeModal)" @proceed="updateControlCode" />
    <processing-popup :modalBase=waitModal />
</template>