<script lang="ts" setup>
    // import vue stuff
    import { computed, ref, watch } from 'vue';
    import { useI18n } from 'vue-i18n';
    import { useStore } from '@/store';

    // import types
    import { Modal } from '@/store/commonModels/modal';
    import { ActionTypes } from '@/store/action-types';
    import { ProductCode } from '../../../store/models/settings/productCode';

    // modals and widgets
    import ModalWidget from '@/widgets/Modal3.vue';

    // props & emits
    const props = defineProps<{ modalBase: Modal }>();
    const emit = defineEmits<{ (e: 'close'): void, (e: 'proceed', updatedProductCode: ProductCode, addNext: boolean): void }>();

    // local vars
    const store = useStore();
    const { t } = useI18n({ useScope: 'global' });
    const loading = ref(false);
    const currentProductCode = ref(new ProductCode());
    const currentProductCodeType = ref(false);

    // functions
    const setProductCodeType = () => {
        currentProductCode.value.type = currentProductCodeType.value ? 0 : 1;
    }
    const canDoNext = () => {
        return !props.modalBase.customProps.code;
    }
    const canSave = () => {
        return currentProductCode.value.name;
    }

    const proceed = async (addNext: boolean) => {
        emit('proceed', currentProductCode.value, addNext);
        resetModal();
    }
    const cancel = async () => {
        emit('close');
        resetModal();
    }
    const resetModal = () => {
        currentProductCode.value.code = null;
        currentProductCode.value.name = null;
        currentProductCode.value.type = null;
        currentProductCode.value.status = null;
    }

    watch(props.modalBase, (nV, oV) => {
        if (nV.on == false) resetModal();
        if (nV.on == true) {
            if (nV.customProps != null) {
                currentProductCode.value = nV.customProps;
                currentProductCodeType.value = nV.customProps.type == 0 ? true : false;
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
                    <div class="row mt-2">
                        <div class="col-md-3 text-end no-right-padding pt-1">{{t('settings.productCode.productCode')}}:</div>
                        <div class="col-md-9">
                            <input type="text" class="form-control form-control-sm" v-model="currentProductCode.name" />
                        </div>
                    </div>
                    <div class="row mt-3" v-if="!props.modalBase.customProps.code">
                        <div class="col-md-3 text-end no-right-padding">{{t('settings.productCode.system')}}:</div>
                        <div class="col-md-3 pt-1">
                            <input type="checkbox" v-model="currentProductCodeType" @change="setProductCodeType()" />
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
                    <button class="btn btn-sm btn-secondary ms-2" @click="proceed(true)" v-if="canDoNext()" :disabled="!canSave()"><i class="las la-save"></i> {{t('operation.general.saveAndAddNext')}}</button>
                </div>
            </div>
        </template>
    </modal-widget>
</template>