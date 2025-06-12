<template>
    <multiselect :modelValue="modelValue"
                 @update:modelValue="onInput"
                 :options="options"
                 :close-on-select="true"
                 placeholder="select"
                 :can-clear="false"
                 :can-deselect="false"
                 :classes="{container: 'form-control-sm multiselect'}"
                 :disabled="disabled"
                 :searchable="true">
    </multiselect>
</template>
<script>
    import Multiselect from '@vueform/multiselect'
    import { defineComponent } from 'vue';
export default defineComponent({
        components: { Multiselect },
        props: {
            disabled: {
                type: Boolean,
                default: false
            },
            modelValue: {
                default: null
            },
            customerCode: { default: null },
            stockTransferJobNo: {
                default: null
            }
        },
        created() {
            if (this.customerCode) {
                if (this.stockTransferJobNo) {
                    this.$dataProvider.storage.getStorageSupplierList(this.customerCode).then((resp) => {
                        this.options = resp.map(x => ({value: x.supplierID, label: x.supplierID + ' | ' + x.companyName}));
                    });
                } else {
                    this.$dataProvider.supplierMasters.getList(this.customerCode).then((resp) => {
                        this.options = resp.map(x => ({value: x.supplierID, label: x.supplierID + ' | ' + x.companyName}));
                    });
                }
            } else {
                return null;
            }
        },
        data() {
            return {
                options: []
            }
        },
        methods: {
            onInput(v) {
                this.$emit('update:modelValue', v)
            },
        }
    })
</script>