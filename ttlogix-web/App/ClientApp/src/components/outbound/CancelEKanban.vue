<template>
    <confirm @close="$emit('close')" @ok="process">
        <div>
            <p>{{ $t('operation.doYouWantToDeleteTheseEKanbanOrders') }}</p>
            <ul>
                <li v-for="ekanban in eKanbans" :key="ekanban.orderNo">
                    {{ ekanban.orderNo }}
                </li>
            </ul>
        </div>
    </confirm>
</template>


<script>
    import { defineComponent } from 'vue';

    export default defineComponent({
        props: ['eKanbans'],
        methods: {
            process() {
                const orderNos = this.eKanbans.map(e => e.orderNo);  // 把所有 orderNo 提取出来组成数组
                this.$dataProvider.ekanbans.cancelEKanbans(orderNos)
                    .then(() => {
                        this.$notify({
                            title: this.$t('success.Done'),
                            text: this.$t('success.OrdersDeleted'),
                            type: 'success',
                            group: 'temporary'
                        });
                        this.$emit('done');
                    })
                    .finally(() => {
                        this.$emit('close');
                    });
            }
        }
    });
</script>

