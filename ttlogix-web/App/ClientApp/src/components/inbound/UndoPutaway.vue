<template>
    <confirm @close="$emit('close')" @ok="process">
        {{$t('operation.doYouWantToUndoPutaway')}}
    </confirm>
</template>
<script>
import { defineComponent } from 'vue';
export default defineComponent({
    props: ['pids'],
    methods: {
        process() {
            this.$dataProvider.inbounds.undoPutaway(this.pids).then(() => {
                this.$notify({
                    title: this.$t('success.Done'),
                    text: this.$t('success.Completed'),
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