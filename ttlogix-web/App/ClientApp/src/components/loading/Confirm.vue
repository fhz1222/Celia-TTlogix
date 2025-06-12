<template>
    <div>
        <confirm v-if="jobsNoInv == null || jobsNoInv.length == 0" @close="$emit('close')" @ok="firstAccept">
            {{$t('loading.confirm.confQuestion')}} <strong>{{jobNo}}</strong>?
            <br>
            {{$t('loading.confirm.infoCargoOut')}}

        </confirm>
        <confirm v-else @close="$emit('close')" @ok="process">
            {{$t('loading.confirm.noInvoiceInfo')}} {{jobsNoInv.join(", ")}}
        </confirm>
    </div>
</template>
<script>
    import { defineComponent } from 'vue';
export default defineComponent({
        props: ['jobNo'],
        created() {
            this.jobsNoInv = null
        },
        data() {
            return {
                jobsNoInv: null
            }
        },
    methods: {
        firstAccept() {
            this.$dataProvider.loadings.getBondedStockJobNosWithoutCommInv(this.jobNo)
                .then(r => {
                    this.jobsNoInv = r
                    if (this.jobsNoInv.length == 0) {
                        this.process()
                    }
                })
        },
        process() {
            this.$dataProvider.loadings.confirmLoading(this.jobNo).then(() => {
                //this.$emit('done', resp.data)
                this.$notify({
                    title: this.$t('success.Done'),
                    text: this.$t('success.JobConfirmed'),
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