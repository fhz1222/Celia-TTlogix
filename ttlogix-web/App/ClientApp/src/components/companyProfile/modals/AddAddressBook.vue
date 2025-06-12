<script lang="ts" setup>
    // import vue stuff
    import { ref, watch } from 'vue';
    import { useI18n } from 'vue-i18n';

    // import types
    import { Modal } from '@/store/commonModels/modal';
    import { CompanyProfileAddressBookShort } from '../../../store/models/companyProfileAddressBook';

    // modals and widgets
    import ModalWidget from '@/widgets/Modal3.vue';
    import SelectCountry from '@/widgets/SelectCountry.vue';
    
    // props & emits
    const props = defineProps<{ modalBase: Modal }>();
    const emit = defineEmits<{ (e: 'close'): void, (e: 'proceed', currentAddressBook: CompanyProfileAddressBookShort, addNext: boolean): void }>();

    // local vars
    const { t } = useI18n({ useScope: 'global' });
    const currentAddressBook = ref(new CompanyProfileAddressBookShort);

    // functions
    const canSave = () => {
        return currentAddressBook.value.country && (currentAddressBook.value.address1 || currentAddressBook.value.address2 || currentAddressBook.value.address3 || currentAddressBook.value.address4);
    }
    const proceed = async (addNext: boolean) => {
        emit('proceed', currentAddressBook.value, addNext);
        resetModal();
    };
    const cancel = async () => {
        emit('close');
        resetModal();
    }
    watch(props.modalBase, (newV, oldV) => {
        currentAddressBook.value.companyCode = newV.customProps
        if(newV.on == false) resetModal();
    })

    const resetModal = () => {
        currentAddressBook.value = new CompanyProfileAddressBookShort();
        currentAddressBook.value.companyCode = props.modalBase.customProps;
    }
</script>

<template>
    <modal-widget :modalBase=modalBase @close="emit('close', false)">
        <template name="title" v-slot:title>
            <h5>{{modalBase.title}}</h5>
        </template>
        <template name="body" v-slot:body>
            <div class="row">
                <div class="col-md-3 no-right-padding text-end">{{t('companyProfile.companyCode')}}</div>
                <div class="col-md-9"><strong>{{currentAddressBook.companyCode}}</strong></div>
            </div>
            <div class="row mt-3">
                <div class="col-md-3 no-right-padding text-end pt-1">{{t('companyProfile.address')}} *</div>
                <div class="col-md-9">
                    <input type="text" class="form-control form-control-sm" v-model="currentAddressBook.address1" :maxlength="50" />
                </div>
            </div>
            <div class="row mt-2">
                <div class="col-md-3 no-right-padding text-end pt-1"></div>
                <div class="col-md-9">
                    <input type="text" class="form-control form-control-sm" v-model="currentAddressBook.address2" :maxlength="50" />
                </div>
            </div>
            <div class="row mt-2">
                <div class="col-md-3 no-right-padding text-end pt-1"></div>
                <div class="col-md-9">
                    <input type="text" class="form-control form-control-sm" v-model="currentAddressBook.address3" :maxlength="50" />
                </div>
            </div>
            <div class="row mt-2">
                <div class="col-md-3 no-right-padding text-end pt-1"></div>
                <div class="col-md-9">
                    <input type="text" class="form-control form-control-sm" v-model="currentAddressBook.address4" :maxlength="50" />
                </div>
            </div>
            <div class="row mt-2">
                <div class="col-md-3 no-right-padding text-end pt-1">{{t('companyProfile.postalCode')}}</div>
                <div class="col-md-2">
                    <input type="text" class="form-control form-control-sm" v-model="currentAddressBook.postCode" :maxlength="6" />
                </div>
                <div class="col-md-2 no-left-padding text-end pt-1">{{t('companyProfile.country')}} *</div>
                <div class="col-md-5 no-left-padding multiselect-as-select">
                    <select-country v-model="currentAddressBook.country"></select-country>
                </div>
            </div>
            <div class="row mt-2">
                <div class="col-md-3 no-right-padding text-end pt-1">{{t('companyProfile.telNo')}}</div>
                <div class="col-md-4">
                    <input type="text" class="form-control form-control-sm" v-model="currentAddressBook.telNo" :maxlength="20" />
                </div>
                <div class="col-md-1 no-right-padding text-end pt-1">{{t('companyProfile.faxNo')}}</div>
                <div class="col-md-4">
                    <input type="text" class="form-control form-control-sm" v-model="currentAddressBook.faxNo" :maxlength="20" />
                </div>
            </div>
        </template>
        <template name="footer" v-slot:footer>
            <div class="row">
                <div class="col-md-12 text-end">
                    <button class="btn btn-sm btn-secondary me-2" @click="cancel">{{ t('operation.general.cancel') }}</button>
                    <button class="btn btn-sm btn-primary me-2" @click="proceed(false)" :disabled="!canSave()"><i class="las la-save"></i> {{ t('operation.general.save') }}</button>
                    <button class="btn btn-sm btn-primary" @click="proceed(true)" :disabled="!canSave()"><i class="las la-save"></i> {{t('companyProfile.saveNext')}}</button>
                </div>
            </div>
        </template>
    </modal-widget>
</template>