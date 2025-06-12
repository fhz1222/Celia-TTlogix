<template>
    <multiselect :modelValue="modelValue"
                 @update:modelValue="onInput"
                 :options="options"
                 :close-on-select="true"
                 :can-clear="false"
                 :can-deselect="clearable"
                 placeholder="select"
                 :classes="{container: 'form-control-sm multiselect'}"
                 :disabled="disabled">
    </multiselect>
</template>
<script>
    import Multiselect from '@vueform/multiselect'
    import { defineComponent } from 'vue';
export default defineComponent({
        components: { Multiselect},
        props: {
            clearable: {
                type: Boolean,
                default: false
            },
            disabled: {
                type: Boolean,
                default: false
            },
            modelValue: {
                type: null
            },
        },
        created() {
            this.$dataProvider.currencies.getCurrencies().then(resp => {
                this.options = resp
            })
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