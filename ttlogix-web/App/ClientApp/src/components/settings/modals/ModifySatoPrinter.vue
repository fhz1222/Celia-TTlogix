<script lang="ts" setup>
    // import vue stuff
    import { computed, ref, watch } from 'vue';
    import { useI18n } from 'vue-i18n';
    import { useStore } from '@/store';

    // import types
    import { Modal } from '@/store/commonModels/modal';
    import { ActionTypes } from '@/store/action-types';
    import { SatoPrinter } from '@/store/models/settings/satoPrinter';

    // modals and widgets
    import ModalWidget from '@/widgets/Modal3.vue';

    // props & emits
    const props = defineProps<{ modalBase: Modal }>();
    const emit = defineEmits<{ (e: 'close'): void, (e: 'add', updatedSatoPrinter: SatoPrinter, addNext: boolean): void, (e: 'update', updatedSatoPrinter: SatoPrinter): void }>();

    // local vars
    const store = useStore();
    const { t } = useI18n({ useScope: 'global' });
    const loading = ref(false);
    const currentSatoPrinter = ref(new SatoPrinter());
    const currentSatoPrinterType = ref(false);
    const printerTypes = [{ name: "CL412", type: 0 }, { name: "CL4NX", type: 1 }];
    const newlyAdded = ref(false);

    // functions
    const canDoNext = () => {
        return newlyAdded.value;
    }
    const canSave = () => {
        let check = { ...currentSatoPrinter.value };
        return check.name && check.ip && check.type != null;
    }

    const proceed = async (addNext: boolean) => {
        if (newlyAdded.value) emit('add', currentSatoPrinter.value, addNext)
        else emit('update', currentSatoPrinter.value)
        resetModal();
    }
    const cancel = async () => {
        emit('close');
        resetModal();
    }
    const resetModal = () => {
        currentSatoPrinter.value = new SatoPrinter();
        currentSatoPrinter.value.type = null;
    }

    watch(props.modalBase, (nV, oV) => {
        if (nV.on == false) { newlyAdded.value = false; resetModal(); }
        if (nV.on == true) {
            if (nV.customProps.ip != null) {
                currentSatoPrinter.value = nV.customProps;
                newlyAdded.value = false;
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
                    <div class="row mt-2">
                        <div class="col-md-3 text-end no-right-padding pt-1">IP Address:</div>
                        <div class="col-md-9">
                            <input type="text" class="form-control form-control-sm" v-model="currentSatoPrinter.ip" />
                        </div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-3 text-end no-right-padding pt-1">Printer Name:</div>
                        <div class="col-md-9">
                            <input type="text" class="form-control form-control-sm" v-model="currentSatoPrinter.name" />
                        </div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-3 text-end no-right-padding pt-1">Description:</div>
                        <div class="col-md-9">
                            <input type="text" class="form-control form-control-sm" v-model="currentSatoPrinter.description" />
                        </div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-3 text-end no-right-padding pt-1">Type:</div>
                        <div class="col-md-9">
                            <select class="form-select form-select-sm" v-model="currentSatoPrinter.type">
                                <option v-for="pt,i in printerTypes" :key="i" :value="pt.type">{{pt.name}}</option>
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
                    <button class="btn btn-sm btn-secondary ms-2" @click="proceed(true)" v-if="newlyAdded" :disabled="!canSave()"><i class="las la-save"></i> {{t('operation.general.saveAndAddNext')}}</button>
                </div>
            </div>
        </template>
    </modal-widget>
</template>