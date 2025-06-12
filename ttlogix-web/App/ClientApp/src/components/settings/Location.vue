<script lang="ts" setup>

    import bus from '@/event_bus.js'
    // import vue stuff
    import { useStore } from '@/store';
    import { computed, ref, onActivated, onDeactivated, watch, toRaw } from 'vue';
    import { ActionTypes } from '@/store/action-types';
    import { useI18n } from 'vue-i18n';
    import { useRoute } from 'vue-router';

    // import types
    import { MenuItem, TableColumn } from '@/store/commonModels/config';
    import { Modal } from '@/store/commonModels/modal';
    import { LocationSettings } from '@/store/models/settings/location';
    import { LocationFilters } from '@/store/models/settings/locationFilters';
    import { WarehouseFilters } from '@/store/models/settings/warehouseFilters';
    import { AreaFilters } from '@/store/models/settings/areaFilters';
    import { SettingsComboBox } from '../../store/models/settings/settingsComboBox';
    import { LocationQrCode, PdfLocationLabel, CodeCombo } from '@/store/models/settings/locationLabel'

    // import widgets and modals
    import ProcessingPopup from '@/widgets/ProcessingPopup.vue';
    import SectionMenu from '@/widgets/SectionMenu.vue';
    import TableSortFilterHeader from '@/widgets/TableSortFilterHeader.vue';
    import ModifyLocationPopup from './modals/ModifyLocation.vue'
    import PrintLabelPopup from '@/components/print/PrintLocationLabel.vue';
    import AwaitingButton from '@/widgets/AwaitingButton.vue';
    import * as pdfLabels from '../print/pdf_label_generator'

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
    const list_LOC = computed(() => store.state.settingsLocationList);
    const filters_LOC = ref(new LocationFilters());
    const currentWarehouse = computed(() => store.state.whsCode)
    filters_LOC.value.warehouseCode = currentWarehouse.value;
    const loadingList = ref(false);
    const getLocationStatus = store.getters.getLocationStatus;
    const getLocationType = store.getters.getLocationType;
    const getLocationPriority = store.getters.getLocationPriority;
    const iLogCategoriesDict = ref({});
    const selectionCount = ref(-1);

    // Print - old modal
    const modal: any = ref(null);

    // functions
    const reloadList = async () => {
        loadingList.value = true;
        filters_LOC.value.status = filters_LOC.value.status == 3 ? null : filters_LOC.value.status;
        filters_LOC.value.type = filters_LOC.value.type == 10 ? null : filters_LOC.value.type;
        filters_LOC.value.isPriority = filters_LOC.value.isPriority == 3 ? null : filters_LOC.value.isPriority;
        filters_LOC.value.iLogLocationCategoryId = filters_LOC.value.iLogLocationCategoryId == 10 ? null : filters_LOC.value.iLogLocationCategoryId;
        await store.dispatch(ActionTypes.SETT_LOAD_LOC_LIST, { filters: filters_LOC.value }).catch(() => loadingList.value = false);
        loadingList.value = false;
    }
    const getILogCategory = (id: number) => {
        for (const c in iLogCategory) { if(c == id.toString()) return iLogCategory[c]}
    }
    const modifyLocation = (location: LocationSettings | null = null) => {
        modifyLocationModal.value.on = true;
        modifyLocationModal.value.title = !location ? t('settings.location.addLocation') : t('settings.location.modifyLocation');
        modifyLocationModal.value.customProps = { ...location } ?? null;
    }
    const addLocation = async (updatingLocation: LocationSettings, addNext: boolean) => {
        if (!addNext) closeModal(modifyLocationModal.value);
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.SETT_ADD_LOCATION, { location: updatingLocation }).catch(() => closeModal(waitModal.value));
        closeModal(waitModal.value);
        reloadList();
    }
    const updateLocation = async (updatingLocation: LocationSettings) => {
        closeModal(modifyLocationModal.value);
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.SETT_UPDATE_LOCATION, { location: updatingLocation }).catch(() => closeModal(waitModal.value));
        closeModal(waitModal.value);
        reloadList();
    }
    const toggleLocation = async (code: string, whsCode: string) => {
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.SETT_TOGGLE_ACTIVE_LOCATION, { code: code, whsCode: whsCode }).catch(() => closeModal(waitModal.value));
        closeModal(waitModal.value)
    }
    const loadMore = async () => {
        await store.dispatch(ActionTypes.SETT_LOAD_LOC_MORE, { filters: filters_LOC.value });
    }
    const toggleSelected = (e: MouseEvent, loc: LocationSettings): void => {
        const objIndex = list_LOC.value.items.findIndex((obj:LocationSettings) => obj.code == loc.code);
        if (selectionCount.value == -1) {
            toggleState(objIndex); selectionCount.value = objIndex;
        } else {
            if (e.shiftKey) {
                const startingIndex = objIndex > selectionCount.value ? selectionCount.value : objIndex;
                const diff = Math.abs(objIndex - selectionCount.value);
                for (var i = startingIndex; i < (startingIndex + diff); i++) {
                    list_LOC.value.items[i].selected = i == startingIndex ? list_LOC.value.items[i].selected : !list_LOC.value.items[i].selected;
                }
                selectionCount.value = -1;
            } else {
                toggleState(objIndex); selectionCount.value = objIndex;
            }
        }
    }
    const toggleState = (index: number) => {
        list_LOC.value.items[index].selected = !list_LOC.value.items[index].selected;
    }
    const areSelected = () => {
        return list_LOC.value.items.filter((loc:LocationSettings) => loc.selected == true).length > 0;
    }
    const printLabel = () => {
        if (areSelected()) {
            printLabelModal.value.on = true;
            const printable = list_LOC.value.items.filter((loc:LocationSettings) => loc.selected == true);
            let codeCombos = new Array<CodeCombo>();
            printable.forEach((p:any) => codeCombos.push({ code: p.code ?? '', whsCode: p.warehouseCode ?? '' }));
            printLabelModal.value.customProps = codeCombos;
        }
    }
    const printToPdf = (labels: Array<PdfLocationLabel>) => {
        pdfLabels.downloadLocationLabels(labels);
    }

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const waitModal = ref(new Modal(400, t('generic.processing'), false));
    const modifyLocationModal = ref(new Modal(600, '', true));
    const printLabelModal = ref(new Modal(500, 'Print Selected Location Labels', true));

    // Menu Action
    const menuAction = (menuFunction: Function) => { menuFunction() };
    // Sorting
    const sort = (by: string) => { sortingFn.sort(by, filters_LOC.value.sorting); reloadList(); };
    // Filtering
    const applyFilters = (column: string, value: any, load: boolean) => { filteringFn.applyFilters(column, value, filters_LOC.value); if (load) reloadList(); };

    // initially launched functions
    await store.dispatch(ActionTypes.LOAD_DICT);
    await store.dispatch(ActionTypes.SETT_CB_LOAD_ILOG_LOC_CAT)
               .then((ilc) => { iLogCategoriesDict.value = Object.fromEntries(new Map(ilc.map((o) => [o.code, o.label]))); })
    const locationStatus = store.state.config.dictionaries.locationStatus;
    const locationType = store.state.config.dictionaries.locationType;
    const locationPriority = store.state.config.dictionaries.locationPriority;
    let iLogCategory: {[id:number]:string} = iLogCategoriesDict.value;
    iLogCategory = { ...iLogCategory, 10: 'All' };

    // section Menu
    const menuItems_LOC: Array<MenuItem> = [
        { state: null, stateParams: null, title: 'addNew', function: () => modifyLocation(), icon: 'la-plus-circle' },
        { state: null, stateParams: null, title: 'printLabels', function: () => printLabel(), icon: 'la-print' },
        { state: null, stateParams: null, title: 'refresh', function: reloadList, icon: 'la-redo-alt', float: 'end' },
    ];

    // table related stuff
    const columns_LOC: Array<TableColumn> = [
        { alias: '', columnType: null, dropdown: null, sortable: false, f1: null, f2: null, resetTo: null },
        { alias: 'code', columnType: 'string', dropdown: null, sortable: true, f1: '', f2: null, resetTo: null },
        { alias: 'name', columnType: 'string', dropdown: null, sortable: true, f1: '', f2: null, resetTo: null },
        { alias: 'warehouseCode', columnType: 'string', dropdown: null, sortable: true, f1: currentWarehouse.value, f2: null, resetTo: null, filterNameForSorting: 'whsCode' },
        { alias: 'type', columnType: 'dropdown', dropdown: locationType, sortable: true, f1: '10', f2: null, resetTo: '10'},
        { alias: 'isPriority', columnType: 'dropdown', dropdown: locationPriority, sortable: true, f1: '3', f2: null, resetTo: '3' },
        { alias: 'areaCode', columnType: 'string', dropdown: null, sortable: true, f1: '', f2: null, resetTo: null },
        { alias: 'iLogLocationCategoryId', columnType: 'dropdown', dropdown: iLogCategory, sortable: true, f1: '10', f2: null, resetTo: '10' },
        { alias: 'status', columnType: 'dropdown', dropdown: locationStatus, sortable: true, f1: '3', f2: null, resetTo: '3' }
    ];

    // init the screen
    onActivated(() => {
        reloadList();
        bus.on("refresh", reloadList)
    });
    onDeactivated(() => bus.off("refresh", reloadList))
</script>
<template>
    <section-menu :menu-items="menuItems_LOC" t_orig="operation.general" @action="menuAction" />
    <div class="container-fluid">
        <div class="row my-3">
            <div class="col-md-12">
                <h5 class="text-center mt-2 blue-text weight-normal">
                    {{ t('settings.location.percentSignSearch') }}
                </h5>
                <table class="table table-striped table-hover filtered-sorted-table">
                    <table-sort-filter-header :columns=columns_LOC
                                              :sorting=filters_LOC.sorting
                                              t_core="settings.location"
                                              @sort="sort"
                                              @apply-filters="applyFilters"
                                              :sortable=true
                                              :filtreable=true />
                    <tbody v-if="!loadingList && list_LOC.items && list_LOC.items.length > 0">
                        <tr v-for="loc, i in list_LOC.items" :key="i" class="table-row">
                            <td @click="toggleSelected($event, loc)"><input type="checkbox" v-model="loc.selected" class="form-check-input"></td>
                            <td>{{loc.code}}</td>
                            <td>{{loc.name}}</td>
                            <td>{{loc.warehouseCode}}</td>
                            <td>{{getLocationType(loc.type)}}</td>
                            <td>{{getLocationPriority(loc.isPriority)}}</td>
                            <td>{{loc.areaCode}}</td>
                            <td>{{getILogCategory(loc.iLogLocationCategoryId)}}</td>
                            <td>{{getLocationStatus(loc.status)}}</td>
                            <div class="table-buttons">
                                <button class="btn btn-primary me-2 btn-sm" @click="modifyLocation(loc)"><i class="las la-pen"></i> {{t('modify')}}</button>
                                <button class="btn btn-sm btn-primary me-2" @click="toggleLocation(loc.code, loc.warehouseCode)"><i :class="['las',loc.status == 1 ? 'la-ban' : 'la-redo-alt']"></i> {{loc.status == 1 ? t('operation.general.deactivate') : t('operation.general.reactivate')}}</button>
                            </div>
                        </tr>
                    </tbody>
                </table>
                <div class="text-center pt-5 main-action-icons-small" v-if="!loadingList && list_LOC.items && list_LOC.items.length == 0">
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
        <div class="text-center mt-3 mb-2">
            <div v-if="!loadingList && list_LOC.items && list_LOC.items.length < list_LOC.pagination.totalCount">
                <AwaitingButton @proceed="(callback) => { loadMore().finally(callback) }"
                                :btnText="'operation.dynamicTable.loadMore'"
                                :btnIcon="'las la-chevron-down'"
                                :btnClass="'btn-primary'">
                </AwaitingButton>
                <br />
                <span class="me-2" v-html="t('operation.dynamicTable.showingBatch', {dataLength:list_LOC.items.length, total:list_LOC.pagination.totalCount })"></span>
            </div>
        </div>
    </div>
    <modify-location-popup :modalBase="modifyLocationModal" @close="closeModal(modifyLocationModal)" @update="updateLocation" @add="addLocation" />
    <print-label-popup :modalBase="printLabelModal" @close="closeModal(printLabelModal)" @printToPDF="printToPdf" />
    <processing-popup :modalBase=waitModal />
</template>