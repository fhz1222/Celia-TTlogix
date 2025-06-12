<template>
    <modal containerClass="modal-processing">
        <template name="header" v-slot:header></template>
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
        <template name="footer" v-slot:footer></template>
    </modal>
</template>
<script>
    import { defineComponent } from 'vue';
    export default defineComponent({ });
</script>
<style lang="scss">
.modal-processing {
    .modal-container {
        width: 500px;
    }
}
</style>