export default {
    watch: {
        //TODO: temporary fix, should be fixed properly after removing RouterTab
        '$route'(to) {
            if (to.name.toLowerCase() == this.$options.name.toLowerCase()) {
                if (this.refresh) this.refresh()
            }
        }
    },
}
