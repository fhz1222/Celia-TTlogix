<template>
    <span>
      {{formatted}}
    </span>
</template>
<script>
    import moment from 'moment'
    import { defineComponent } from 'vue';
export default defineComponent({
        props: {
            value: {},
            nbsp: {
                default: null
            }
        },
        computed: {
          formatted() {
            if (!this.value) {
              return ''
                }
            return moment(this.value).format('DD/MM/YYYY HH:mm')
          }
        }
    })
</script>
