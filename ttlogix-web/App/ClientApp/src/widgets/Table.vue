<template>
    <div class="v-table" @click.prevent="">
        <div class="v-table-header">
            <div class="v-col checkbox-col" v-if="multiple">
                <input type="checkbox" :checked="(selection.length == results.data.length)"  @click.stop="selectAll()" />
            </div>
            <template v-for="(col, index) in columns" :key="index">
                <span class="space" v-if="index > (multiple ? -1 : 0)" :key="index + 'space'"></span>
                <div v-tooltip="col.abbrev ? col.title : null" :class="'v-col ' + (model.orderBy == col.data ? 'sorted' : '')" :style="colStyle(index)" @click="sort(index)">
                    <slot :name="'title-' + col.data" :col="col">
                        {{ col.abbrev ? col.abbrev : col.title }}
                    </slot>
                </div>
            </template>
        </div>
        <div class="v-table-header table-header-search">
            <div class="v-col checkbox-col" v-if="multiple">
                
            </div> 
            <template v-for="(col, index) in columns" :key="index">
                <span class="space" v-if="index  > (multiple ? -1 : 0)" :key="index + 'space'"></span>
                <div :class="'v-col '" :style="colStyle(index)">
                    <span v-if="!col.filter" class="no-filter"></span>
                    <slot v-else :name="'filter-' + col.data" :filter="filter">
                        <input type="text" placeholder="..." class="filter" v-model="model[col.data]" @keyup.enter.stop="filter({})" />
                    </slot>
                </div>
            </template>
        </div>
        <div class="v-table-body">
            <template v-for="(row, index) in results.data" :key="index">
                <div class="v-table-row"  :style="[rowColor(row,index) ? {'background': rowColor(row,index)} : {}]" @click.prevent.stop="rowClick(row, $event)" @dblclick.prevent.stop="rowDblClick(row)">
                    <div class="table-columns">
                        <div class="v-col checkbox-col" v-if="multiple">
                            <input type="checkbox" :checked="selection.includes(row)" @click.stop="rowClick(row, $event)" />
                        </div>
                        <template v-for="(col, colx) in colIds" :key="colx + ':' + index">
                            <span class="space" v-if="colx > (multiple ? -1 : 0)" :key="colx + 's' + index"></span>
                            <div class="v-col" :style="colStyle(colx)">
                                <slot :name="col" v-bind:col="col" v-bind:row="row" v-bind:value="row[col]">
                                    {{ row[col] }}
                                </slot>
                            </div>
                        </template>
                    </div>
                    <div class="table-rest"><slot name="expanded" v-bind:row="row"></slot></div>
                    <div class="table-actions" :key="'act' + index" v-if="actions && selection.includes(row)" @click.prevent.stop="">
                        <slot name="actions" v-bind:row="row"></slot>
                    </div>
                    <div class="table-actions overlap" :key="'act' + index" v-if="hoverActions" @click.prevent.stop="">
                        <slot name="hoverActions" v-bind:row="row"></slot>
                    </div>
                </div>
                
            </template>
        </div>
        <div class="text-center mt-3 mb-2">
            <div v-if="results.total > model.pageSize && results.data.length != results.total">
                <awaiting-button v-if="results.total > model.pageSize && results.data.length != results.total"
                                 @proceed="(callback) => { loadNextBatch(results.data.length).finally(callback) }"
                                 :btnText="'operation.dynamicTable.loadMore'"
                                 :btnIcon="'las la-chevron-down'"
                                 :btnClass="'btn-primary'">
                </awaiting-button>
                <br />
                <span class="me-2" v-html="$t('operation.dynamicTable.showingBatch', {dataLength:results.data.length, total:results.total })"></span>
            </div>
            <div v-if="results.total > model.pageSize && results.data.length == results.total && results.total != 0">
                <span v-html="$t('operation.dynamicTable.showingAll', {total:results.total})"></span>
            </div>
            <div v-if="results.total == 0">
                <span>{{ $t('operation.dynamicTable.nothingFound') }}</span>
            </div>
        </div>
    </div>
</template>
<script>
    import { defineComponent } from 'vue';
    import AwaitingButton from '@/widgets/AwaitingButton';
    export default defineComponent({
    components: { AwaitingButton },
    props: {
        func: Function,
        rowColor: {
            type: Function,
            default: () => null
        },
        url : String,
        columns: Array,
        filters: Object,
        search: null,
        actions: {
            default: true
        },
        hoverActions: {
            default: false
        },
        select: {
            type: Boolean,
            default: true
        },
        multiple: {
            type: Boolean,
            default: false
        }
    },
    data() {
        let ids = []
        this.columns.forEach(c => ids.push(c.data))
        let filters = this.filters||{}, defs = {
                orderBy : null,
                desc :false,
                pageNo : 1,
                pageSize: 50
        };
       
        return {
            colIds : ids,
            model : {...defs, ...filters},
            
            selection : [],
            results : {
                data : [],
                pageNo : 0,
                total: 0,
                pageSize : filters.pageSize||50
            },
            loading: false
        }
    },
    computed: {
        pages() {
            if (this.results.total == 0) return 1
            return Math.ceil(this.results.total / this.results.pageSize)
        },
        hasMore() {
            if (this.results.pageNo ==0) {
                return false
            }

            return this.pages > this.results.pageNo 
        },
        pageSize() {
            return this.results.data.length;
        }
    },
    mounted() {
        this.load()
    },
    methods: {
        refresh() {
            this.model.pageNo = 1
            return this.load()
        },
        filter(filters) {
            this.model = {...this.model, ...filters}
            this.model.pageNo = 1
            return this.load()
        },
        colStyle(idx) {
            let style = ''
            if (idx+1 == this.columns.length) {
                style = ''
            } else {
                style = 'width:' + this.columns[idx].width + 'px;'
            }

            return style
        },
        load() {
            this.loading = true
            return this.func(this.model)
                .then(r => {
                    this.results = r
                    this.selection = []
                    this.$emit('table-data-loaded');
                    this.loading = false
                    return r
            })
        },
        loadNextBatch(dataLength) {
            this.loading = true;
            this.model.pageNo = Math.ceil(dataLength / this.model.pageSize) + 1;
            return this.func(this.model)
                .then(r => { r.data.forEach((row) => { this.results.data.push(row) }); this.loading = false; })
        },
        sort(id) {
            if (!this.columns[id].sortable) {
                return
            }
            let data = this.columns[id].data
            if (this.model.orderBy == data) {
                this.model.desc = !this.model.desc
            } else {
                this.model.orderBy = data
                this.model.desc = false
            }
            this.load()
        },
        goPage(page) {
            this.model.pageNo = page
            this.load()
        },
        rowClick(item, evt) {
            if (this.select) {
                if (this.selection.includes(item) && this.multiple) {
                    this.selection.splice(this.selection.indexOf(item), 1)
                } else if(this.multiple) {
                    if (evt && evt.shiftKey && this.selection.length > 0) {
                        document.getSelection().removeAllRanges();
                        let i = this.results.data.indexOf(this.selection[this.selection.length - 1])
                        let j = this.results.data.indexOf(item)
                        this.selection = []
                        if (j < i) [j, i] = [i, j]
                        for (let k = i; k <= j; k++)
                            this.selection.push(this.results.data[k])
                    }
                    else {
                        this.selection.push(item)
                    }
                } else {
                    this.selection = [item]
                }
                this.$emit('selection', this.selection)
            } else {
                this.$emit('row-click', item)
            }
            
        },
        rowDblClick(item) {
            this.$emit('row-dblclick', item)
        },
        selectAll() {

            if (this.selection.length != this.results.data.length) {
                this.selection = [...this.results.data]
            } else {
                this.selection = []
            }
            this.$emit('selection', this.selection)
        }
    }
})
</script>
<style lang="scss">
@import '../assets/style/table.scss';
</style>
