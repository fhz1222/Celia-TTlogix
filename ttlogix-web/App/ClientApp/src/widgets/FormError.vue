<template>
   <div class="form-error" v-show="fieldErrors">
       <span v-for="(v,k) in fieldErrors" :key="k">{{v}}</span>
   </div>
</template>

<script>

import { defineComponent } from 'vue';
export default defineComponent({
    props: ['errors', 'field'],
    
        computed: {
            fieldErrors() {
                try {
                    var key, keys = Object.keys(this.errors);
                    var n = keys.length; var errorsTL = {}
                    while (n--) {
                        key = keys[n];
                        errorsTL[key.toLowerCase()] = this.errors[key];
                    }
                    if (errorsTL && errorsTL[this.field.toLowerCase()]) {
                        let arr = []
                        if (Array.isArray(errorsTL[this.field.toLowerCase()])) {
                            arr = errorsTL[this.field.toLowerCase()]
                        }
                        else {
                            arr = [errorsTL[this.field.toLowerCase()]]
                        }
                        return arr.map(eStr => {
                            try {
                                let e = JSON.parse(eStr)
                                if (e.MessageKey.toLowerCase()) {
                                    return this.$te(`error.${e.MessageKey}`)
                                        ? this.$t(`error.${e.MessageKey}`, e.Arguments)
                                        : e.MessageKey
                                } else {
                                    return this.$te(`error.${e}`)
                                        ? this.$t(`error.${e}`)
                                        : e
                                }
                            } catch {
                                return this.$t(`error.${eStr}`)
                            }
                        })

                    }
                    return null;
                } catch (ex) {
                    return null
                }
            }
        }
})
</script>

<style lang="scss" scoped>
 div {
    box-sizing: border-box;
    color: #ff3600;
    width: 100%;
    span {
        display: block;
        font-size: 12px;
    }
 }
</style>