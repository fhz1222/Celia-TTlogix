<template>
    <confirm @close="$emit('close')" @ok="process">
        {{$t('loading.cancel.cancelQuestion')}} <strong>{{jobNo}}</strong>?
    </confirm>
</template>
<script>
import { defineComponent } from 'vue';
export default defineComponent({
    props: ['jobNo'],
    methods: {
        process() {
            this.$dataProvider.loadings.cancelLoading(this.jobNo).then(() => {
                this.$notify({
                    title: this.$t('success.Done'),
                    text: this.$t('success.JobCancelled'),
                    type: 'success',
                    group: 'temporary'
                })
                this.$emit('done')
            }).finally(() => {
                this.$emit('close')
            });
        }
    }
})
</script>