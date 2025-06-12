<script lang="ts" setup>
    // import vue stuff
    import { computed, ref, watch } from 'vue';
    import { useI18n } from 'vue-i18n';
    import { useStore } from '@/store';
    
    // import types
    import { Modal } from '@/store/commonModels/modal';
    import { CustomerInventory } from '@/store/models/customerInventory';
    import { ActionTypes } from '@/store/action-types';
    import { ControlProductCode } from '@/store/models/controlProductCode';
    import { CustomerUom } from '../../../store/models/customerUom';

    // modals and widgets
    import ModalWidget from '@/widgets/Modal3.vue';
    import SelectUom from '@/widgets/SelectUOMWithDecimal.vue';

    // props & emits
    const props = defineProps<{ modalBase: Modal }>();
    const emit = defineEmits<{ (e: 'close'): void, (e: 'proceed', updatedUom: CustomerUom, addNext: boolean): void }>();

    // local vars
    const store = useStore();
    const { t } = useI18n({ useScope: 'global' });
    const productCodes = computed(() => store.state.productCodes);
    const controlCodes = computed(() => store.state.controlCodes);
    const loading = ref(false);
    const currentUom = ref(new CustomerUom());
    const customerCode = ref(props.modalBase.customProps?.customerCode);
    const globalUomList = computed(() => store.state.globalUomList);
    const allowedDecimals = [
        { num: 0, label: '0' },
        { num: 1, label: '1' },
        { num: 2, label: '2' },
        { num: 3, label: '3' },
        { num: 4, label: '4' },
        { num: 5, label: '5' },
        { num: 6, label: '6' }
    ];

    // functions
    const canDoNext = () => {
        return !props.modalBase.customProps.mUom
    }
    const canSave = () => {
        return currentUom.value.uom;
    }

    const proceed = async (addNext: boolean) => {
        emit('proceed', currentUom.value, addNext);
        resetModal();
    }
    const cancel = async () => {
        emit('close');
        resetModal();
    }
    const resetModal = () => {
        currentUom.value = new CustomerUom();
        currentUom.value.decimalNum = 0;
    }

    watch(props.modalBase, (nV, oV) => {
        if (nV.on == false) resetModal();
        if (nV.on == true) {
            if (nV.customProps.mUom != null) currentUom.value = nV.customProps.mUom;
            else currentUom.value.decimalNum = 0;
            customerCode.value = props.modalBase.customProps.customerCode;
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
                        <div class="col-md-4 text-end">{{t('customer.popups.modifyUom.customer')}}</div>
                        <div class="col-md-4"><strong>{{props.modalBase.customProps?.customerCode}}</strong></div>
                    </div>
                    <div class="row mt-3">
                        <div class="col-md-4 text-end">{{t('customer.popups.modifyUom.uom')}}</div>
                        <div class="col-md-8 multiselect-as-select">
                            <span v-if="props.modalBase.customProps.mUom"><strong>{{currentUom.name}}</strong></span>
                            <select class="form-select form-select-sm" v-model="currentUom.uom" v-if="!props.modalBase.customProps.mUom">
                                <option v-for="gu,i in globalUomList" :key="i" :value="gu.code">{{gu.label}}</option>
                            </select>
                        </div>
                    </div>
                    <div class="row mt-3">
                        <div class="col-md-4 text-end pt-1">{{t('customer.popups.modifyUom.decimalPlace')}}</div>
                        <div class="col-md-3">
                            <select class="form-select form-select-sm" v-model="currentUom.decimalNum">
                                <option v-for="dcm,i in allowedDecimals" :key="i" :value="dcm.num">{{dcm.label}}</option>
                            </select>
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
                    <button class="btn btn-sm btn-secondary ms-2" @click="proceed(true)" v-if="canDoNext()" :disabled="!canSave()"><i class="las la-save"></i> {{t('companyProfile.saveNext')}}</button>
                </div>
            </div>
        </template>
    </modal-widget>
</template>