<template>
    <multiselect :modelValue="modelValue" 
                 @update:modelValue="onInput" 
                 :options="options" 
                 mode="multiple" 
                 :close-on-select="false" 
                 placeholder="select" 
                 label="code"
                 :hideSelected="false"
                 :multipleLabel="(n) => `${n.length} selected`">
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
                default: []
            }
        },
        created() {
            this.$dataProvider.customers.getCustomers().then(resp => {
                this.options = resp.map(x => x.code);
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