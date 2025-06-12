<template>
    <multiselect :modelValue="modelValue"
                 @update:modelValue="onInput"
                 :options="options"
                 :close-on-select="true"
                 placeholder="select"
                 :can-clear="false"
                 :can-deselect="false"
                 :classes="{container: 'form-control-sm multiselect'}"
                 :disabled="disabled">
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
            supplierID: { default: null }
        },
        created() {
            if (this.customerCode && this.supplierID) {
                this.$dataProvider.storage.getStorageInJobNosList(this.customerCode, this.supplierID).then(resp => {
                    this.options = resp.map(x => x.jobNo)
                })
            } else {
                return null;
            }
        },
        data() {
            return {
                options: [],
            }
        },
        methods: {
            onInput(v) {
                this.$emit('update:modelValue', v)
            }
        }
    })
</script>