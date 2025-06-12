<script setup lang="ts">
import AwaitingButton from '@/widgets/AwaitingButton.vue'
import { useI18n } from 'vue-i18n';

defineProps<{
    status: string,
    canBeBlocked: boolean,
    canBeUnblocked: boolean,
    canBeRequestedNow: boolean
}>();

const emit = defineEmits<{
    (e: 'block', callback: () => void): void,
    (e: 'unblock', callback: () => void): void,
    (e: 'requestNow', callback: () => void): void
}>();

const { t } = useI18n({ useScope: 'global' });

</script>

<template>
    <div class="card">
        <div class="card-header">
            <h5>{{ t('invoicing.invoicingTitle') }}</h5>
        </div>
        <div class="card-body">
            <div class="row mb-2">
                <div class="col-4 text-end pt-1 no-right-padding">{{t('invoicing.statusTitle')}}</div>
                <div class="col-6">
                    <input type="text" class="form-control form-control-sm" :value="t(`invoicing.status.${status}`)" disabled />
                </div>
            </div>
            <div class="row">
                <div class="col px-4">
                    <awaiting-button v-if="!canBeUnblocked" :enabled="canBeBlocked" @proceed="(callback) => { $emit('block', callback) }"
                        :btnText="'invoicing.block'" :btnIcon="'las la-times-circle'" :btnClass="'btn-danger w-100'">
                    </awaiting-button>
                    <awaiting-button v-if="canBeUnblocked" @proceed="(callback) => { $emit('unblock', callback) }"
                        :btnText="'invoicing.unblock'" :btnIcon="'las la-times-circle'" :btnClass="'btn-secondary w-100'">
                    </awaiting-button>
                </div>
                <div class="col px-4">
                    <awaiting-button :enabled="canBeRequestedNow" @proceed="(callback) => { $emit('requestNow', callback) }"
                        :btnText="'invoicing.requestNow'" :btnIcon="'las la-envelope-open-text'" :btnClass="'btn-success w-100'">
                    </awaiting-button>
                </div>
            </div>
            
        </div>
    </div>
</template>