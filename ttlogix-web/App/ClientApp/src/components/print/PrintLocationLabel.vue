<script lang="ts" setup>
    // import vue stuff
    import { ref, watch } from 'vue';
    import { useI18n } from 'vue-i18n';
    import { useStore } from '@/store';

    // import types
    import { Modal } from '@/store/commonModels/modal';
    import { ActionTypes } from '@/store/action-types';
    import { LocationLabel, LocationQrCode, PdfLocationLabel } from '@/store/models/settings/locationLabel'

    // modals and widgets
    import ModalWidget from '@/widgets/Modal3.vue';
    import SelectLabelPrinter from '@/widgets/SelectLabelPrinter.vue'

    // props & emits
    const props = defineProps<{ modalBase: Modal }>();
    const emit = defineEmits<{ (e: 'close'): void, (e: 'printToPDF', labels: Array<PdfLocationLabel>): void }>();

    // local vars
    const store = useStore();
    const { t } = useI18n({ useScope: 'global' });
    const printing = ref(false);
    const printedLocations = ref(new LocationLabel());
    const qrCodes = ref(new Array<LocationQrCode>());
    const labels = ref(new Array<PdfLocationLabel>());

    // functions
    const print = async () => {
        emit('close');
        await store.dispatch(ActionTypes.SETT_PRINT_LOC_LABELS, { locations: printedLocations.value }).catch(() => emit('close'));
    }
    const canPrint = () => {
        return printedLocations.value.printer && printedLocations.value.copies;
    }
    const canPrintToPdf = () => {
        return printedLocations.value.copies > 0;
    }
    const printToPDF = async () => {
        printing.value = true;
        await store.dispatch(ActionTypes.SETT_GET_LOC_LABELS, { locations: printedLocations.value })
            .then((qrCodesResult) => {
                qrCodes.value = qrCodesResult;
                qrCodes.value.forEach((qr) => {
                    labels.value.push({ code: qr.name, qrCode: qr.code, whsCode: printedLocations.value.codes.find((pl) => pl.code == qr.name)?.whsCode ?? '' });
                })
                emit('printToPDF', labels.value);
            }).catch(() => { printing.value = false; emit('close'); });
        printing.value = false;
        emit('close');
    }
    const resetModal = () => {
        printing.value = false;
        printedLocations.value = new LocationLabel();
        printedLocations.value.copies = 1;
        labels.value = new Array<PdfLocationLabel>();
    }

    watch(props.modalBase, async (nV, oV) => {
        if (nV.on == false) { resetModal(); }
        if (nV.on == true) {
            if (nV.customProps != null) {
                printedLocations.value.codes = nV.customProps;
                printedLocations.value.copies = 1;
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
            <div class="row">
                <div class="col-md-12">
                    <div class="card">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-md-3 text-end no-right-padding pt-1">{{t('settings.location.output')}}:</div>
                                <div class="col-md-9 multiselect-as-select">
                                    <select-label-printer v-model="printedLocations.printer" />
                                </div>
                            </div>
                            <div class="row mt-3">
                                <div class="col-md-3 text-end no-right-padding pt-1">{{t('settings.location.copies')}}:</div>
                                <div class="col-md-3">
                                    <input class="form-control form-control-sm text-end print-copies" type="number" :step="1" :min="1" v-model="printedLocations.copies" />
                                </div>
                            </div>
                            <div class="row" v-if="printing">
                                <div class="col-md-12">
                                    <div class="bars1-wrapper">
                                        <div id="bars1">
                                            <span></span>
                                            <span></span>
                                            <span></span>
                                            <span></span>
                                            <span></span>
                                        </div>
                                        <h6 class="mt-2">{{t('settings.location.generatingQR')}}</h6>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </template>
        <template name="footer" v-slot:footer>
            <div class="row">
                <div class="col-md-12 text-end">
                    <button class="btn btn-sm btn-primary me-2" type="button" :disabled="!canPrintToPdf() || printing" @click="printToPDF()"><i class="las la-file-alt"></i> {{$t('inbound.operation.printToPdf')}}</button>
                    <button class="btn btn-sm btn-primary me-2" type="button" :disabled="!canPrint() || printing" @click="print()"><i class="las la-print"></i> {{$t('inbound.operation.print')}}</button>
                    <button class="btn btn-sm btn-secondary" type="button" :disabled="printing" @click.stop="$emit('close')"><i class="las la-times"></i> {{$t('inbound.operation.close')}}</button>
                </div>
            </div>
        </template>
    </modal-widget>
</template>