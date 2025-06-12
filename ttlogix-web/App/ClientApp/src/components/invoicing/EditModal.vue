<script setup lang="ts">
import Multiselect from '@vueform/multiselect'
import { Modal } from '@/store/commonModels/modal';
import ModalWidget from '../../widgets/Modal3.vue';
import AwaitingButton from '../../widgets/AwaitingButton.vue';
import { computed, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();

const props = defineProps<{ data: {batchId: number, comment: string, truckDeparture: number, disabled: boolean, onSave: (batchId: number, comment: string, truckDeparture: number, callback: () => void) => Promise<void> } | null }>();

const emit = defineEmits<{
    (e: 'close'): void,
}>();

const modalBase = computed(() => {
    return { ...new Modal(700), on: !!props.data?.batchId }
});
const comment = ref('')
const truckDeparture = ref<number>()
watch(props, () => {
    comment.value = props.data?.comment ?? ''
    truckDeparture.value = props.data?.truckDeparture
})

const save = async (callback: () => void) => {
    await props.data!.onSave(props.data!.batchId, comment.value, truckDeparture.value!, callback)
}

const from = 9
const to = 16;
const options = [...Array(to - from + 1).keys()].map(x => {
    const t = from + x
    return {
        label: `${t.toString().padStart(2, '0')}:00`,
        value: t
    }
})

var offset = 0;

const scroll = (data: WheelEvent) => {
    if (props.data?.disabled) return;
    offset += data.deltaY
    if (Math.abs(offset) > 20) {
        const i = options.findIndex(x => x.value == truckDeparture.value)
        const newI = Math.min(Math.max(i + Math.sign(offset), 0), options.length - 1);
        truckDeparture.value = options[newI].value
        offset = 0
    }
}

</script>

<template>
    <ModalWidget :modalBase="modalBase" @close="emit('close')">
        <template #title>
            <h5>Edit info for customs agency</h5>
        </template>
        <template #body>
            <div class="row">
                <div class="col-3 mt-2 text-end pt-1 no-right-padding">
                    {{ t('invoicing.truckdeparture') }}
                </div>
                <div class="col-9 mt-2 multiselect-as-select" @wheel.stop.prevent="scroll">
                    <!-- <input type="number" class="form-control form-control-sm" v-model="truckDeparture"
                        :disabled="disabled" /> -->
                    <multiselect :options="options" v-model="truckDeparture" :can-clear="false" :disabled="data?.disabled" :classes="{container: 'form-control-sm multiselect'}"></multiselect>

                </div>
            </div>
            <div class="row">
                <div class="col-3 mt-2 text-end pt-1 no-right-padding">
                    {{ t('invoicing.comment') }}
                </div>
                <div class="col-9 mt-2">
                    <textarea class="form-control form-control-sm" v-model="comment" style="height: 100%"
                        :disabled="data?.disabled"></textarea>
                </div>
            </div>
        </template>
        <template #footer>
            <div class="text-end">
                <awaiting-button @proceed="save" :btnText="'saveButton'" :btnIcon="'las la-save'" :btnClass="'btn-primary'">
                </awaiting-button>
            </div>
        </template>
    </ModalWidget>
</template>