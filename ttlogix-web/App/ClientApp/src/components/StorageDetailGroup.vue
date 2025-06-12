<template>
    <div class="storage-detail-group">

        <div class="toolbar-new">
            <div class="row pt-2 pb-2">
                <div class="col-md-6 ps-4 main-action-icons">
                    
                    <div class="d-inline-block me-5 link-box">
                        <a href="#" @click.stop.prevent="createModal = true" class="text-center">
                            <div><i class="las la-file-medical"></i></div>
                            <div>Create</div>
                        </a>
                    </div>
                    <div class="d-inline-block me-5 link-box">
                        <a href="#" @click.stop.prevent="printSelected()" class="text-center">
                            <div><i class="las la-print"></i></div>
                            <div>Print selected</div>
                        </a>
                    </div>
                    <div class="d-inline-block me-5 link-box">
                        <a href="#" @click.stop.prevent="detailsSelected()" class="text-center">
                            <div><i class="las la-tasks"></i></div>
                            <div>Print report</div>
                        </a>
                    </div>
                    
                </div>
                <div class="col-md-6 text-end pe-5 main-action-icons">
                    <div class="d-inline-block link-box">
                        <a href="#" @click.stop.prevent="refresh()" class="text-center">
                            <div><i class="las la-redo-alt"></i></div>
                            <div>Refresh</div>
                        </a>
                    </div>    
                </div>
            </div>
        </div>
        <div class="wrap">
            <dynamic-table :func="(params) => $dataProvider.storageGroup.getGroups(params)" ref="table" :columns="cols" :filters="defaultFilter" :multiple="true" :search="true" @row-dblclick="showDetails([$event.groupID])" :hoverActions="true" :actions="false">
                <template v-slot:createdDate="{value}">
                    <date :value="value" />
                </template>
                <template v-slot:closedDate="{value}">
                    <date :value="value" />
                </template>
                <template v-slot:repackedDate="{value}">
                    <date :value="value" />
                </template>
                <template v-slot:filter-createdDate="{filter}">
                    <date-picker :date="createdDate" @set="(date) => {filter({createdDate: date})}" />
                </template>
                <template v-slot:filter-closedDate="{filter}">
                    <date-picker :date="closedDate" @set="(date) => {filter({closedDate: date})}" />
                </template>
                <template v-slot:filter-repackedDate="{filter}">
                    <date-picker :date="repackedDate" @set="(date) => {filter({repackedDate: date})}" />
                </template>
                <template v-slot:inJobNo="{value}">
                    <span v-for="v in uniqueArray(value)" :key="v">{{ v }} </span>
                </template>
                
                <template v-slot:hoverActions="{row}">
                    <ul>
                        <li v-if="row.status == 0"><button class="btn btn-sm btn-danger" @click="deletedGroup = row"><i class="las la-trash-alt"></i> Remove</button></li>
                        <li><button class="btn btn-sm btn-primary" @click="showDetails([row.groupID])"><i class="la la-search"></i> Details</button></li>
                        <li><button class="btn btn-sm btn-primary" @click="printLabel = [row.groupID]"><i class="las la-print"></i> Print Label</button></li>
                        <li><button class="btn btn-sm btn-primary" @click="transform = row"><i class="las la-random"></i> Transform</button></li>
                        <li><button class="btn btn-sm btn-primary" @click="printPIDLabel = [row.groupID]"><i class="las la-print"></i> Print PID Label</button></li>
                    </ul>
                </template>
            </dynamic-table>
        </div>
        <print-pid v-if="printPIDLabel" @close="printPIDLabel = null" :selected="printPIDLabel" :all="printPIDLabel"></print-pid>
        <print-gid v-if="printLabel" @close="printLabel = null" :selected="printLabel" :all="printLabel"></print-gid>

        <confirm v-if="transform" @close="transform = null" @ok="transformGroup">
            <template>
                Group {{transform.groupID}} will be transformed.
            </template>
        </confirm>
        <confirm v-if="deletedGroup" @close="deletedGroup = null" @ok="deleteGroup()">

            <template>
                Group {{deletedGroup.groupID}} will be removed.
            </template>
        </confirm>
        <modal v-if="details" @close="details = null" container-class="storage-group-modal">
            <template name="body" v-slot:body>
                <div class="page" v-for="detail in details" :key="'det' + detail.gid">
                    <div class="head-r">
                        <div class="head">
                            
                        {{ detail.gid }}<br />
                
                        <small>QTY: {{ detail.data.length }}</small>
                        </div>
                        <canvas :ref="'qr' + detail.gid" class="qr"></canvas>
                    </div>
                
                    <table class="table">
                        <thead>
                            <tr>
                                <th>Product Code</th>
                                <th>Customer</th>
                                <th>Supplier</th>
                                <th>Qty</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="row in detail.data" :key="row.pid">
                                <td>{{row.productCode }}</td>
                                <td>{{row.customerCode }}</td>
                                <td>{{row.supplierID }}</td>
                                <td>{{row.qty }}</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </template>
            <template name="footer" v-slot:footer>
                <button class="button small" type="button" @click.stop="print()">
                    {{$t('inbound.operation.print')}}
                </button>
                <button class="button small" type="button" @click.stop="details = null">
                    {{$t('inbound.operation.close')}}
                </button>
            </template>
        </modal>
        <modal v-if="createModal" container-class="qty-modal">
            <template v-slot:header>
                Create
            </template>
            <template v-slot:body>
                <div class="form-row">
                    <label class="form-label">Quantity</label>
                    <div class="form-input full multiselect-as-select">
                        <input type="number" v-model="createQty" ref="qty" @click="$refs.qty.select()" />
                    </div>
                </div>
                <div class="form-row">
                    <label class="form-label">Prefix</label>
                    <div class="form-input full multiselect-as-select">
                        <input type="text" v-model="createName" ref="name" @click="$refs.name.select()" />
                    </div>
                </div>
            </template>
            <template name="footer" v-slot:footer>
                <button class="button small" type="button" @click.stop="create()">
                    Create
                </button>
                <button class="button small" type="button" @click.stop="createModal = false">
                    {{$t('inbound.operation.close')}}
                </button>
            </template>
        </modal>
    </div>
</template>

<script>
    import qrcode from 'qrcode';
    import DynamicTable from '@/widgets/Table.vue'
    import Date from '@/widgets/Date'
    import Modal from '@/widgets/Modal'
    import Confirm from '@/widgets/Confirm'
    import { toServer } from '@/service/date_converter.js'
    import RouteRefreshMixin from '@/mixins/routeRefreshMixin.js'
    import PrintLabelPIDForGID from '@/components/print/PrintLabelPIDForGID'
    import PrintLabelGID from '@/components/print/PrintLabelGID'
    import DatePicker from '@/widgets/DatePicker';

    import { defineComponent } from 'vue';
export default defineComponent({
        name: 'StorageGroup',
        mixins: [RouteRefreshMixin],
        components: { DynamicTable, Date, "print-pid": PrintLabelPIDForGID, "print-gid": PrintLabelGID, Modal, Confirm, DatePicker},
        data() {
            return {
                receivedFilter: null,
                modal: null,
                transform: null,
                createdDate: null,
                closedDate: null,
                repackedDate: null,
                printLabel : null,
                printPIDLabel: null,
                createModal : false,
                createQty : 1,
                createName : 'Group',
                deletedGroup:null,
                status: {
                    label: this.$t('inbound.status.Outstanding'),
                    value: ["New", "InUse", "Sent"]
                },
                details: null,
                defaultFilter: {
                    orderBy: 'groupID',
                    desc: true,
                    pageSize: 50,
                    pageNo: 1,
                    status: null
                },
                cols: [
                    {
                        data: 'groupID',
                        title: "Group ID" ,
                        sortable: true,
                        filter: true,
                        width: 200
                    },
                   {
                       data : 'createdDate',
                       title : 'Created Date',
                       sortable: true,
                       filter: true,
                       width : 130
                   },
                   
                   {
                        data: 'status',
                        title: "Status",
                        sortable: true,
                        filter: true,
                        width: 145
                   },
                   {
                        data: 'quantity',
                        title: "Quantity",
                        sortable: false,
                        filter: false,
                        width: 80
                   },
                   {
                        data: 'inJobNo',
                        title: "Job No",
                        sortable: false,
                        filter: true,
                        width: 145
                   },
                   {
                        data: 'name',
                        title: "Name",
                        sortable: false,
                        filter: true,
                        width: 145
                   },
                   {
                       data : 'closedDate',
                       title : 'Closed Date',
                       sortable: true,
                       filter: true,
                       width : 130
                   },
                   {
                       data : 'repackedDate',
                       title : 'Repacked Date',
                       sortable: true,
                       filter: true,
                       width : 130
                   }
               ]
            }
        },
        methods: {
            toServer,
            refresh() {
                return this.$refs.table.refresh()
            },
            deleteGroup() {
                this.$dataProvider.storageGroup.deleteGroup(this.deletedGroup.groupID).then(() => {
                    this.$notify({text : `Group ${this.deletedGroup.groupID}  has been deleted`, type : 'success',group: 'temporary'})
                    this.refresh()
                    this.deletedGroup = null
                })
            },
            transformGroup() {
                var group = this.transform
                this.$notify({text : `Group ${group.groupID}  in progress`, type : 'warning',group: 'temporary'})
                this.$dataProvider.storageGroup.transformGroup(group.groupID).then(() => {
                    this.$notify({text : `Group ${group.groupID}  has been transformed`, type : 'success',group: 'temporary'})
                    this.refresh()
                   
                    
                })
                this.transform = null
            },
            create() {
                this.$dataProvider.storageGroup.createGroup(this.createQty, this.createName).then(data => {
                    this.$notify({text : 'New group has been prepared', type: 'success',group: 'temporary'})
                    this.createModal = false;
                    this.refresh().then(() => {
                        this.$refs.table.selection = this.$refs.table.results.data.filter(x => data.data.includes(x.groupID))
                    })
                })
            },
            uniqueArray( ar ) {
                if (!ar || !Array.isArray(ar)) {
                    return []
                }
            var j = {};

            ar.forEach( function(v) {
                j[v+ '::' + typeof v] = v;
            });

            return Object.keys(j).map(function(v){
                return j[v];
            });
            },
            showDetails(gids) {
                if (gids.length == 0) {
                    return
                }
                var details = {}
                gids.forEach(gid => {
                    details[gid] = {
                        gid : gid,
                        data: [],
                        qty : 0
                    };
                    
                    
                    this.$dataProvider.storageGroup.details(gid).then(resp => {
                        details[gid].qty = resp.length
                        details[gid].data = resp
                    })
                })
                this.details = details
                this.$nextTick(() => {
                    let delimiter = '\u0005'
                    for (var gid in this.details) {
                        var qr = '<SG>' + gid + delimiter + 'Group' + delimiter + '' + delimiter + '' + delimiter
                        qrcode.toCanvas(this.$refs['qr' + gid][0], qr)

                    }
                })
                
            },
            print() {
                window.print()
            },
            printSelected() {
                this.printLabel = [];
                this.$refs.table.selection.forEach(g => this.printLabel.push(g.groupID))
                if (this.printLabel.length == 0) {
                    this.printLabel = null
                }
            },
            detailsSelected() {
                let tmp = []
                this.$refs.table.selection.forEach(g => tmp.push(g.groupID))
                this.showDetails(tmp)
            }
          
        }

    })
</script>

<style lang="scss">
.storage-group-modal {
    .modal-body {
        max-height: 80vh; 
        overflow: auto;
    }
    .qr {
        width: 120px; 
        height: 120px; 
    }
}
.qty-modal {
    .modal-container {
        max-width: 400px;
    }
    
}
.storage-group-modal {
    .page {
        
        page-break-after: always;
        page-break-inside: avoid;
        .head-r {
            display: flex;
            .head {
                flex: 1;
            }
        }
    }
}
@media print {

  .storage-group-modal {
    .page {
        height: 90vh;
    }
  }
  .modal-body, .router-tab__container {
      overflow: visible;
      height: auto;
  }
  .router-tab {
      display: block;
  }
  .router-tab__header, .storage-detail-group > .wrap {
      padding: 0px;
  }
  .router-tab__header, .storage-detail-group > .wrap > .button {
      display: none;
  }
  .modal-mask {
      position: static !important;

  }
  .vue-notification-group,.container-fluid {display: none;}
  .toolbar-new, header, footer, .storage-detail-group .v-table, .modal-footer {display: none;}
  
  .storage-group-modal {
      .modal-header {
          display: none;
      }
      .modal-wrapper, .storage-group-modal, .modal-container {
          display: block;
          max-width: none;
          box-shadow: none;
      }
      .modal-header, .modal-body {
          padding-left: 0px;
          padding-right: 0px;
          max-height: none;
      }
  }
}
</style>