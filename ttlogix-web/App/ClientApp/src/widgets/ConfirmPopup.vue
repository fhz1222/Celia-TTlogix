<script lang="ts" setup>
    import { Modal } from '@/store/commonModels/modal';
    import ModalWidget from '@/widgets/Modal3.vue';

    const props = defineProps<{modalBase: Modal}>();
    const emit = defineEmits<{(e: 'close'): void, (e: 'proceed'): void }>();

    const onProceed = async () => {
        if(props.modalBase.fnMod.callback) {
            await props.modalBase.fnMod.callback();
        }
        emit('proceed');
    }
</script>

<template>
    <modal-widget :modalBase=modalBase @close="emit('close')">
        <template name="title" v-slot:title>
            <h5>{{modalBase.title}}</h5>
        </template>
        <template name="body" v-slot:body>
            <div class="row pt-2 pb-4 px-3">
                <div class="col-md-12" v-html="modalBase.fnMod.message"></div>
            </div>
        </template>
        <template name="footer" v-slot:footer>
            <div class="text-end">
                <button class="btn btn-sm btn-secondary me-3" @click="emit('close')">
                    <i class="las la-ban"></i> Cancel
                </button>
                <button class="btn btn-sm btn-primary" @click="onProceed">
                    <i class="las la-play"></i> Proceed
                </button>
            </div>
        </template>
    </modal-widget>
</template>