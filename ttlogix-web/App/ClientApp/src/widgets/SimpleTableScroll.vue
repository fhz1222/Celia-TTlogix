<template>
    <table class="simple-table" ref="table">
        <thead>
            <tr ref="head">
                <slot name="head"></slot>
            </tr>
        </thead>
        <tbody ref="body" :style="{maxHeight: height}">
            <slot name="body"></slot>
        </tbody>
    </table>
</template>
<script>
    import { defineComponent } from 'vue';
export default defineComponent({
        props: {
            height: {
                default: '100px'
            }
        },
        mounted() {
            if (this.$refs.body.children[0]) {
                let rowColsWs = [...this.$refs.body.children[0].children].map(x => x.clientWidth)
                let headColsWs = [...this.$refs.head.children].map(x => x.clientWidth)
                let colsWs = rowColsWs.map((w, i) => w > headColsWs[i] ? w : headColsWs[i])

                colsWs.forEach((cw, i) => {
                    if (i == colsWs.length - 1) {
                        this.$refs.head.children[i].style.width = '1000px'
                    }
                    else {
                        this.$refs.head.children[i].style.minWidth = (cw * 1.1) + 'px'
                    }
                })
                let rows = [...this.$refs.body.children]
                rows.forEach(r => {
                    colsWs.forEach((cw, i) => {
                        if (i == colsWs.length - 1) {
                            r.children[i].style.width = '1000px'
                        }
                        else {
                            r.children[i].style.minWidth = (cw * 1.1) + 'px'
                        }
                    })
                })
            }
        }
    })
</script>
<style lang="scss" scoped>
    thead, tbody {
        display: block;
    }

    tbody {
        overflow-y: auto; /* Trigger vertical scroll    */
        overflow-x: hidden; /* Hide the horizontal scroll */
    }
</style>