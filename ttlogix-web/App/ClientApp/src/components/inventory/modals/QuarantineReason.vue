<script lang="ts" setup>
    import { Modal } from '@/store/commonModels/modal';
    import ModalWidget from '@/widgets/Modal3.vue';
    import { ref, watch } from 'vue';
    import { Quarantine } from '../../../store/models/quarantine';

    const props = defineProps<{ modalBase: Modal }>();
    const emit = defineEmits<{ (e: 'close'): void, (e: 'updateReason', pids: Array<string>, reason: string): void }>();

    const pids = ref<Array<string>>([]);
    const reason = ref('');
    const oldReasons = ref<Array<string>>([]);

    watch(() => props.modalBase.customProps, (nVal:Array<Quarantine>, oVal) => {
        pids.value = nVal ? nVal.map(q => q.pid) : new Array<string>();
        // reason.value = nVal ? nVal.map(r => r.reason) : new Array<string>();
        oldReasons.value = nVal ? nVal.map(r => r.reason) : new Array<string>();
    });

    const executeReasonChange = () => {
        emit('updateReason', pids.value, reason.value);
    }

</script>

<template>
    <modal-widget :modalBase=modalBase @close="emit('close')">
        <template name="title" v-slot:title>
            <h5>{{modalBase.title}}</h5>
        </template>
        <template name="body" v-slot:body>
            <div>
                <div class="row">
                    <div class="col" v-for="pid in pids" :key="pid">{{ pid }}</div>
                </div>
            </div>
            <div>{{oldReasons.length == 1 ? oldReasons[0] ? ('Old reason: ' + oldReasons[0]) : 'Old reason: -none given-' : ''}}</div>
            <div class="mt-4">
                <div>Reason:</div>
                <textarea class="form-control form-control-sm" v-model="reason"></textarea>
            </div>
        </template>
        <template name="footer" v-slot:footer>
            <div class="text-end">
                <button class="btn btn-sm btn-primary"
                        @click="executeReasonChange">
                    <i class="las la-pen"></i> Update
                </button>
            </div>
        </template>
    </modal-widget>
</template>