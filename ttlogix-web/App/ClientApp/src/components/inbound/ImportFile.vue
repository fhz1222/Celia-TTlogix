<template>
    <div>
        <modal containerClass="modal-create-inbound-file">
            <template name="header" v-slot:header>
                {{ $t('inbound.operation.importFile.title') }}
            </template>
            <template name="body" v-slot:body>
                <div class="progress" v-if="upload">
                    <div class="progress-bar" role="progressbar" :style="'width: ' + progress + '%'" aria-valuenow="25" aria-valuemin="0" aria-valuemax="100"></div>
                </div>
                <div v-else>
                    <div class="form-row">
                        <label class="form-label">{{$t('inbound.detail.customer')}}</label>
                        <div class="form-input full multiselect-as-select">
                            <select-customer v-model="model.customerCode" :disabled="upload" />
                            <form-error :errors="errors" field="CustomerCode" />
                        </div>
                    </div>
                    <div class="form-row">
                        <label class="form-label">{{$t('inbound.detail.supplier')}}</label>
                        <div class="form-input full">
                            <v-select v-model="model.supplierID" :options="suppliers" :reduce="p => p.supplierID">
                                <template v-slot:option="option">
                                    <span :style="{display: 'inline-block', width: idLength*9 + 'px'}">{{option.supplierID}}</span>
                                    | {{option.companyName}}
                                </template>
                            </v-select>
                            <form-error :errors="errors" field="Supplier" />
                        </div>
                    </div>
                    <div class="custom-file">
                        <input type="file" ref="file" class="custom-file-input" id="importFile" @change="selectedFile" :disabled="upload" />
                        <label class="custom-file-label" for="customFileLang">{{ $t('inbound.operation.importFile.input') }}</label>
                        
                    </div>
                </div>
                
            </template>
            <template name="footer" v-slot:footer>
                <button class="button small" type="button" @click.stop="process()" :disabled="upload">
                    {{$t('inbound.operation.import')}}
                </button>
                <button class="button small" type="button" @click.stop="$emit('close')">
                    {{$t('inbound.operation.close')}}
                </button>
            </template>
        </modal>
    </div>
</template>
<script>
//    import DynamicTable from '@/widgets/Table.vue'
import FormError from '@/widgets/FormError'
import SelectCustomer from '@/widgets/SelectCustomer'
import { defineComponent } from 'vue';
export default defineComponent({
    components: {FormError, SelectCustomer},
    data() {
        return {
            confirmModal: null,
            file: null,
            model: {
                file : null,
                supplierID : null,
                customerCode : null
            },
            suppliers: [],
            progress : 0,
            upload : false,
            errors : {}
        }
    },
    watch: {
        'model.customerCode'(v) {
            this.model.supplierID = null;
            this.$dataProvider.supplierMasters.getList(v).then(resp => {
                this.idLength = Math.max(...resp.map(s => s.supplierID.length))
                this.suppliers = resp.map(s => { return { label: `${s.supplierID} | ${s.companyName}`, ...s } })
            })
        }
    },
    methods: {
        open(row) {
            this.modal = { type: 'bycontainer', row: row }
        },
        selectedFile() {
            if (this.$refs.file && this.$refs.file.files && this.$refs.file.files.length) {
                this.model.file = this.$refs.file.files[0]
            } else {
                this.model.file = null
            }
        },
        
        process() {
            if (!this.model.file || !this.model.supplierID) {
                this.$notify({
                    title: this.$t('error.ErrorOccured'),
                    text: this.$t('inbound.error.select_file'),
                    type: 'error',
                    group: 'temporary'
                })
            } else {
                this.upload = true
                this.$dataProvider.inbounds.importFile(this.model, progressEvent => {
                    var percentCompleted = Math.round((progressEvent.loaded * 100) / progressEvent.total)
                    this.progress = percentCompleted
                }).then(resp => {
                    this.$emit('done', resp)
                    this.upload = false
                }).finally(() => {
                    this.upload = false
                    this.progress = 0
                })
            }
        }
    }
})
</script>
<style lang="scss">
    .modal-create-inbound-file {
        .modal-container

    {
        width: 570px;
    }

    .modal-body {
        max-height: 70vh;
        overflow: auto;
        .table

    {
        padding-top: 0px;
    }

    }
    }
</style>