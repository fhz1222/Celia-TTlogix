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
    import { SatoPrinter } from '../../store/models/settings/satoPrinter';
    import { SatoPrinterFilters } from '../../store/models/settings/satoPrinterFilters';

    // import widgets and modals
    import ProcessingPopup from '@/widgets/ProcessingPopup.vue';
    import SectionMenu from '@/widgets/SectionMenu.vue';
    import TableSortFilterHeader from '@/widgets/TableSortFilterHeader.vue';
    import ModifySatoPrinterPopup from './modals/ModifySatoPrinter.vue';
    import ConfirmPopup from '@/widgets/ConfirmPopup.vue';

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
    const list_SATO = computed(() => store.state.settings.satoPrinter);
    const filters_SATO = ref(new SatoPrinterFilters());
    const loadingList = ref(false);
    const getSatoPrinterType = store.getters.getSatoPrinterType;

    // functions
    const reloadList = async () => {
        loadingList.value = true;
        filters_SATO.value.type = filters_SATO.value.type == 3 ? null : filters_SATO.value.type;
        await store.dispatch(ActionTypes.SETT_LOAD_SATO_LIST, { filters: filters_SATO.value })
            .catch(() => loadingList.value = false);
        loadingList.value = false;
    }
    const modifySatoPrinter = (satoPrinter: SatoPrinter | null = null) => {
        modifySatoPrinterModal.value.on = true;
        modifySatoPrinterModal.value.title = !satoPrinter ? t('settings.sato.addPrinter') : t('settings.sato.modifyPrinter');
        modifySatoPrinterModal.value.customProps = { ...satoPrinter };
    }
    const addSatoPrinter = async (updatingSatoPrinter: SatoPrinter, addNext: boolean) => {
        if (!addNext) closeModal(modifySatoPrinterModal.value);
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.SETT_ADD_SATO, { satoPrinter: updatingSatoPrinter }).catch(() => waitModal.value.on = false);
        closeModal(waitModal.value);
        reloadList();
    }
    const updateSatoPrinter = async (updatingSatoPrinter: SatoPrinter, addNext: boolean) => {
        closeModal(modifySatoPrinterModal.value);
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.SETT_UPDATE_SATO, { satoPrinter: updatingSatoPrinter }).catch(() => waitModal.value.on = false);
        closeModal(waitModal.value);
        reloadList();
    }
    const deleteSatoPrinter = async (satoPrinter: SatoPrinter) => {
        confirmModal.value.fnMod.message = t('settings.sato.confirmDelete', { 'name': satoPrinter.name, 'ip': satoPrinter.ip });
        confirmModal.value.on = true;
        confirmModal.value.fnMod.callback = async () => { await deleteSatoPrinterExecute(satoPrinter.ip) };
    }
    const deleteSatoPrinterExecute = async (ip: string | null) => {
        closeModal(confirmModal.value);
        if (ip) {
            waitModal.value.on = true;
            await store.dispatch(ActionTypes.SETT_DELETE_SATO, { ip: ip }).catch(() => waitModal.value.on = false);
            closeModal(waitModal.value);
        }
        reloadList();
    }

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const waitModal = ref(new Modal(500, t('generic.processing'), false));
    const modifySatoPrinterModal = ref(new Modal(500, '', true));
    const confirmModal = ref(new Modal(600, t('generic.pleaseConfirm'), false));

    // Menu Action
    const menuAction = (menuFunction: Function) => { menuFunction() };
    // Sorting
    const sort = (by: string) => { sortingFn.sort(by, filters_SATO.value.sorting); reloadList(); };
    // Filtering
    const applyFilters = (column: string, value: any, load: boolean) => { filteringFn.applyFilters(column, value, filters_SATO.value); if (load) reloadList(); };

    // initially launched functions
    await store.dispatch(ActionTypes.LOAD_DICT);
    const satoPrinterType = store.state.config.dictionaries.satoPrinterType;

    // section Menu
    const menuItems_SATO: Array<MenuItem> = [
        { state: null, stateParams: null, title: 'addNew', function: () => modifySatoPrinter(), icon: 'la-plus-circle' },
        { state: null, stateParams: null, title: 'refresh', function: reloadList, icon: 'la-redo-alt', float: 'end' },
    ];

    // table related stuff
    const columns_SATO: Array<TableColumn> = [
        { alias: 'ip',         columnType: 'string',  dropdown: null, sortable: true, f1: '', f2: null, resetTo: null },
        { alias: 'name',       columnType: 'string',  dropdown: null, sortable: true, f1: '', f2: null, resetTo: null },
        { alias: 'description',columnType: 'string',  dropdown: null, sortable: true, f1: '', f2: null, resetTo: null },
        { alias: 'type',       columnType: 'dropdown',dropdown: satoPrinterType, sortable: true, f1: '3', f2: null, resetTo: '3' }
    ];

    // init the screen
    onActivated(() => {
        reloadList();
        bus.on("refresh", reloadList)
    });
    onDeactivated(() => bus.off("refresh", reloadList))
</script>
<template>
    <section-menu :menu-items="menuItems_SATO" t_orig="inboundReversal.operation" @action="menuAction" />
    <div class="container-fluid">
        <div class="row my-3">
            <div class="col-md-12">
                <table class="table table-striped table-hover filtered-sorted-table">
                    <table-sort-filter-header :columns=columns_SATO
                                              :sorting=filters_SATO.sorting
                                              t_core="settings.sato"
                                              @sort="sort"
                                              @apply-filters="applyFilters"
                                              :sortable=true
                                              :filtreable=true />
                    <tbody v-if="!loadingList && list_SATO.length > 0">
                        <tr v-for="ss, i in list_SATO" :key="i" class="table-row">
                            <td>{{ss.ip}}</td>
                            <td>{{ss.name}}</td>
                            <td>{{ss.description}}</td>
                            <td>{{getSatoPrinterType(ss.type)}}</td>
                            <div class="table-buttons">
                                <button class="btn btn-danger me-2 btn-sm" @click="deleteSatoPrinter(ss)"><i class="las la-trash"></i> {{t('operation.general.delete')}}</button>
                                <button class="btn btn-primary me-2 btn-sm" @click="modifySatoPrinter(ss)"><i class="las la-pen"></i> {{t('modify')}}</button>
                            </div>
                        </tr>
                    </tbody>
                </table>
                <div class="text-center pt-5 main-action-icons-small" v-if="!loadingList && list_SATO.length == 0">
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
    <modify-sato-Printer-popup :modalBase="modifySatoPrinterModal" @close="closeModal(modifySatoPrinterModal)" @update="updateSatoPrinter" @add="addSatoPrinter" />
    <processing-popup :modalBase=waitModal />
    <confirm-popup :modalBase=confirmModal @close="closeModal(confirmModal)" />
</template>