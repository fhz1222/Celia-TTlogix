<template>
   <div>
     <pulse-loader loading="loading" :color="color" :size="size"></pulse-loader>
   </div>
</template>

<script>
   import PulseLoader from 'vue-spinner/src/PulseLoader.vue';
   import { defineComponent } from 'vue';
export default defineComponent({
    components : {PulseLoader},
    props: {
        size:{
            type: String,
            default: '8px'
        },
        color : {
            type: String,
            default: '#326195'
        }
    }

   })
</script>

<style lang="scss" scoped>
 div {
    box-size: content-box;
    text-align: center;
 }
</style>