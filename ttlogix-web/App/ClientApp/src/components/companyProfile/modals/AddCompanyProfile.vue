<script lang="ts" setup>
    // import vue stuff
    import { ref, watch } from 'vue';
    import { useI18n } from 'vue-i18n';
    
    // import types
    import { Modal } from '@/store/commonModels/modal';
    import { CompanyProfileShort } from '../../../store/models/companyProfile';

    // modals and widgets
    import ModalWidget from '@/widgets/Modal3.vue';

    // props & emits
    const props = defineProps<{ modalBase: Modal }>();
    const emit = defineEmits<{ (e: 'close'): void, (e: 'proceed', currentCompany: CompanyProfileShort, addNext: boolean): void }>();

    // local vars
    const { t } = useI18n({ useScope: 'global' });
    const currentCompany = ref(new CompanyProfileShort);

    // functions
    const proceed = async (addNext: boolean) => {
        emit('proceed', currentCompany.value, addNext);
        resetModal();
    };
    const cancel = async () => {
        emit('close');
        resetModal();
    }
    watch(props.modalBase, (newV, oldV) => {
        if (newV.on == false) resetModal();
    })
    const resetModal = () => { currentCompany.value = new CompanyProfileShort() }
</script>

<template>
    <modal-widget :modalBase=modalBase @close="emit('close', false)">
        <template name="title" v-slot:title>
            <h5>{{modalBase.title}}</h5>
        </template>
        <template name="body" v-slot:body>
            <div class="row">
                <div class="col-md-3 text-end pt-1">{{t('companyProfile.companyCode')}}</div>
                <div class="col-md-3">
                    <input type="text" class="form-control form-control-sm" v-model="currentCompany.code" :maxlength="10" />
                </div>
            </div>
            <div class="row mt-2">
                <div class="col-md-3 text-end pt-1">{{t('companyProfile.companyName')}}</div>
                <div class="col-md-9">
                    <input type="text" class="form-control form-control-sm" v-model="currentCompany.name" :maxlength="50" />
                </div>
            </div>
        </template>
        <template name="footer" v-slot:footer>
            <div class="row">
                <div class="col-md-12 text-end">
                    <button class="btn btn-sm btn-secondary me-2" @click="cancel"><i class="las la-ban"></i> {{ t('operation.general.cancel') }}</button>
                    <button class="btn btn-sm btn-secondary me-2" @click="proceed(false)" :disabled="!currentCompany.code || !currentCompany.name"><i class="las la-save"></i> {{ t('operation.general.save') }}</button>
                    <button class="btn btn-sm btn-secondary" @click="proceed(true)" :disabled="!currentCompany.code || !currentCompany.name"><i class="las la-save"></i> {{t('companyProfile.saveNext')}}</button>
                </div>
            </div>
        </template>
    </modal-widget>
</template>