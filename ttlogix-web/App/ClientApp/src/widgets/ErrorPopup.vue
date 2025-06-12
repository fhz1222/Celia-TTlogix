<script lang="ts" setup>
    import { Modal } from '@/store/commonModels/modal';
    import ModalWidget from '@/widgets/Modal3.vue';
    import { ref, watch } from 'vue';

    const props = defineProps<{modalBase: Modal}>();
    const emit = defineEmits<{(e: 'close'): void }>();

    const errorTitle = ref<string>();
    const errorMessage = ref<string>();
    const errorStatus = ref<number>();
    
    watch(() => props.modalBase.customProps, (nVal, oVal) => {
        if (nVal) {
            if (nVal.title) {
                var errMess = '';
                if (nVal.errors) {
                    for (const key in nVal.errors) {
                        errMess += nVal.errors[key].join() + '<br /><br />';
                    }
                }
                errorTitle.value = nVal.title;
                errorMessage.value = errMess;
                errorStatus.value = nVal.status;
            } else {
                errorMessage.value = nVal;
            }
        }
    });
</script>

<template>
    <modal-widget :modalBase=modalBase @close="emit('close')">
        <template name="title" v-slot:title>
            <h5>{{ modalBase.title }}: {{ errorStatus }}</h5>
        </template>
        <template name="body" v-slot:body>
            <div class="row mb-2 px-3">
                <div class="col-md-12">
                    <strong v-if="errorMessage && errorMessage.length > 0">{{ errorTitle }}</strong>
                    <span v-if="!errorMessage || errorMessage.length == 0">{{ errorTitle }}</span>
                </div>
            </div>
            <div class="row pt-2 my-2 px-3 scrollable-45" v-if="errorMessage && errorMessage.length > 0">
                <div class="col-md-12">
                    <strong>Error Message:</strong><br />
                    <div v-html="errorMessage"></div>
                </div>
            </div>
        </template>
        <template name="footer" v-slot:footer>
            <div class="text-end">
                <button class="btn btn-sm btn-light" @click="emit('close')">
                    <i class="las la-ban"></i> Close
                </button>
            </div>
        </template>
    </modal-widget>
</template>