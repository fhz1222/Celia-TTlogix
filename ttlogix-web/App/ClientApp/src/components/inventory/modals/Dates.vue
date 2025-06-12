<script lang="ts" setup>
    import { ref, watch } from 'vue';
    import moment from 'moment';

    import { Modal } from '@/store/commonModels/modal';
    import { ReportFilter } from '@/store/commonModels/reportFilter';
    import { Report } from '@/store/commonModels/report';
    import DatePicker from '@/widgets/DatePicker';

    import ModalWidget from '@/widgets/Modal3.vue';
    
    const props = defineProps<{modalBase: Modal}>();
    const emit = defineEmits<{(e: 'close'): void, (e: 'print', modalToClose:Modal, reportFilter:ReportFilter, report:Report): void }>();

    const reportFilter = ref(new ReportFilter());
    const report = ref<Report | null>(null);
    const bothDates = ref(false);

    watch(() => props.modalBase.customProps, (nVal, oVal) => {
        if(nVal){
            report.value = nVal.report ?? nVal.report;
            bothDates.value = nVal.bothDates;
        } else {
            report.value = null;
            reportFilter.value.dateStart = null;
            reportFilter.value.dateEnd = null;
        }
    });
    const printDisabled = ():boolean => {
        const bothFilled = !reportFilter.value.dateStart || !reportFilter.value.dateEnd;
        return (bothDates.value && bothFilled) || (!bothDates.value && !reportFilter.value.date);
    }
    const print = () => {
        reportFilter.value.dateStart = moment(reportFilter.value.dateStart).format();
        reportFilter.value.dateEnd = moment(reportFilter.value.dateEnd).add(1,'day').format();
        reportFilter.value.date = moment(reportFilter.value.date).add(1,'day').format();
        emit('print', props.modalBase, reportFilter.value, props.modalBase.customProps.report);
    }
</script>

<template>
    <modal-widget :modalBase=modalBase @close="emit('close')">
        <template name="title" v-slot:title>
            <h5>{{modalBase.title}}</h5>
        </template>
        <template name="body" v-slot:body>
            <div class="card mt-2">
                <div class="card-body">
                    <div class="row" v-if="bothDates">
                        <div class="col-md-3 pt-1 text-end">From:</div>
                        <div class="col-md-4">
                            <date-picker :date="reportFilter.dateStart" @set="(date) => {reportFilter.dateStart = date}" />
                        </div>
                        <div class="col-md-1 pt-1 text-end no-left-padding">To:</div>
                        <div class="col-md-4">
                            <date-picker :date="reportFilter.dateEnd" @set="(date) => {reportFilter.dateEnd = date}" />
                        </div>
                    </div>
                    <div class="row" v-if="!bothDates">
                        <div class="col-md-3 pt-1 text-end">Date:</div>
                        <div class="col-md-6">
                            <date-picker :date="reportFilter.date" @set="(date) => {reportFilter.date = date}" />
                        </div>
                    </div>
                </div>
            </div>
        </template>
        <template name="footer" v-slot:footer>
            <div class="text-end">
                <button class="btn btn-sm btn-primary"
                        @click="print"
                        :disabled="printDisabled()">
                        <i class="las la-print"></i> Print
                </button>
            </div>
        </template>
    </modal-widget>
</template>