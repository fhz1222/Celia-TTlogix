<template>
    <div class="small-font">
        <modal containerClass="modal-create-inbound-increase">
            <template name="header" v-slot:header>
                {{ title }}
            </template>
            <template name="body" v-slot:body>

                <!-- Print option -->
                <div><strong>{{ $t('inbound.detail.printLabelPrintOptionTitle') }}</strong></div>
                <div class="card">
                    <div class="card-body">
                        <!-- All -->
                        <div class="row" v-if="!showSelectedOnly()">
                            <div class="col-md-12">
                                <div class="form-check">
                                    <input class="form-check-input" type="radio" value="all" v-model="mode"
                                        name="printOption" id="printOption1">
                                    <label class="form-check-label" for="printOption1">
                                        {{
                                                $t('inbound.detail.printLabelPrintAll')
                                        }}
                                    </label>
                                </div>
                            </div>
                        </div>

                        <!-- PID -->
                        <div class="row" v-if="!showSelectedOnly()">
                            <div class="col-md-3">
                                <div class="form-check pt-2">
                                    <input class="form-check-input" type="radio" value="pid" v-model="mode"
                                        name="printOption" id="printOption3">
                                    <label class="form-check-label" for="printOption3">
                                        {{
                                                $t('inbound.detail.printLabelPrintPID')
                                        }}
                                    </label>
                                </div>
                            </div>
                            <div class="col-md-9">
                                <input type="text" class="form-control" :disabled="mode != 'pid'" v-model="pid" />
                            </div>
                        </div>
                        <!-- Selected -->
                        <div class="row">
                            <div class="col">
                                <div class="form-check">
                                    <input class="form-check-input" type="radio" value="selected" v-model="mode"
                                        name="printOption" id="printOption2" :disabled="selected == null">
                                    <label class="form-check-label" for="printOption2">
                                        {{
                                                $t('inbound.detail.printLabelPrintSelected')
                                        }}
                                    </label>
                                </div>
                            </div>
                        </div>
                        <div class="row" v-if="mode == 'selected'">
                            <div class="col-5">
                                {{ $t('inbound.detail.printLabelPrintSelected') }}
                            </div>
                            <div class="col-2">
                            </div>
                            <div class="col-5">
                                {{ $t('inbound.detail.printLabelPrintAvailable') }}
                            </div>
                        </div>
                        <div class="row" v-if="mode == 'selected'">
                            <div class="col-5">
                                <div class="scrollable-20">
                                    <table
                                        class="table table-striped table-hover align-middle select-pid-table no-selection">
                                        <tbody>
                                            <tr v-for="(row, index) in selectedPids" :key="index"
                                                @click="onSelectedPidClick(row)"
                                                :class="{ 'selected': selectedSelPids.includes(row) }">
                                                <td>{{ row }}</td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                            <div class="col-2 text-center">
                                <div class="row mb-2">
                                    <div class="col">
                                        <button class="btn btn-sm btn-primary me-2" @click="select()"
                                            @dblclick="deselectOne(row)">
                                            <i class="las la-arrow-left"></i>
                                        </button>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col">
                                        <button class="btn btn-sm btn-primary me-2" @click="deselect()">
                                            <i class="las la-arrow-right"></i>
                                        </button>
                                    </div>
                                </div>
                            </div>
                            <div class="col-5">
                                <div class="scrollable-20">
                                    <table
                                        class="table table-striped table-hover align-middle select-pid-table no-selection">
                                        <tbody>
                                            <tr v-for="(row, index) in available" :key="index"
                                                @click="onAvailablePidClick(row)" @dblclick="selectOne(row)"
                                                :class="{ 'selected': availableSelPids.includes(row) }">
                                                <td>{{ row }}</td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>

                <!-- Settings -->
                <div class="row mt-3">
                    <div class="col-md-8">
                        <!-- Settings -->
                        <div><strong>{{ $t('inbound.detail.printLabelPrintSettingsTitle')
                        }}</strong></div>
                        <div class="card">
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-md-3 text-end pt-2 no-right-padding">
                                        {{
                                                $t('inbound.detail.printLabelPrintOutput')
                                        }}
                                    </div>
                                    <div class="col-md-9 multiselect-as-select">
                                        <select-label-printer v-model="printerIP" />
                                        <form-error :errors="errors" field="Printer" />
                                    </div>
                                </div>

                                <div class="row mt-2">
                                    <div class="col-md-3 text-end pt-2 no-right-padding">
                                        {{
                                                $t('inbound.detail.printLabelPrintCopies')
                                        }}
                                    </div>
                                    <div class="col-md-5">
                                        <input type="number" class="form-control" v-model="copies" />
                                        <form-error :errors="errors" field="Copies" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-md-4">
                        <!-- Label size -->
                        <div><strong>{{ $t('inbound.detail.printLabelPrintLabelSizeTitle')
                        }}</strong></div>
                        <div class="card">
                            <div class="card-body">
                                <div class="form-check">
                                    <input class="form-check-input" type="radio" name="labelSize" id="labelSize1"
                                        value="SMALLER" v-model="labelSize">
                                    <label class="form-check-label" for="labelSize1">{{
                                            $t('inbound.detail.printLabelPrintSmaller')
                                    }}</label>
                                </div>
                                <div class="form-check mt-1">
                                    <input class="form-check-input" type="radio" name="labelSize" id="labelSize2"
                                        value="SMALL" v-model="labelSize">
                                    <label class="form-check-label" for="labelSize2">{{
                                            $t('inbound.detail.printLabelPrintSmall')
                                    }}</label>
                                </div>
                                <div class="form-check mt-1">
                                    <input class="form-check-input" type="radio" name="labelSize" id="labelSize3"
                                        value="BIG" v-model="labelSize">
                                    <label class="form-check-label" for="labelSize3">{{
                                            $t('inbound.detail.printLabelPrintOdette')
                                    }}</label>
                                </div>
                            </div>
                            <form-error :errors="errors" field="Type" />
                        </div>
                    </div>
                </div>
            </template>
            <template name="footer" v-slot:footer>
                <button class="btn btn-sm btn-primary" type="button" @click.stop="printToPDF()">
                    <i class="las la-file-alt"></i> {{ $t('inbound.operation.printToPdf') }}
                </button>
                <button class="btn btn-sm btn-primary" type="button" @click.stop="process()">
                    <i class="las la-print"></i> {{ $t('inbound.operation.print') }}
                </button>
                <button class="btn btn-sm btn-secondary" type="button" @click.stop="$emit('close')">
                    <i class="las la-times"></i> {{ $t('inbound.operation.close') }}
                </button>
            </template>
        </modal>


    </div>
</template>
<script>
import FormError from '@/widgets/FormError.vue'
import SelectLabelPrinter from '@/widgets/SelectLabelPrinter.vue'
import { defineComponent } from 'vue';
export default defineComponent({
    components: { SelectLabelPrinter, FormError },
    props: ['selected', 'all', 'title', 'print', 'printPDF', 'selectedOnly'],
        created() {
            this.mode = this.selected.length > 0 ? 'selected' : 'all'
            this.selectedPids = this.selected
        },
    data() {
        return {
            errors: {},
            selectedPids: null,
            selectedSelPids: [],
            availableSelPids: [],
            pid: "",
            mode: null,
            labelSize: window.localStorage.getItem('_last_label_size') || null,
            copies: 1,
            printerIP: window.localStorage.getItem('_last_printer') || null,
            printing: false
        }
    },
    computed: {
        available() {
            return this.all.filter(x => !this.selectedPids.includes(x))
        }
    },
    methods: {
        showSelectedOnly(){ return this.selectedOnly },
        process() {
            if (this.printing) return;
            this.printing = true;
            var pid = this.mode == 'pid' ? [this.pid]
                : this.mode == 'selected' ? this.selectedPids
                : this.mode == 'all' ? this.all : []
            this.print(pid, this.printerIP, this.labelSize, this.copies).then(() => {
                this.printing = false;
                this.$emit('close');
            }).catch(e => {
                if (e.response && e.response.data.errors) {
                    this.errors = e.response.data.errors
                }
                this.printing = false;
            })
            if (this.printerIP) window.localStorage.setItem('_last_printer', this.printerIP)
            if (this.labelSize) window.localStorage.setItem('_last_label_size', this.labelSize)
        },
        deselect() {
            this.selectedPids = this.selectedPids.filter(x => !this.selectedSelPids.includes(x))
            this.endSelect()
        },
        select() {
            this.selectedPids = this.selectedPids.concat(this.availableSelPids)
            this.endSelect()
        },
        selectOne(row) {
            this.selectedPids.push(row)
            this.endSelect()
        },
        deselectOne(row) {
            this.selectedPids = this.selectedPids.filter(x => row != x)
            this.endSelect()
        },
        endSelect() {
            this.$nextTick(() => {
                this.selectedPids.sort((a, b) => this.all.indexOf(a) - this.all.indexOf(b))
                this.availableSelPids = []
                this.selectedSelPids = []
            })
        },
        onAvailablePidClick(pid) {
            this.selectedSelPids = []
            const index = this.availableSelPids.indexOf(pid);
            if (index > -1) {
                this.availableSelPids.splice(index, 1);
            }
            else {
                this.availableSelPids.push(pid)
            }
        },
        onSelectedPidClick(pid) {
            this.availableSelPids = []
            const index = this.selectedSelPids.indexOf(pid);
            if (index > -1) {
                this.selectedSelPids.splice(index, 1);
            }
            else {
                this.selectedSelPids.push(pid)
            }

        },
        async printToPDF() {
            if (this.printing) return;
            this.printing = true;
            var pid = this.mode == 'pid' ? [this.pid]
                : this.mode == 'selected' ? this.selectedPids
                    : this.mode == 'all' ? this.all : [];

            this.printPDF(pid).then(() => {
                this.printing = false;
                this.$emit('close')
            }).catch(e => {
                if (e.response && e.response.data.errors) {
                    this.errors = e.response.data.errors
                }
            })

        }
    }
})
</script>

<style lang="scss">
.modal-create-inbound-increase {
    .modal-container {
        width: 500px;
    }
}
</style>