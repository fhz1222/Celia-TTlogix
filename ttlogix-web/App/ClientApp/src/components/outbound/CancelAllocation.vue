<template>
    <modal containerClass="modal-cancel-allocation">
        <template name="header" v-slot:header>
            {{$t('outbound.cancelAllocation.title')}}
        </template>

        <template name="body" v-slot:body class="small-font">
            <div class="row">
                <div class="col-md-2 text-end pt-1 no-right-padding small-font">
                    {{$t('outbound.cancelAllocation.partNo')}}
                </div>
                <div class="col-md-3 no-right-padding">
                    <input class="form-control form-control-sm" :value="lineItem.productCode" disabled />
                </div>
                <div class="col-md-1 text-end pt-1">#</div>
                <div class="col-md-1 no-left-padding no-right-padding">
                    <input class="form-control form-control-sm" :value="lineItem.lineItem" disabled />
                </div>
                <div class="col-md-2 text-end pt-1 no-right-padding small-font">
                    {{$t('outbound.cancelAllocation.supplierID')}}
                </div>
                <div class="col-md-2 no-right-padding">
                    <input class="form-control form-control-sm" :value="lineItem.supplierID" disabled />
                </div>
            </div>
            <div class="row mt-3">
                
            </div>

            <div class="row mt-3">
                <div class="col-md-5">
                    <div><strong>{{$t('outbound.cancelAllocation.outstandingList')}}</strong></div>
                    <div>{{$t('outbound.cancelAllocation.outstandingListInfo')}}</div>
                </div>
                <div class="col-md-2">&nbsp;</div>
                <div class="col-md-5">
                    <div><strong>{{$t('outbound.cancelAllocation.removeList')}}</strong></div>
                    <div class="text-danger">{{$t('outbound.cancelAllocation.removeListInfo')}}</div>
                </div>
            </div>
            <div class="row">
                <!-- PICKING LIST -->
                <div class="col-md-5">
                    <div class="card">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="scrollable-45">

                                        <table class="table table-sm table-striped table-hover align-middle selectable" v-if="availableItems.length">
                                            <thead class="sticky-header">
                                                <tr>
                                                    <th>{{$t('outbound.cancelAllocation.grid_No')}}</th>
                                                    <th>{{$t('outbound.cancelAllocation.grid_Qty')}}</th>
                                                    <th>{{$t('outbound.cancelAllocation.grid_LocationCode')}}</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr v-for="(row, index) in availableItems" 
                                                    :key="index" 
                                                    :class="getSelectedClass(row,selectedAvailableItems)" 
                                                    @click.prevent.stop="selectAvailable(row, index)">
                                                    <td class="unselectable">{{row.seqNo}}</td>
                                                    <td class="unselectable">{{row.qty}}</td>
                                                    <td class="unselectable">{{row.locationCode}}</td>
                                                </tr>
                                            </tbody>
                                        </table>

                                    </div>
                                </div>
                            </div>
                            <div class="row mt-3">
                                <div class="col-md-8 text-end pt-1 no-right-padding small-font">
                                    {{$t('outbound.cancelAllocation.total')}}
                                </div>
                                <div class="col-md-4">
                                    <input class="form-control form-control-sm" :value="totalAvailable" disabled />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-md-2 text-center">
                    <div class="mt-5">
                        <div>
                            <button class="btn btn-danger" @click.stop="moveToRemoved()" :disabled="selectedAvailableItems.length == 0">
                                <i class="las la-arrow-right"></i>
                            </button>
                        </div>
                        <div class="mt-3">
                            <button class="btn btn-primary" @click.stop="moveToAvailable()" :disabled="selectedRemovedItems.length == 0">
                                <i class="las la-arrow-left"></i>
                            </button>
                        </div>
                    </div>
                </div>
                <!-- REMOVE LIST -->
                <div class="col-md-5">
                    <div class="card">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="scrollable-45">

                                        <table class="table table-sm table-striped table-hover align-middle selectable" v-if="removedItems.length">
                                            <thead class="sticky-header">
                                                <tr>
                                                    <th>{{$t('outbound.cancelAllocation.grid_No')}}</th>
                                                    <th>{{$t('outbound.cancelAllocation.grid_Qty')}}</th>
                                                    <th>{{$t('outbound.cancelAllocation.grid_LocationCode')}}</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr v-for="(row, index) in removedItems" :key="index" 
                                                    :class="getSelectedClass(row,selectedRemovedItems)" 
                                                    @click.prevent.stop="selectRemoved(row, index)">
                                                    <td class="unselectable">{{row.seqNo}}</td>
                                                    <td class="unselectable">{{row.qty}}</td>
                                                    <td class="unselectable">{{row.locationCode}}</td>
                                                </tr>
                                            </tbody>
                                        </table>

                                    </div>
                                </div>
                            </div>
                            <div class="row mt-3">
                                <div class="col-md-8 text-end pt-1 no-right-padding small-font">
                                    {{$t('outbound.cancelAllocation.total')}}
                                </div>
                                <div class="col-md-4">
                                    <input class="form-control form-control-sm" :value="totalRemoved" disabled />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </template>

        <template name="footer" v-slot:footer>
            <div class="row">
                <div class="col-md-12 text-end no-right-padding">
                    <button class="btn btn-sm btn-primary me-2" disabled v-if="!canConfirm()">
                        <i class="las la-check-circle"></i> {{$t('outbound.operation.confirm')}}
                    </button>
                    <awaiting-button v-if="canConfirm()"
                                     @proceed="(callback) => { disableClose(); process().finally(callback) }"
                                     :btnText="'outbound.operation.confirm'"
                                     :btnIcon="'las la-check-circle'"
                                     :btnClass="'btn-primary'">
                    </awaiting-button>
                    <button class="btn btn-sm btn-secondary" type="button" @click.stop="$emit('close')" :disabled="closeDisabled">
                        <i class="las la-times"></i> {{$t('outbound.operation.close')}}
                    </button>
                </div>
            </div>
        </template>

    </modal>
</template>
<script>
//import { lastIndexOf } from "core-js/core/array";

//import FormError from '@/widgets/FormError'
import { defineComponent } from 'vue';
import AwaitingButton from '@/widgets/AwaitingButton'
export default defineComponent({
    props: ['jobNo', 'lineItem'],
    created() {
        this.refresh()
    },
    components: { AwaitingButton },
    data() {
        return {
            availableItems: [],
            selectedAvailableItems: [],
            removedItems: [],
            selectedRemovedItems: [],
            totalAvailable: 0,
            totalRemoved: 0,
            firstIndex: -1,
            lastIndex: -1,
            goingUp: false,
            range: -1,
            startIndex: -1,
            firstIndexWasInSelected: false,
            closeDisabled: false
        }
    },
    methods: {
        refresh() {
            this.$dataProvider.pickingLists.getPickingListWithUOM(this.jobNo, this.lineItem.lineItem).then(resp => {
                this.availableItems = resp;
                this.recalculateTotals();
            });
            this.selectedAvailableItems = [];
        },
        disableClose() {
            this.closeDisabled = true;
        },
        selectAvailable(row, index) {
            this.setUpShiftSelect(index, event.shiftKey, false);
            var doubles = [];

            if (this.lastIndex != -1) {
                for (var ind = this.startIndex; ind < ((this.range + this.startIndex) - (this.goingUp ? 1 : 0)); ind++) {
                    if (!this.objectInArray(this.availableItems[ind], this.selectedAvailableItems)) {
                        this.selectedAvailableItems.push(this.availableItems[ind]);
                    } else {
                        doubles.push(this.availableItems[ind].seqNo);
                    }
                }
                
                this.removeFromList(this.availableItems[this.firstIndex].seqNo, doubles);
                if (this.firstIndexWasInSelected && !this.goingUp) {
                    this.removeFromList(this.availableItems[this.firstIndex], this.selectedAvailableItems);
                    this.firstIndexWasInSelected = false;
                }
                
                if (doubles.length > 0) {
                    this.removeFromListByKeyName(this.selectedAvailableItems, doubles, 'seqNo');
                }
            } else {
                if (this.objectInArray(row, this.selectedAvailableItems)) {
                    this.firstIndexWasInSelected = true;
                    this.removeFromList(row, this.selectedAvailableItems);
                } else {
                    this.selectedAvailableItems.push(row);
                }
            }

            this.setUpShiftSelect(index, event.shiftKey, event.shiftKey ? true : false);
        },
        selectRemoved(row, index) {
            this.setUpShiftSelect(index, event.shiftKey, false);
            var doubles = [];

            if (this.lastIndex != -1) {
                for (var ind = this.startIndex; ind < ((this.range + this.startIndex) - (this.goingUp ? 1 : 0)); ind++) {
                    if (!this.objectInArray(this.removedItems[ind], this.selectedRemovedItems)) {
                        this.selectedRemovedItems.push(this.removedItems[ind]);
                    } else {
                        doubles.push(this.removedItems[ind].seqNo);
                    }
                }

                this.removeFromList(this.removedItems[this.firstIndex].seqNo, doubles);
                if (this.firstIndexWasInSelected && !this.goingUp) {
                    this.removeFromList(this.removedItems[this.firstIndex], this.selectedRemovedItems);
                    this.firstIndexWasInSelected = false;
                }

                if (doubles.length > 0) {
                    this.removeFromListByKeyName(this.selectedRemovedItems, doubles, 'seqNo');
                }
            } else {
                if (this.objectInArray(row, this.selectedRemovedItems)) {
                    this.firstIndexWasInSelected = true;
                    this.removeFromList(row, this.selectedRemovedItems);
                } else {
                    this.selectedRemovedItems.push(row);
                }
            }

            this.setUpShiftSelect(index, event.shiftKey, event.shiftKey ? true : false);
        },
        setUpShiftSelect(index, shift, reset) {
            this.firstIndex = reset ? -1 : shift ? this.firstIndex : index;
            this.lastIndex = reset ? -1 : shift ? index : -1;
            this.goingUp = reset ? false : (this.lastIndex - this.firstIndex) > 0 ? true : false;
            this.range = reset ? -1 : Math.abs(this.lastIndex - this.firstIndex) + 1;
            this.startIndex = reset ? -1 : this.goingUp ? this.firstIndex + 1 : this.lastIndex;
        },
        removeFromList(item, list) {
            var l = list.length;
            while (l--) {
                if (list[l] === item) {
                    list.splice(l, 1);
                }
            }
        },
        removeFromListByKeyName(list, listOfValues, keyName) {
            var l = list.length;
            while (l--) {
                if (listOfValues.includes(list[l][keyName])) {
                    list.splice(l, 1);
                }
            }
        },
        getSelectedClass(row,arr) {
            return this.objectInArray(row, arr) == true ? "selected" : "";
        },
        objectInArray(obj, arr) {
            for (var i = 0; i < arr.length; i++) {
                if (arr[i] === obj) {
                    return true;
                }
            }
            return false;
        },
        moveToRemoved() {
            var toRemove = this.selectedAvailableItems;
            this.availableItems = this.availableItems.filter(((el) => !toRemove.includes(el)));
            this.removedItems = this.removedItems.concat(toRemove);
            this.selectedAvailableItems = [];
            this.recalculateTotals();
        },
        moveToAvailable() {
            var toRestore = this.selectedRemovedItems;
            this.removedItems = this.removedItems.filter(((el) => !toRestore.includes(el)));
            this.availableItems = this.availableItems.concat(toRestore);
            this.selectedRemovedItems = [];
            this.recalculateTotals();
        },
        recalculateTotals() {
            this.totalAvailable = 0;
            this.totalRemoved = 0;
            for (var i = 0; i < this.availableItems.length; i++) {
                this.totalAvailable += this.availableItems[i].qty;
            }
            for (var j = 0; j < this.removedItems.length; j++) {
                this.totalRemoved += this.removedItems[j].qty;
            }
        },
        process() {
            var _cancelAllocationDto = [];
            for (var r = 0; r < this.removedItems.length; r++) {
                _cancelAllocationDto.push({ "jobNo": this.jobNo, "lineItem": this.lineItem.lineItem, "seqNo": this.removedItems[r].seqNo });
            }
            var cancelAllocationDto = { "jobNo": this.jobNo, "lineItem": this.lineItem.lineItem, "itemsToCancel": _cancelAllocationDto };

            return this.$dataProvider.outbounds.cancelAllocation(cancelAllocationDto).then(resp => {
                this.$emit('done', resp);
            }).catch(e => {
                if (e.response && e.response.data.errors) {
                    this.errors = e.response.data.errors
                } else if (e.response && e.response.data.length) {
                    this.errors = { ProductCode: e.response.data }
                }
            }).finally(() => { this.$emit('done'); })
        },
        canConfirm() {
            return this.totalRemoved > 0;
        }
    },
    watch: {
        
    }
})
</script>
<style lang="scss">
    .modal-cancel-allocation {
        .modal-container {
            width: 800px;
        }
    }
</style>