<script lang="ts" setup>
    import { Modal } from '@/store/commonModels/modal';
    import ModalWidget from '@/widgets/Modal3.vue';
    import { ref, watch } from 'vue';

    const props = defineProps<{modalBase: Modal}>();
    const emit = defineEmits<{(e: 'close'): void }>();

    const uri = ref<string>('');
    
    watch(() => props.modalBase.customProps, (nVal, oVal) => {
        if(nVal){
            const urlCreator = window.URL || window.webkitURL;
            uri.value = urlCreator.createObjectURL(new Blob([nVal.file], {
                type: 'application/pdf'
            }));
        }
    });
</script>

<template>
    <modal-widget :modalBase=modalBase @close="emit('close')">
        <template name="title" v-slot:title>
            <h5>{{modalBase.title}}</h5>
        </template>
        <template name="body" v-slot:body>
            <object :data=uri type="application/pdf" class="pdf-viewer">
                <div>No online PDF viewer installed</div>
            </object>
        </template>
        <template name="footer" v-slot:footer>
            <div class="text-end">
                <a v-if="modalBase.customProps.file" :href="uri" class="btn btn-sm btn-primary" type="button" :download="modalBase.customProps.name || 'file.pdf'">
                    Download
                </a>
            </div>
        </template>
    </modal-widget>
</template>