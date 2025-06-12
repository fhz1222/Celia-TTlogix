<script lang="ts" setup>
    // import vue stuff
    import { ref, watch } from 'vue';
    import { useI18n } from 'vue-i18n';

    // import types
    import { Modal } from '@/store/commonModels/modal';
    import { CompanyProfileAddressContact } from '../../../store/models/companyProfileAddressContact';

    // modals and widgets
    import ModalWidget from '@/widgets/Modal3.vue';

    // props & emits
    const props = defineProps<{ modalBase: Modal }>();
    const emit = defineEmits<{ (e: 'close'): void, (e: 'proceed', currentAddress: CompanyProfileAddressContact, addNext: boolean): void }>();

    // local vars
    const { t } = useI18n({ useScope: 'global' });
    const currentAddressContact = ref(new CompanyProfileAddressContact());

    // functions
    const proceed = async (addNext: boolean) => {
        emit('proceed', currentAddressContact.value, addNext);
        resetModal();
    };
    const cancel = async () => {
        emit('close');
        resetModal();
    }
    watch(props.modalBase, (newV, oldV) => {
        currentAddressContact.value.addressCode = newV.customProps;
        if(newV.on == false) resetModal();
    })
    const resetModal = () => {
        currentAddressContact.value = new CompanyProfileAddressContact();
        currentAddressContact.value.addressCode = props.modalBase.customProps;
    }
</script>

<template>
    <modal-widget :modalBase=modalBase @close="emit('close', false)">
        <template name="title" v-slot:title>
            <h5>{{modalBase.title}}</h5>
        </template>
        <template name="body" v-slot:body>
            <div class="row">
                <div class="col-md-4 no-right-padding text-end">{{t('companyProfile.addressBook')}}</div>
                <div class="col-md-8"><strong>{{currentAddressContact.addressCode}}</strong></div>
            </div>
            <div class="row mt-2">
                <div class="col-md-4 no-right-padding text-end pt-1">{{t('companyProfile.picName')}} *</div>
                <div class="col-md-8">
                    <input type="text" class="form-control form-control-sm" v-model="currentAddressContact.name" :disabled="currentAddressContact.status == 0" :maxlength="30" />
                </div>
            </div>
            <div class="row mt-2">
                <div class="col-md-4 no-right-padding text-end pt-1">{{t('companyProfile.telNo')}}</div>
                <div class="col-md-4">
                    <input type="text" class="form-control form-control-sm" v-model="currentAddressContact.telNo" :disabled="currentAddressContact.status == 0" :maxlength="20" />
                </div>
            </div>
            <div class="row mt-2">
                <div class="col-md-4 no-right-padding text-end pt-1">{{t('companyProfile.faxNo')}}</div>
                <div class="col-md-4">
                    <input type="text" class="form-control form-control-sm" v-model="currentAddressContact.faxNo" :disabled="currentAddressContact.status == 0" :maxlength="20" />
                </div>
            </div>
            <div class="row mt-2">
                <div class="col-md-4 no-right-padding text-end pt-1">{{t('companyProfile.email')}}</div>
                <div class="col-md-8">
                    <input type="text" class="form-control form-control-sm" v-model="currentAddressContact.email" :disabled="currentAddressContact.status == 0" :maxlength="50" />
                </div>
            </div>
        </template>
        <template name="footer" v-slot:footer>
            <div class="row">
                <div class="col-md-12 text-end">
                    <button class="btn btn-sm btn-secondary me-2" @click="cancel">{{ t('operation.general.cancel') }}</button>
                    <button class="btn btn-sm btn-primary me-2" @click="proceed(false)" :disabled="!currentAddressContact.name"><i class="las la-save"></i> {{ t('operation.general.save') }}</button>
                    <button class="btn btn-sm btn-primary" @click="proceed(true)" :disabled="!currentAddressContact.name"><i class="las la-save"></i> {{t('companyProfile.saveNext')}}</button>
                </div>
            </div>
        </template>
    </modal-widget>
</template>