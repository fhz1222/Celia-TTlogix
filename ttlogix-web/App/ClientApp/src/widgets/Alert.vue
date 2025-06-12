<template>
    <div class="alert modal-alert">
        <modal v-if="!hidden">
            <!--<template name="header" v-slot:header>System information</template>-->
            <template name="body" v-slot:body>
                <p class="text">
                    {{ text }}
                </p>
            </template>
            <template name="footer" v-slot:footer>
                <button class="btn btn-sm btn-primary" type="button" @click="$emit('close'); hidden = true">
                    <i class="las la-check-circle"></i> {{$t('operation.general.ok')}}
                </button>
            </template>
        </modal>
    </div>
</template>
<script>
    import { defineComponent } from 'vue';
export default defineComponent({
        name :'alert',
        props : {
            text : {
                type: String
            }
        },
        data : function() {
            return {
                hidden : false
            }
        }
    })
</script>
<style lang="scss">
.alert div.foot {
    text-align: center;
}
.alert p.text {
    text-align: center;
}
.modal-alert {
    .modal-container {
        width: 500px;
    }
}
</style>