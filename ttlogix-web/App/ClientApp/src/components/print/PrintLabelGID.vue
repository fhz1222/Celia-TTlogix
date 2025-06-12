<template>
    <print-label-feature :all="all" :selected="selected" :print="print" :printPDF="printPDF" :title="$t('inbound.detail.printLabelModalTitle')"
        @done="$emit('done')" @close="$emit('close')" />
</template>
<script>
import PrintLabelFeature from './PrintLabel'
import {downloadGIDLabels} from './pdf_label_generator'
import { defineComponent } from 'vue';
export default defineComponent({
    components: { PrintLabelFeature },
    props: ['selected', 'all'],
    methods: {
        print(pid, printerIP, labelSize, copies) {
            return this.$dataProvider.storageGroup.printLabels(pid, printerIP, labelSize, copies);
        },
        printPDF(pid) {
            return new Promise((resolve, reject) => {
                this.$dataProvider.storageGroup.getGroupLabels(pid).then((labels) => {
                    downloadGIDLabels(labels);
                    resolve()
                }).catch(e => reject(e))
            })
        }
    }
})
</script>