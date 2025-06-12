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
        },
        created() {
            this.$dataProvider.countries.getCountries().then(resp => {
                this.options = resp.map(x => ({value: x.code, label: x.name}));
            })
        },
        data() {
            return {
                options: []
            }
        },
        methods: {
            onInput(v) {
                this.$emit('update:modelValue', v)
            }
        }
    })
</script>