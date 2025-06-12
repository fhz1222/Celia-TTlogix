<script lang="ts" setup>
    import { Modal } from '@/store/commonModels/modal';
    import ModalWidget from '@/widgets/Modal3.vue';

    const props = defineProps<{modalBase: Modal}>();
    const emit = defineEmits<{(e: 'close'): void }>();

    const close = async () => {
        emit('close');
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
                <button class="btn btn-sm btn-primary" @click="close">
                    <i class="las la-check-circle"></i> Ok
                </button>
            </div>
        </template>
    </modal-widget>
</template>