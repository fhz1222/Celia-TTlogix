<script lang="ts" setup>
    // import vue stuff
    import { computed, ref, watch } from 'vue';
    import { useI18n } from 'vue-i18n';
    import { useStore } from '@/store';
    
    // import types
    import { Modal } from '@/store/commonModels/modal';
    import { ActionTypes } from '@/store/action-types';
    import { AreaType } from '../../../store/models/settings/areaType';

    // modals and widgets
    import ModalWidget from '@/widgets/Modal3.vue';

    // props & emits
    const props = defineProps<{ modalBase: Modal }>();
    const emit = defineEmits<{ (e: 'close'): void, (e: 'add', updatedActionType: AreaType, addNext: boolean): void, (e: 'update', updatedActionType: AreaType): void }>();

    // local vars
    const store = useStore();
    const { t } = useI18n({ useScope: 'global' });
    const loading = ref(false);
    const currentAreaType = ref(new AreaType());
    const currentAreaTypeType = ref(false);
    const newlyAdded = ref(false);

    // functions
    const setAreaTypeType = () => {
        currentAreaType.value.type = currentAreaTypeType.value ? 0 : 1;
    }
    const canDoNext = () => {
        return newlyAdded.value;
    }
    const canSave = () => {
        return currentAreaType.value.code && currentAreaType.value.name;
    }

    const proceed = async (addNext: boolean) => {
        if(newlyAdded.value) { emit('add', currentAreaType.value, addNext); resetModal(); }
        else emit('update', currentAreaType.value)
    }
    const cancel = async () => {
        emit('close');
        resetModal();
    }
    const resetModal = () => {
        currentAreaType.value = new AreaType();
    }

    watch(props.modalBase, (nV, oV) => {
        if (nV.on == false) { newlyAdded.value = false; resetModal(); }
        if (nV.on) {
            if (nV.customProps.code != null) {
                newlyAdded.value = false;
                currentAreaType.value = nV.customProps;
                currentAreaTypeType.value = nV.customProps.type == 0 ? true : false;
            } 
            else {
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
                        <div class="col-md-3 text-end no-right-padding pt-1">{{ t('settings.areaType.code') }}:</div>
                        <div class="col-md-4" v-if="newlyAdded">
                            <input type="text" class="form-control form-control-sm" v-model="currentAreaType.code" />
                        </div>
                        <div class="col-md-4 pt-1" v-if="!newlyAdded"><strong>{{ currentAreaType.code }}</strong></div>
                    </div>
                    <div class="row mt-3">
                        <div class="col-md-3 text-end no-right-padding pt-1">{{ t('settings.areaType.areaType') }}:</div>
                        <div class="col-md-9">
                            <input type="text" class="form-control form-control-sm" v-model="currentAreaType.name" />
                        </div>
                    </div>
                    <div class="row mt-3" v-if="newlyAdded">
                        <div class="col-md-3 text-end no-right-padding">{{ t('settings.areaType.system') }}:</div>
                        <div class="col-md-4 pt-1">
                            <input type="checkbox" v-model="currentAreaTypeType" @change="setAreaTypeType()" />
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