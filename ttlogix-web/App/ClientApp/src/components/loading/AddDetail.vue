<template>
    <modal containerClass="modal-add-detail-loading">
        <template name="header" v-slot:header>
            {{$t('loading.addDetail.title')}}
        </template>
        <template name="body" v-slot:body>

            <!-- Loading Selection Header -->
            <div class="card" v-if="header != null">
                <div class="card-header">
                    <h5>{{$t('loading.addDetail.loadingSelectionHeader')}}</h5>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-2 col-lg-1 text-end pt-1 no-right-padding">{{$t('loading.addDetail.jobNo')}}</div>
                        <div class="col-md-4 col-lg-4">
                            <input type="text" class="form-control form-control-sm" :value="header.jobNo" disabled />
                            <form-error :errors="errors" field="JobNo" />
                        </div>
                        <div class="col-md-2 col-lg-1 text-end pt-1 no-right-padding">{{$t('loading.addDetail.customer')}}</div>
                        <div class="col-md-4 col-lg-4">
                            <input type="text" class="form-control form-control-sm" :value="header.customerName" disabled />
                            <form-error :errors="errors" field="CustomerName" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="card mt-3">
                <div class="card-header">
                    <h5>{{$t('loading.addDetail.availableOrderList')}}</h5>
                </div>
                <div class="card-body">
                    <div class="scrollable-45">
                        <div v-if="loading" class="mt-5 mb-5 text-center">
                            <div class="spinner-border text-primary" role="status">
                                <span class="visually-hidden">{{$t('generic.loading')}}</span>
                            </div>
                        </div>
                        <table class="table table-sm table-striped table-hover align-middle selectable" v-if="orders.length">
                            <thead>
                                <tr>
                                    <th>
                                        <input type="checkbox"
                                               @click.stop="selectedOrders.length == orders.length ? selectedOrders = [] : selectedOrders = orders.map(x => x.orderNo)"
                                               :checked="selectedOrders.length == orders.length" />
                                    </th>
                                    <th class="num">{{$t('loading.addDetail.no')}}</th>
                                    <th class="pointer" @click.prevent.stop="sortAdd(row => row.orderNo, 'orderNo')">
                                        <span class="d-inline text-nowrap">{{$t('loading.addDetail.orderNo')}}</span>
                                        <i v-if="sortedBy == 'orderNo'" class="d-inline" :class="sortIcon()"></i>
                                    </th>
                                    <th class="pointer" @click.prevent.stop="sortAdd(row => row.outboundJobNo, 'outboundJobNo')">
                                        <span class="d-inline text-nowrap">{{$t('loading.addDetail.outboundJobNo')}}</span>
                                        <i v-if="sortedBy == 'outboundJobNo'" class="d-inline" :class="sortIcon()"></i>
                                    </th>
                                    <th class="pointer" @click.prevent.stop="sortAdd(row => row.supplierID, 'supplierID')">
                                        <span class="d-inline text-nowrap">{{$t('loading.addDetail.supplierId')}}</span>
                                        <i v-if="sortedBy == 'supplierID'" class="d-inline" :class="sortIcon()"></i>
                                    </th>
                                    <th class="pointer" @click.prevent.stop="sortAdd(row => row.supplierName, 'supplierName')">
                                        <span class="d-inline text-nowrap">{{$t('loading.addDetail.supplierName')}}</span>
                                        <i v-if="sortedBy == 'supplierName'" class="d-inline" :class="sortIcon()"></i>
                                    </th>
                                    <th class="pointer" @click.prevent.stop="sortAdd(row => row.etd, 'etd')">
                                        <span class="d-inline text-nowrap">{{$t('loading.addDetail.releasedDate')}}</span>
                                        <i v-if="sortedBy == 'etd'" class="d-inline" :class="sortIcon()"></i>
                                    </th>
                                    <th class="pointer" @click.prevent.stop="sortAdd(row => row.outboundStatus, 'outboundStatus')">
                                        <span class="d-inline text-nowrap">{{$t('loading.addDetail.status')}}</span>
                                        <i v-if="sortedBy == 'outboundStatus'" class="d-inline" :class="sortIcon()"></i>
                                    </th>
                                    <th class="pointer" @click.prevent.stop="sortAdd(row => row.remark, 'remark')">
                                        <span class="d-inline text-nowrap">{{$t('loading.addDetail.remark')}}</span>
                                        <i v-if="sortedBy == 'remark'" class="d-inline" :class="sortIcon()"></i>
                                    </th>
                                    <th class="pointer" @click.prevent.stop="sortAdd(row => row.transportNo, 'transportNo')">
                                        <span class="d-inline text-nowrap">{{$t('outbound.header.transportNumber')}}</span>
                                        <i v-if="sortedBy == 'transportNo'" class="d-inline" :class="sortIcon()"></i>
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="(row, index) in orders" :key="index" :class="{selected: isSelected(row)}" @click.prevent.stop="onClick(row)">
                                    <td>
                                        <input type="checkbox" @click.stop v-model="selectedOrders" :value="row.orderNo" />
                                    </td>
                                    <td class="num">{{index + 1}}</td>
                                    <td>{{row.orderNo}}</td>
                                    <td>{{row.outboundJobNo}}</td>
                                    <td>{{row.supplierID}}</td>
                                    <td>{{row.supplierName}}</td>
                                    <td>
                                        <date-time :value="row.etd" nbsp />
                                    </td>
                                    <td>{{$t('outbound.status.' + row.outboundStatus)}}</td>
                                    <td>{{row.remark}}</td>
                                    <td>{{row.transportNo}}</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </template>
        <template name="footer" v-slot:footer>
            <button class="btn btn-sm btn-primary me-2" type="button" @click.stop="process()" :disabled="processing">
                <i class="las la-plus-circle"></i> {{$t('loading.operation.add')}}
            </button>
            <button class="btn btn-sm btn-secondary" type="button" @click.stop="$emit('close')" :disabled="processing">
                <i class="las la-times"></i> {{$t('loading.operation.close')}}
            </button>
        </template>
    </modal>
</template>
<script>
    import FormError from '@/widgets/FormError'
    import DateTime from '@/widgets/DateTime'
    //import SimpleTableScroll from '@/widgets/SimpleTableScroll'
import { defineComponent } from 'vue';
export default defineComponent({
    props: {
        title: {
            default: 'Add Loading Detail'
        },
        header: {},
    },
    created() {
        this.refresh()
    },
        components: { FormError, DateTime, /*SimpleTableScroll*/},
    data() {
        return {
            errors: {},
            processing: false,
            orders: [],
            selectedOrders: [],
            loading: true,
            sortedAsc: false,
            sortedBy: ""
        }
    },
    methods: {
        refresh() {
            this.$dataProvider.loadings.getLoadingEntryList(this.header.customerCode).then(resp => {
                this.orders = resp
                this.loading = false;
            })
            this.selectedOrders = []
        },
        isSelected(row) {
            return this.selectedOrders.includes(row.orderNo)
        },
        onClick(row) {
            var index = this.selectedOrders.indexOf(row.orderNo);
            if (index !== -1) {
                this.selectedOrders.splice(index, 1);
            }
            else {
                this.selectedOrders.push(row.orderNo);
            }
        },
        process() {
            this.$dataProvider.loadings.batchCreateLoadingDetails(this.header.jobNo, this.selectedOrders).then(() => {
                this.$notify({
                    title: this.$t('success.Done'),
                    text: this.$t('success.OrdersAdded'),
                    type: 'success',
                    group: 'temporary'
                })
                this.$emit('done')
            }).finally(() => {
                this.$emit('close')
            });
        },
        sortAdd(selector, by, mode) {
            if (mode) this.sortedAsc = mode == 'asc';
            else if (this.sortedBy != by) this.sortedAsc = true;
            else this.sortedAsc = !this.sortedAsc;
            this.sortedBy = by;
            this.orders = this.orders.sort((a, b) => {
                if (typeof (selector(a)) === 'string' || selector(a) instanceof String) {
                    return selector(a).localeCompare(selector(b)) * (this.sortedAsc ? 1 : -1);
                } else {
                    var x = selector(a); var y = selector(b); var d = this.sortedAsc ? -1 : 1;
                    return ((x < y) ? d : ((x > y) ? (-d) : 0));
                }
            });
        },
        sortIcon() {
            return this.sortedAsc ? 'las la-sort-amount-up-alt' : 'las la-sort-amount-down';
        }
    }
})
</script>
<style lang="scss">
    .modal-add-detail-loading {
        .modal-container {
            width: 95vw;
        }
    }
</style>