<script lang="ts" setup>
    // import vue stuff
    import { computed, ref, watch } from 'vue';
    import { useI18n } from 'vue-i18n';
    import { useStore } from '@/store';

    // import types
    import { Modal } from '@/store/commonModels/modal';
    import { ActionTypes } from '@/store/action-types';
    import { Warehouse } from '../../../store/models/settings/warehouse';

    // modals and widgets
    import ModalWidget from '@/widgets/Modal3.vue';
    import SelectCountry from '@/widgets/SelectCountry.vue';

    // props & emits
    const props = defineProps<{ modalBase: Modal }>();
    const emit = defineEmits<{ (e: 'close'): void, (e: 'add', updatedWarehouse: Warehouse, addNext: boolean): void, (e: 'update', updatedWarehouse: Warehouse): void }>();

    // local vars
    const store = useStore();
    const { t } = useI18n({ useScope: 'global' });
    const loading = ref(false);
    const currentWarehouse = ref(new Warehouse());
    const currentWarehouseType = ref(false);
    const code = ref();
    const newlyAdded = ref(true);

    // functions
    const reloadItem = async () => {
        loading.value = true;
        await store.dispatch(ActionTypes.SETT_LOAD_WAREHOUSE, { code: code.value })
                   .then((whs) => currentWarehouse.value = whs)
                   .catch(() => loading.value = false);
        loading.value = false;
    }
    const setWarehouseType = () => {
        currentWarehouse.value.type = currentWarehouseType.value ? 0 : 1;
    }
    const canDoNext = () => {
        return !props.modalBase.customProps;
    }
    const canSave = () => {
        return currentWarehouse.value.code && currentWarehouse.value.name && currentWarehouse.value.country;
    }

    const proceed = async (addNext: boolean) => {
        if (newlyAdded.value) emit('add', currentWarehouse.value, addNext)
        else emit('update', currentWarehouse.value)
        resetModal();
    }
    const cancel = async () => {
        emit('close');
        resetModal();
    }
    const resetModal = () => {
        currentWarehouse.value = new Warehouse();
    }

    watch(props.modalBase, async (nV, oV) => {
        if (nV.on == false) { newlyAdded.value = false; resetModal(); }
        if (nV.on == true) {
            if (nV.customProps != null) {
                code.value = nV.customProps;
                newlyAdded.value = false;
                await reloadItem();
                //currentWarehouseType.value = nV.customProps.type == 0 ? true : false;
            } else {
                newlyAdded.value = true;
            }
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
                    <div class="row">
                        <div class="col-md-2 text-end  no-left-padding pt-1">{{t('settings.warehouse.code')}}:</div>
                        <div class="col-md-4 no-left-padding" v-if="newlyAdded">
                            <input type="text" class="form-control form-control-sm" v-model="currentWarehouse.code" :disabled="loading" />
                        </div>
                        <div class="col-md-4 no-left-padding pt-1" v-if="!newlyAdded"><strong>{{ currentWarehouse.code }}:</strong></div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-2 text-end  no-left-padding pt-1">{{t('settings.warehouse.warehouse')}}:</div>
                        <div class="col-md-10 no-left-padding">
                            <input type="text" class="form-control form-control-sm" v-model="currentWarehouse.name" :disabled="loading" />
                        </div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-2 text-end  no-left-padding pt-1">{{t('settings.warehouse.area')}}:</div>
                        <div class="col-md-4 no-left-padding"> 
                            <input type="text" class="form-control form-control-sm" v-model="currentWarehouse.area" :disabled="loading" />
                        </div>
                        <div class="col-md-2 no-left-padding pt-1">m<sup>2</sup></div>
                        <div class="col-md-2 text-end no-right-padding pt-1" v-if="newlyAdded">{{t('settings.warehouse.system')}}:</div>
                        <div class="col-md-2 pt-2" v-if="!props.modalBase.customProps">
                            <input type="checkbox" v-model="currentWarehouseType" @change="setWarehouseType()" :disabled="loading" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="card mt-2">
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-2 text-end pt-1 no-left-padding">{{t('settings.warehouse.address')}}:</div>
                        <div class="col-md-10 no-left-padding">
                            <input type="text" class="form-control form-control-sm" v-model="currentWarehouse.address1" :disabled="loading" />
                        </div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-2 text-end pt-1 no-left-padding"></div>
                        <div class="col-md-10 no-left-padding">
                            <input type="text" class="form-control form-control-sm" v-model="currentWarehouse.address2" :disabled="loading" />
                        </div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-2 text-end pt-1 no-left-padding"></div>
                        <div class="col-md-10 no-left-padding">
                            <input type="text" class="form-control form-control-sm" v-model="currentWarehouse.address3" :disabled="loading" />
                        </div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-2 text-end pt-1 no-left-padding"></div>
                        <div class="col-md-10 no-left-padding">
                            <input type="text" class="form-control form-control-sm" v-model="currentWarehouse.address4" :disabled="loading" />
                        </div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-2 text-end pt-1 no-left-padding">{{t('settings.warehouse.postCode')}}:</div>
                        <div class="col-md-10 no-left-padding">
                            <input type="text" class="form-control form-control-sm" v-model="currentWarehouse.postCode" :disabled="loading" />
                        </div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-2 text-end pt-1 no-left-padding">{{t('settings.warehouse.country')}}:</div>
                        <div class="col-md-5 no-left-padding multiselect-as-select">
                            <select-country v-model="currentWarehouse.country" :disabled="loading"></select-country>
                        </div>
                    </div>
                </div>
            </div>
            <div class="card mt-2">
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-2 text-end no-left-padding pt-1">{{t('settings.warehouse.pic')}}:</div>
                        <div class="col-md-10 no-left-padding">
                            <input type="text" class="form-control form-control-sm" v-model="currentWarehouse.pic" :disabled="loading" />
                        </div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-2 text-end pt-1 no-left-padding">{{t('settings.warehouse.tel')}}:</div>
                        <div class="col-md-4 no-left-padding">
                            <input type="text" class="form-control form-control-sm" v-model="currentWarehouse.telNo" :disabled="loading" />
                        </div>
                        <div class="col-md-2 text-end pt-1 no-left-padding">{{t('settings.warehouse.fax')}}:</div>
                        <div class="col-md-4 no-left-padding">
                            <input type="text" class="form-control form-control-sm" v-model="currentWarehouse.faxNo" :disabled="loading" />
                        </div>
                    </div>
                </div>
            </div>
        </template>
        <template name="footer" v-slot:footer>
            <div class="row">
                <div class="col-md-12 text-end">
                    <button class="btn btn-sm btn-secondary" @click="cancel"><i class="las la-ban"></i> {{ t('operation.general.cancel') }}</button>
                    <button class="btn btn-sm btn-secondary ms-2" @click="proceed(false)" :disabled="!canSave() || loading"><i class="las la-save"></i> {{ t('operation.general.save') }}</button>
                    <button class="btn btn-sm btn-secondary ms-2" @click="proceed(true)" v-if="canDoNext()" :disabled="!canSave() || loading"><i class="las la-save"></i> {{t('operation.general.saveAndAddNext')}}</button>
                </div>
            </div>
        </template>
    </modal-widget>
</template>