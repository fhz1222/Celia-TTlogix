<template>
    <confirm @close="$emit('close')" @ok="process">
        {{$t('operation.doYouWantToCancelJobNo', {'jobNo': job.jobNo})}}
    </confirm>
</template>
<script>
import { defineComponent } from 'vue';
export default defineComponent({
    props: ['job'],
    methods: {
        process() {
            this.$dataProvider.outbounds.cancelOutbound(this.job.jobNo).then(() => {
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