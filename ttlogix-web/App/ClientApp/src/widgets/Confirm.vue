<template>
    <modal containerClass="modal-confirm">
        <template name="header" v-slot:header>
            {{title ? title : $t('widgets.confirm.question')}}
        </template>
        <template name="body" v-slot:body>
            <slot></slot>
        </template>
        <template name="footer" v-slot:footer>
            <awaiting-button @proceed="(callback) => { disableNo(); $emit('ok', callback) }"
                             :btnText="'operation.general.yes'"
                             :btnIcon="'las la-check-circle'"
                             :btnClass="'btn-primary'">
            </awaiting-button>

            <button class="btn btn-sm btn-secondary" type="button" @click.stop="$emit('close')" :disabled="noDisabled">
                <i class="las la-times"></i> {{$t('operation.general.no')}}
            </button>
        </template>
    </modal>
</template>
<script>
    import { defineComponent } from 'vue';
    import AwaitingButton from '@/widgets/AwaitingButton'
    export default defineComponent({
        components: { AwaitingButton },
        props: ['subtitle', 'title'],
        data() {
            return {
                noDisabled: false
            }
        },
        methods: {
            disableNo() {
                this.noDisabled = true;                
            }
        }
    })
</script>
<style lang="scss">
.modal-confirm {
    .modal-container {
        width: 500px;
    }
}
</style>