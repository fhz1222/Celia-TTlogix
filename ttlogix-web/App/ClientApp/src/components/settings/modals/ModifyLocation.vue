<script lang="ts" setup>
    // import vue stuff
    import { computed, ref, watch } from 'vue';
    import { useI18n } from 'vue-i18n';
    import { useStore } from '@/store';

    // import types
    import { Modal } from '@/store/commonModels/modal';
    import { ActionTypes } from '@/store/action-types';
    import { LocationSettings } from '@/store/models/settings/location';
    import { SettingsComboBox } from '@/store/models/settings/settingsComboBox';

    // modals and widgets
    import ModalWidget from '@/widgets/Modal3.vue';

    // props & emits
    const props = defineProps<{ modalBase: Modal }>();
    const emit = defineEmits<{ (e: 'close'): void, (e: 'add', updatedLocation: LocationSettings, addNext: boolean): void, (e: 'update', updatedLocation: LocationSettings): void }>();

    // local vars
    const store = useStore();
    const { t } = useI18n({ useScope: 'global' });
    const loading = ref(false);
    const currentLocation = ref(new LocationSettings());
    const activeAreas = ref(new Array<SettingsComboBox>());
    const activeWarehouses = ref(new Array<SettingsComboBox>());
    const currentLocationPriority = ref(false);
    const newlyAdded = ref(true);
    const getLocationType = store.getters.getLocationType;

    // functions
    const initializeScreen = async () => {
        loading.value = true;
        if (currentLocation.value.code != null) {
            await store.dispatch(ActionTypes.SETT_LOAD_LOCATION, { code: currentLocation.value.code ?? '', whsCode: currentLocation.value.warehouseCode ?? '' })
                .then((a) => { currentLocation.value = a; currentLocationPriority.value = currentLocation.value.isPriority == 1 ? true : false; }).catch(() => loading.value = false);
        }
        if (!newlyAdded.value) await loadActiveAreas(currentLocation.value.warehouseCode ?? '')
        await store.dispatch(ActionTypes.SETT_CB_LOAD_ACTIVE_WAREHOUSES).then((aw) => activeWarehouses.value = aw).catch(() => loading.value = false);
        loading.value = false;
    }
    const loadActiveAreas = async (whsCode: string) => {
        await store.dispatch(ActionTypes.SETT_CB_LOAD_ACTIVE_AREAS, { whsCode: whsCode })
                   .then((aa) => activeAreas.value = aa).catch(() => loading.value = false);
    }
    const canDoNext = () => {
        return !props.modalBase.customProps;
    }
    const canSave = () => {
        return currentLocation.value.code && currentLocation.value.name && currentLocation.value.warehouseCode && currentLocation.value.areaCode;
    }

    const proceed = async (addNext: boolean) => {
        currentLocation.value.isPriority = currentLocationPriority.value ? 1 : 0;
        if (newlyAdded.value) emit('add', currentLocation.value, addNext)
        else emit('update', currentLocation.value)
        resetModal();
    }
    const cancel = async () => {
        emit('close');
        resetModal();
    }
    const resetModal = () => {
        currentLocation.value = new LocationSettings();
        activeAreas.value = new Array<SettingsComboBox>();
        currentLocationPriority.value = false;
    }

    watch(props.modalBase, async (nV, oV) => {
        if (nV.on == false) { newlyAdded.value = false; resetModal(); }
        if (nV.on == true) {
            if (nV.customProps.code != null) {
                currentLocation.value = nV.customProps;
                newlyAdded.value = false;
            } else {
                newlyAdded.value = true;
            }
            await initializeScreen();
        }
    })
</script>

<template>
    <modal-widget :modalBase=modalBase @close="emit('close', false)">
        <template name="title" v-slot:title>
            <h5>{{modalBase.title}}</h5>
        </template>
        <template name="body" v-slot:body>
            <div class="card">
                <div class="card-body">
                    <div class="row mt-2">
                        <div class="col-md-4 text-end no-right-padding pt-1">{{t('settings.location.locationCode')}}:</div>
                        <div class="col-md-8" v-if="newlyAdded">
                            <input type="text" class="form-control form-control-sm" v-model="currentLocation.code" />
                        </div>
                        <div class="col-md-4 pt-1" v-if="!newlyAdded"><strong>{{currentLocation.code}}</strong></div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-4 text-end no-right-padding pt-1">{{t('settings.location.locationName')}}:</div>
                        <div class="col-md-8">
                            <input type="text" class="form-control form-control-sm" v-model="currentLocation.name" />
                        </div>
                    </div>
                    <div class="row mt-3">
                        <div class="col-md-4 text-end no-right-padding">{{t('settings.location.warehouse')}}:</div>
                        <div class="col-md-8 multiselect-as-select">
                            <select class="form-select form-select-sm" v-if="newlyAdded" v-model="currentLocation.warehouseCode" @change="loadActiveAreas(currentLocation.warehouseCode)">
                                <option v-for="whs,i in activeWarehouses" :key="i" :value="whs.code">{{whs.label}}</option>
                            </select>
                            <span v-if="!newlyAdded"><strong>{{currentLocation.warehouseCode}}</strong></span>
                        </div>
                    </div>
                    <div class="row mt-3">
                        <div class="col-md-4 text-end no-right-padding">{{t('settings.location.area')}}:</div>
                        <div class="col-md-8 multiselect-as-select">
                            <select class="form-select form-select-sm" v-model="currentLocation.areaCode" v-if="newlyAdded && activeAreas.length > 0">
                                <option v-for="a,i in activeAreas" :key="i" :value="a.code">{{a.code + ' - ' + a.label}}</option>
                            </select>
                            <span v-if="!newlyAdded"><strong>{{currentLocation.areaCode}}</strong></span>
                            <span v-if="newlyAdded && activeAreas.length == 0 && !currentLocation.warehouseCode">{{t('settings.location.noWhsSelected')}}</span>
                            <span v-if="newlyAdded && activeAreas.length == 0 && currentLocation.warehouseCode">No active areas for {{currentLocation.warehouseCode}}</span>
                        </div>
                    </div>
                    <div class="row mt-3">
                        <div class="col-md-4 text-end no-right-padding pt-1">{{t('settings.location.capacity')}}:</div>
                        <div class="col-md-3">
                            <input type="text" class="form-control form-control-sm" v-model="currentLocation.m3" />
                        </div>
                        <div class="col-md-3 pt-1 no-left-padding">m<sup>3</sup></div>
                    </div>
                    <div class="row mt-3">
                        <div class="col-md-4 text-end no-right-padding pt-1">{{t('settings.location.type')}}:</div>
                        <div class="col-md-8 pt-1" v-if="newlyAdded">
                            <div class="row">
                                <div class="col-6">
                                    <input class="form-check-input me-2" type="radio" v-model="currentLocation.type" value="0" name="type" id="type1">
                                    <label class="form-check-label" for="type1">{{t('settings.location.normal')}}</label>
                                </div>
                                <div class="col-6">
                                    <input class="form-check-input me-2" type="radio" v-model="currentLocation.type" value="1" name="type" id="type2">
                                    <label class="form-check-label" for="type2">{{t('settings.location.quarantine')}}</label>
                                </div>
                                <div class="col-6 mt-2">
                                    <input class="form-check-input me-2" type="radio" v-model="currentLocation.type" value="3" name="type" id="type3">
                                    <label class="form-check-label" for="type3">{{t('settings.location.standBy')}}</label>
                                </div>
                                <div class="col-6 mt-2">
                                    <input class="form-check-input me-2" type="radio" v-model="currentLocation.type" value="2" name="type" id="type4">
                                    <label class="form-check-label" for="type4">{{t('settings.location.crossDock')}}</label>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-8 pt-1" v-if="!newlyAdded"><strong>{{getLocationType(currentLocation.type)}}</strong></div>
                    </div>
                    <div class="row mt-3">
                        <div class="col-md-4 text-end no-right-padding">{{t('settings.location.priority')}}:</div>
                        <div class="col-md-8">
                            <input class="form-check-input me-2" type="checkbox" v-model="currentLocationPriority" id="priority">
                        </div>
                    </div>
                </div>
            </div>
        </template>
        <template name="footer" v-slot:footer>
            <div class="row">
                <div class="col-md-12 text-end">
                    <button class="btn btn-sm btn-secondary" @click="cancel"><i class="las la-ban"></i> {{ t('operation.general.cancel') }}</button>
                    <button class="btn btn-sm btn-secondary ms-2" @click="proceed(false)" :disabled="!canSave()"><i class="las la-save"></i> {{ t('operation.general.save') }}</button>
                    <button class="btn btn-sm btn-secondary ms-2" @click="proceed(true)" v-if="newlyAdded" :disabled="!canSave()"><i class="las la-save"></i> {{t('operation.general.saveAndAddNext')}}</button>
                </div>
            </div>
        </template>
    </modal-widget>
</template>