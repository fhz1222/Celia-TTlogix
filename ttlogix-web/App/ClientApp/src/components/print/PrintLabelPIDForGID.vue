<template>
    <print-label-feature :all="all" :selected="selected" :print="print" :printPDF="printPDF" :title="$t('inbound.detail.printLabelModalTitle')"
        @done="$emit('done')" @close="$emit('close')" />
</template>
<script>
import PrintLabelFeature from './PrintLabel'
import {downloadPIDLabels} from './pdf_label_generator'
import { defineComponent } from 'vue';
export default defineComponent({
    components: { PrintLabelFeature },
    props: ['selected', 'all'],
    methods: {
        print(pid, printerIP, labelSize) {
            return this.$dataProvider.storageGroup.printPIDLabels(pid, printerIP, labelSize);
        },
        printPDF(pid) {
            return new Promise((resolve, reject) => {
                this.$dataProvider.storageGroup.getStorageLabelsForGIDs(pid).then((labels) => {
                    downloadPIDLabels(labels);
                    resolve()
                }).catch(e => reject(e))
            })
        }
    }
})
</script>