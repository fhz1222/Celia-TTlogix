<template>
    <modal containerClass="modal-pdf-preview">
        <template name="header" v-slot:header>
            {{title || $t('reports.preview.reportPreview')}}
        </template>
        <template name="body" v-slot:body>
            <iframe v-show="uri" :src="uri" type="application/pdf" ref="frame"></iframe>
            <p v-if="!pdf">{{$t('widgets.pdfViewer.loadingPreview')}}</p>
        </template>
        <template name="footer" v-slot:footer>
   
            <a v-if="pdf" :href="uri" class="a-as-btn button small" type="button" :download="name || 'file.pdf'">
                {{$t('operation.general.download')}}
            </a>
            <button class="button small" type="button" @click.stop="$emit('close')">
                {{$t('operation.general.close')}}
            </button>
        </template>
    </modal>
</template>
<script>
    import Modal from '@/widgets/Modal.vue'
    import { defineComponent } from 'vue';
export default defineComponent({
        props: ['name', 'pdf', 'title'],
        name: 'pdf-preview',
        components: {Modal},
        computed: {
            uri() {
                if (!this.pdf) {
                    return null
                }

                return window.URL.createObjectURL(this.pdf)
            }
        }
    })
</script>
<style lang="scss">
    .modal-pdf-preview {
        .modal-container {
            width: 1000px;
        }
        .modal-body {
            min-height: 70vh;
            overflow: auto;
            display: flex;
            flex-direction: column;
        }
        iframe {
            border: 1px solid silver;
            height: 100%;
            flex: 1;
            overflow: auto;
            width: 100%;
        }
    }
    .a-as-btn, .a-as-btn:visited, .a-as-btn:hover, .a-as-btn:active {
        color: white;
    }
</style>