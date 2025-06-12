<template>
    <print-label-feature :all="all" :selected="selected" :print="print" :printPDF="printPDF" :title="title" :selectedOnly="selectedOnly"
        @done="$emit('done')" @close="$emit('close')" />
</template>
<script>
import PrintLabelFeature from './PrintLabel'
import {downloadPIDLabels} from './pdf_label_generator'
import { defineComponent } from 'vue';
export default defineComponent({
    components: { PrintLabelFeature },
    props: ['selected', 'all', 'labelType', 'selectedOnly'],
    methods: {
        print(pid, printerIP, labelSize, copies) {
            switch (this.labelType) {
                case 'QRPRINT_STORAGELABEL':
                case 'QRPRINT_STOCKTRANSFERLABEL':
                    return this.$dataProvider.storage.printStorageLabels(pid, printerIP, labelSize, copies);
                default:
                    return new Promise((r) => r());
            }
        },
        printPDF(pid) {
            return new Promise((resolve, reject) => {
                this.$dataProvider.storage.getStorageLabels(pid).then((labels) => {
                    downloadPIDLabels(labels);
                    resolve()
                }).catch(e => reject(e))
            })
        }
    },
    computed: {
        title() {
            switch (this.labelType) {
                case 'QRPRINT_STORAGELABEL': return this.$t('inbound.detail.printLabelStorageLabel');
                case 'QRPRINT_STOCKTRANSFERLABEL': return this.$t('inbound.detail.printLabelStockTransferLabel');
                default: return '';
            }
        }
    }
})
</script>