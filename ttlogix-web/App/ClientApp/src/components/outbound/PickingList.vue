<template>
    <modal containerClass="modal-picking-list-outbound">
        <template name="header" v-slot:header>
            {{$t('outbound.detail.outboundPickingList')}} &nbsp; &nbsp; &nbsp; {{$t('outbound.detail.grid_PartNo')}}: {{ productCode }}, &nbsp; {{$t('outbound.detail.grid_SupplierID')}}: {{ supplierId }}, &nbsp; {{$t('outbound.detail.grid_WHS')}}: {{ whsCode }}
        </template>
        <template name="body" v-slot:body>
            <div>
                <div class="float-start"><strong>{{$t('outbound.detail.availableList')}}</strong></div>
                <div class="float-end">loaded: <strong>{{ availableItems.length }}</strong>, total available: <strong>{{ allAvailable }}</strong></div>
                <div class="clearfix"></div>
            </div>
            <div class="card">
                <div class="card-body relative">
                    <div class="scrollable-25">

                        <!-- table header -->
                        <div class="div-table-header-wrapper">
                            <div class="div-table-header">
                                <div class="float-start picking-list-col0">&nbsp;</div>
                                <div class="float-start picking-list-col1" @click.prevent.stop="sortAvailable('pid')">
                                    {{$t('outbound.detail.grid_PID')}} <i v-if="availableFilter.sorting.by == 'pid'" :class="getSortClass('a')"></i>
                                </div>
                                <div class="float-start picking-list-col2" @click.prevent.stop="sortAvailable('qty')">
                                    {{$t('outbound.detail.grid_Qty')}} <i v-if="availableFilter.sorting.by == 'qty'" :class="getSortClass('a')"></i>
                                </div>
                                <div class="float-start picking-list-col3" @click.prevent.stop="sortAvailable('inboundDate')">
                                    {{$t('outbound.detail.grid_InDate')}} <i v-if="availableFilter.sorting.by == 'inboundDate'" :class="getSortClass('a')"></i>
                                </div>
                                <div class="float-start picking-list-col4" @click.prevent.stop="sortAvailable('ownership')">
                                    {{$t('outbound.detail.grid_Ownership')}} <i v-if="availableFilter.sorting.by == 'ownership'" :class="getSortClass('a')"></i>
                                </div>
                                <div class="float-start picking-list-col5" @click.prevent.stop="sortAvailable('locationCode')">
                                    {{$t('outbound.detail.grid_LocationID')}} <i v-if="availableFilter.sorting.by == 'locationCode'" :class="getSortClass('a')"></i>
                                </div>
                                <div class="float-start picking-list-col6" @click.prevent.stop="sortAvailable('groupID')">
                                    {{$t('outbound.detail.grid_GroupID')}} <i v-if="availableFilter.sorting.by == 'groupID'" :class="getSortClass('a')"></i>
                                </div>
                                <div class="clearfix"></div>
                            </div>
                            <div class="div-table-filters">
                                <div class="float-start picking-list-col0">&nbsp;</div>
                                <div class="float-start picking-list-col1">
                                    <input type="text" class="form-control" v-model="searchAV.pid" @keydown.enter="filterAvailableItems" placeholder="filter by PID" />
                                </div>
                                <div class="float-start picking-list-col2">&nbsp;</div>
                                <div class="float-start picking-list-col3">
                                    <date-picker :date="searchAV.inboundDate" @set="(date) => {searchAV.inboundDate = date;filterAvailableItems()}" :disabled="processing" />
                                </div>
                                <div class="float-start picking-list-col4">&nbsp;</div>
                                <div class="float-start picking-list-col5">
                                    <input type="text" class="form-control" v-model="searchAV.locationCode" @keydown.enter="filterAvailableItems" placeholder="filter by Location" />
                                </div>
                                <div class="float-start picking-list-col6">
                                    <input type="text" class="form-control" v-model="searchAV.groupID" @keydown.enter="filterAvailableItems" placeholder="filter by Group ID" />
                                </div>
                                <div class="clearfix"></div>
                            </div>
                        </div>
                        <!-- spinner -->
                        <div v-if="loadingAvailable" class="mt-4 mb-3 text-center">
                            <div class="spinner-border text-primary" role="status">
                                <span class="visually-hidden">{{$t('generic.loading')}}</span>
                            </div>
                        </div>
                        <!-- no items -->
                        <div v-if="availableItems.length == 0">
                            <div class="text-center my-4">
                                <strong>No items available</strong>
                            </div>
                        </div>
                        <!-- list -->
                        <div id="#infLoading" v-if="!loadingAvailable && availableItems.length > 0">
                            <div v-for="(row, index) in availableItems" :key="index" :class="['div-table', {selected: selectedAvailable.includes(row)}]" @click.prevent.stop="toggleAvailable(row)">
                                <div class="float-start picking-list-col0"><input @click.stop type="checkbox" v-model="selectedAvailable" :value="row" /></div>
                                <div class="float-start picking-list-col1">{{row.pid}}</div>
                                <div class="float-start picking-list-col2">{{row.qty}}</div>
                                <div class="float-start picking-list-col3"><date-time :value="row.inboundDate" nbsp /></div>
                                <div class="float-start picking-list-col4">{{row.ownershipString}}</div>
                                <div class="float-start picking-list-col5">{{row.locationCode}}</div>
                                <div class="float-start picking-list-col6" @click.stop.prevent="selectGroup(row.groupID)">{{row.groupID}}</div>
                                <div class="clearfix"></div>
                            </div>
                            <!-- infinite scroll component -->
                            <infinite-loading target="#infLoading" @firstload=false @infinite="handleScroll">
                                <!-- custom spinner -->
                                <template #spinner>
                                    <div class="mt-3 mb-2 text-center">
                                        <div class="spinner-border text-primary" role="status" v-if="availableItems.length < allAvailable">
                                            <span class="visually-hidden">{{$t('generic.loading')}}</span>
                                        </div>
                                    </div>
                                </template>
                            </infinite-loading>
                        </div>
                    </div>

                </div>
            </div>

            <div class="row mt-2">
                <div class="col-md-12">
                    <button class="btn btn-sm btn-primary me-2" @click="allocate()" :disabled="processing">
                        <i class="las la-arrow-down"></i> {{$t('outbound.detail.allocate')}}
                    </button>
                    <button class="btn btn-sm btn-primary me-2" @click="unAllocate()" :disabled="processing">
                        <i class="las la-arrow-up"></i> {{$t('outbound.detail.unAllocate')}}
                    </button>
                    <button class="btn btn-sm btn-primary" @click="autoAllocate()" :disabled="processing">
                        <i class="las la-project-diagram"></i> {{$t('outbound.detail.autoAllocate')}}
                    </button>
                    <label class="" style="margin-left: 50px;">To Pick: {{qtyToPick}}</label>
                    <label class="" style="margin-left: 50px;">Picked: {{qtyPicked}}</label>
                </div>
            </div>

            <div class="mt-2"><strong>{{$t('outbound.detail.pickingList')}}</strong></div>
            <div class="card">
                <div class="card-body">
                    <div class="scrollable-25">
                        <div v-if="loadingAllocated" class="mt-5 mb-5 text-center">
                            <div class="spinner-border text-primary" role="status">
                                <span class="visually-hidden">{{$t('generic.loading')}}</span>
                            </div>
                        </div>
                        <table class="table table-sm table-striped table-hover align-middle selectable" v-if="!loadingAllocated && allocatedItems.length">
                            <thead>
                                <tr>
                                    <th class="checkbox"></th>
                                    <th class="pointer" @click.prevent.stop="sortPicked('pid')">
                                        {{$t('outbound.detail.grid_PID')}}
                                        <i v-if="sortedByS == 'pid'" :class="getSortClass('s')"></i>
                                    </th>
                                    <th class="pointer" @click.prevent.stop="sortPicked('qty')">
                                        {{$t('outbound.detail.grid_Qty')}}
                                        <i v-if="sortedByS == 'qty'" :class="getSortClass('s')"></i>
                                    </th>
                                    <th class="pointer" @click.prevent.stop="sortPicked('inboundDate')">
                                        {{$t('outbound.detail.grid_InDate')}}
                                        <i v-if="sortedByS == 'inboundDate'" :class="getSortClass('s')"></i>
                                    </th>
                                    <th class="pointer" @click.prevent.stop="sortPicked('ownershipString')">
                                        {{$t('outbound.detail.grid_Ownership')}}
                                        <i v-if="sortedByS == 'ownershipString'" :class="getSortClass('s')"></i>
                                    </th>
                                    <th class="pointer" @click.prevent.stop="sortPicked('locationCode')">
                                        {{$t('outbound.detail.grid_LocationID')}}
                                        <i v-if="sortedByS == 'locationCode'" :class="getSortClass('s')"></i>
                                    </th>
                                    <th class="pointer" @click.prevent.stop="sortPicked('groupID')">
                                        {{$t('outbound.detail.grid_GroupID')}}
                                        <i v-if="sortedByS == 'groupID'" :class="getSortClass('s')"></i>
                                    </th>
                                </tr>
                                <tr class="search-row">
                                    <td></td>
                                    <td>
                                        <input type="text" class="form-control" v-model="searchPICK.pid" placeholder="filter by PID" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <input type="text" class="form-control" v-model="searchPICK.inboundDate" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <input type="text" class="form-control" v-model="searchPICK.locationCode" placeholder="filter by Location" />
                                    </td>
                                    <td>
                                        <input type="text" class="form-control" v-model="searchPICK.groupID" placeholder="filter by Group ID" />
                                    </td>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="(row, index) in filteredAllocatedItems" :key="index" :class="{selected: selectedAllocated.includes(row)}" @click.prevent.stop="toggleAllocated(row)">
                                    <td><input @click.stop type="checkbox" v-model="selectedAllocated" :value="row" /></td>
                                    <td>{{row.pid}}</td>
                                    <td>{{row.qty}}</td>
                                    <td><date-time :value="row.inboundDate" nbsp /></td>
                                    <td>{{row.ownershipString}}</td>
                                    <td>{{row.locationCode}}</td>
                                    <td @click.stop.prevent="selectGroupAllocated(row.groupID)">{{row.groupID}}</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>

        </template>
        <template name="footer" v-slot:footer>
            <button class="btn btn-sm btn-secondary" type="button" @click.stop="$emit('close')">
                <i class="las la-times"></i> {{$t('outbound.operation.close')}}
            </button>
        </template>
    </modal>
</template>
<script>
    import DateTime from '@/widgets/DateTime'
    import moment from 'moment'
    import { defineComponent } from 'vue';
    import { toServer, fromServer } from '@/service/date_converter.js'
    import DatePicker from '@/widgets/DatePicker';
    
    export default defineComponent({
        props: ['jobNo', 'lineItem', 'qtyToPick', 'supplierId', 'productCode', 'whsCode'],
        created() {
            this.refresh();
        },
        components: { DateTime, DatePicker },
        data() {
            return {
                loadingAvailable: true,
                loadingAvailablePart: false,
                allAvailable: 0,
                currentAvailablePage: 1,
                availableHasNextPage: false,
                loadingAllocated: true,
                availableItems: [],
                allocatedItems: [],
                selectedAllocated: [],
                selectedAvailable: [],
                qtyPicked: 0,
                processing: false,
                searchAV: {
                    pid: null,
                    groupID: null,
                    locationCode: null,
                    productCode: null,
                    supplierID: null,
                    inboundDate: null,
                    whsCode: null
                },
                searchPICK: {
                    pid: null,
                    groupID: null,
                    locationCode: null,
                    productCode: null,
                    supplierID: null,
                    inboundDate: null,
                    whsCode: null
                },
                sortedAscA: false,
                sortedAscS: false,
                sortedByA: "pid",
                sortedByS: "pid",
                availableFilter: {
                    outboundJobNo: this.jobNo,
                    lineItem: this.lineItem,
                    PID: null,
                    inboundDate: null,
                    location: null,
                    groupID: null,
                    pagination: {
                        pageNumber: 1,
                        itemsPerPage: 100
                    },
                    sorting: {
                        by: 'pid',
                        descending: true
                    }
                }
            }
        },
        computed: {
            filteredAllocatedItems() {
                return this.allocatedItems.filter(row => {
                    this.allocatedItems.forEach(function (object) {
                        for (var key in object) {
                            if (object[key] == null)
                                object[key] = '';
                        }
                    });
                    let ok = true
                    Object.keys(this.searchPICK).forEach(key => {
                        let dt = this.searchPICK[key] || ''
                        let val = row[key] || ''

                        if (key == 'inboundDate') {
                            val = moment(val).format('DD/MM/YYYY HH:mm')
                        }
                        dt = (dt + '').trim()
                        val = (val + '').trim()
                        if (dt.length > 0) {
                            if (val.toLowerCase().indexOf(dt.toLowerCase()) === -1) {
                                ok = false
                            }
                        }
                    })

                    return ok
                })
            }
        },
        methods: {
            toServer,
            fromServer,
            filterAvailableItems() {
                this.availableFilter.PID = this.searchAV.pid;
                this.availableFilter.inboundDate = this.searchAV.inboundDate;
                this.availableFilter.location = this.searchAV.locationCode;
                this.availableFilter.groupID = this.searchAV.groupID;
                this.availableFilter.pagination.pageNumber = 1;
                this.availableFilter.pagination.itemsPerPage = 100;

                this.handleScroll(true, true);
            },
            selectGroup(groupID) {
                if (groupID == null) {
                    return
                }
                let pids = this.availableItems.filter(x => x.groupID == groupID)

                let all = true
                pids.forEach(p => {
                    if (!this.selectedAvailable.includes(p)) {
                        all = false
                    }
                })
                this.selectedAvailable = this.selectedAvailable.filter(x => pids.includes(x))
                if (!all) {
                    pids.forEach(p => this.selectedAvailable.push(p))
                }
            },
            selectGroupAllocated(groupID) {
                if (groupID == null) {
                    return
                }
                let pids = this.allocatedItems.filter(x => x.groupID == groupID)
                let all = true
                pids.forEach(p => {
                    if (!this.selectedAllocated.includes(p)) {
                        all = false
                    }
                })
                this.selectedAllocated = this.selectedAllocated.filter(x => pids.includes(x))
                if (!all) {
                    pids.forEach(p => this.selectedAllocated.push(p))
                }
            },
            toggleAvailable(row) {
                if (this.selectedAvailable.includes(row)) {
                    this.selectedAvailable = this.selectedAvailable.filter(x => x != row)
                } else {
                    this.selectedAvailable.push(row)
                }
            },
            toggleAllocated(row) {
                if (this.selectedAllocated.includes(row)) {
                    this.selectedAllocated = this.selectedAllocated.filter(x => x != row)
                } else {
                    this.selectedAllocated.push(row)
                }
            },
            refresh() {
                this.qtyPicked = 0
                this.loadingAvailable = true
                this.loadingAllocated = true
                this.$dataProvider.pickingLists.getPickingListWithUOM(this.jobNo, this.lineItem).then(resp => {
                    this.allocatedItems = resp;
                    this.loadingAllocated = false
                    if (this.allocatedItems) {
                        this.qtyPicked = this.allocatedItems.map(a => a.qty).reduce((a, b) => a + b, 0)
                    }
                    this.sortedAscS = !this.sortedAscS;
                    this.sortPicked(this.sortedByA);
                });
                
                this.handleScroll(true);
                this.selectedAllocated = [];
                this.selectedAvailable = [];
            },
            handleScroll(resetPages = false, filtering = false) {
                if ((typeof(resetPages) === 'boolean' && resetPages) || !this.loadingAvailablePart && this.availableHasNextPage) {
                    if ((typeof (resetPages) === 'boolean' && resetPages)) {
                        this.selectedAvailable = filtering ? this.selectedAvailable : [];
                        this.loadingAvailable = true;
                    } else {
                        this.loadingAvailablePart = true;
                    }
                    
                    this.availableFilter.pagination.pageNumber = (typeof(resetPages) != 'boolean') ? this.availableFilter.pagination.pageNumber + 1 : 1;
                    this.$dataProvider.storage.getStorageDetailWithPartsInfoList(this.availableFilter).then(resp => {
                        this.availableItems = (typeof (resetPages) === 'boolean' && resetPages) ? resp.items : this.availableItems.concat(resp.items);
                        this.loadingAvailable = false;
                        this.loadingAvailablePart = false;
                        this.availableHasNextPage = resp.pagination.hasNextPage;
                        this.allAvailable = resp.pagination.totalCount;
                        //if ((typeof (resetPages) === 'boolean' && resetPages)) this.selectedAvailable = [];
                    })
                }
            },
            allocate() {
                if (this.processing) {
                    return
                }

                if (this.selectedAvailable.length > 0) {
                    this.processing = true
                    this.loadingAvailable = true
                    this.loadingAllocated = true
                    let allocate = [...this.selectedAvailable].map(item => ({
                        jobNo: this.jobNo,
                        lineItem: this.lineItem,
                        pid: item.pid,
                    }))
                    this.$dataProvider.pickingLists.allocate(allocate).finally(() => {
                        this.refresh();
                        this.processing = false;
                    })
                }
            },
            unAllocate() {
                //TODO: refactor this method
                if (this.processing) {
                    return
                }
                if (this.selectedAllocated.length > 0) {
                    this.processing = true
                    this.loadingAvailable = true
                    this.loadingAllocated = true
                    let allocate = [...this.selectedAllocated].map(item => ({
                            jobNo: this.jobNo,
                            lineItem: this.lineItem,
                            seqNo: item.seqNo,
                            pid: item.pid,
                        }))
                    this.$dataProvider.pickingLists.unAllocate(allocate).finally(() => {
                            this.refresh();
                            this.processing = false;
                       })
                }                
            },
            autoAllocate() {
                if (this.processing) {
                    return
                }
                this.processing = true
                this.loadingAvailable = true
                this.inProgress = true
                this.loadingAllocated = true
                this.$dataProvider.pickingLists.autoAllocate(this.jobNo, this.lineItem).then(() => {
                    this.refresh();
                }).finally(() => this.processing = false)
            },
            sortAvailable(o) {
                this.availableFilter.sorting.descending = !this.availableFilter.sorting.descending;
                this.availableFilter.sorting.by = o;

                this.handleScroll(true,true);
            },
            sortPicked(o) {
                this.sortedAscS = !this.sortedAscS;
                this.sortedByS = o;
                this.allocatedItems = this.allocatedItems.sort((a, b) => a[o].toString().localeCompare(b[o]) * (this.sortedAscS ? 1 : -1));

            },
            getSortClass(list) {
                var up = "las la-sort-amount-up-alt";
                var down = "las la-sort-amount-down";
                return list == "a" ? this.availableFilter.sorting.descending ? up : down : this.sortedAscS ? up : down;
            }
        }
    })
</script>
<style lang="scss">
    .modal-picking-list-outbound {
        .modal-container {
            width: 95vw;
        }
        .search-row {
            input {
                width: 100%;
            }
        }
    }
    
</style>