<script lang="ts" setup>
    import { Modal } from '@/store/commonModels/modal';
    import ModalWidget from '@/widgets/Modal3.vue';

    const props = defineProps<{modalBase: Modal}>();
    const emit = defineEmits<{(e: 'close'): void, (e: 'addNewAdjustment', jobNo: string | null, customerCode: string | null): void }>();
</script>

<template>
    <modal-widget :modalBase=modalBase @close="emit('close')">
        <template name="title" v-slot:title>
            <h5>{{modalBase.title}}</h5>
        </template>
        <template name="body" v-slot:body>
            <div class="row pt-2 pb-4 px-3">
                <div class="col-md-12">
                    <div class="bars1-wrapper">
                        <div id="bars1">
                            <span></span>
                            <span></span>
                            <span></span>
                            <span></span>
                            <span></span>
                        </div>
                        <h5>Please wait ...</h5>
                    </div>
                </div>
            </div>
        </template>
    </modal-widget>
</template>