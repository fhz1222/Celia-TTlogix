<template>
    <div class="container-fluid inbound part-master">
        <div class="row mt-3 mb-5">
            <div class="col-md-7">
                <div class="card">
                    <div class="card-header">
                        <h3>{{ $t('users.main.title') }}</h3>
                    </div>
                    <div class="card-body">

                        <!-- toolbar -->
                        <div class="toolbar-new">
                            <div class="row pb-2">
                                <div class="col-md-6 ps-4 main-action-icons">
                                    <div class="d-inline-block link-box">
                                        <a href="#" @click.stop.prevent="addUser(null, true)" class="text-center">
                                            <div><i class="las la-user-plus"></i></div>
                                            <div>{{ $t('users.operation.addUser') }}</div>
                                        </a>
                                    </div>
                                </div>
                                <div class="col-md-6 text-end pe-5 main-action-icons">
                                    <div class="d-inline-block link-box">
                                        <a href="#" @click.stop.prevent="refresh()" class="text-center">
                                            <div><i class="las la-redo-alt"></i></div>
                                            <div>{{ $t('operation.general.refresh') }}</div>
                                        </a>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- users' table -->
                        <dynamic-table :func="(params) => $dataProvider.users.getUsers(params)" ref="table"
                            :columns="cols" :filters="defaultFilter" :multiple="false" :search="true"
                            @row-dblclick="showDetails($event)" :hoverActions="true" :actions="false">
                            <template v-slot:filter-statusString="{ filter }">
                                <multiselect :options="statuses" label="label" v-model="status"
                                    @update:modelValue="filter({ status: $event})" :can-clear="false"
                                    :showLabels="false"></multiselect>
                            </template>
                            <template v-slot:hoverActions="{ row }">
                                <ul>
                                    <li v-tooltip="'Modify'">
                                        <button class="btn btn-sm btn-primary" @click="addUser(row.code, false)">
                                            <i class="las la-pen"></i>
                                        </button>
                                    </li>
                                    <li v-tooltip="'Deactivate'" v-if="row.statusString === 'Active'">
                                        <button class="btn btn-sm btn-danger" @click="toggleUserStatus(row.code)"><i class="las la-ban"></i></button>
                                    </li>
                                    <li v-tooltip="'Reactivate'" v-if="row.statusString === 'InActive'">
                                        <button class="btn btn-sm btn-primary" @click="toggleUserStatus(row.code)"><i
                                                class="las la-redo-alt"></i></button>
                                    </li>
                                </ul>
                            </template>
                            <template v-slot:statusString="{ value }">
                                <div class="user-management-status"
                                    :style="{ backgroundColor: value == 'Active' ? '#149414' : '#bbb' }"></div>
                            </template>
                        </dynamic-table>
                    </div>
                </div>
            </div>
            <div class="col-md-5">
                <div class="card">
                    <div class="card-header">
                        <h3>{{ $t('users.main.userGroups') }}</h3>
                    </div>
                    <div class="card-body">
                        <!-- toolbar -->
                        <div class="toolbar-new">
                            <div class="row pb-2">
                                <div class="col-md-6 ps-4 main-action-icons">
                                    <div class="d-inline-block link-box">
                                        <a href="#" @click.stop.prevent="addGroup(null, true)" class="text-center">
                                            <div><i class="las la-users"></i></div>
                                            <div>{{ $t('users.operation.addGroup') }}</div>
                                        </a>
                                    </div>
                                </div>
                                <div class="col-md-6 text-end pe-5 main-action-icons">
                                    <div class="d-inline-block link-box">
                                        <a href="#" @click.stop.prevent="refresh()" class="text-center">
                                            <div><i class="las la-redo-alt"></i></div>
                                            <div>{{ $t('operation.general.refresh') }}</div>
                                        </a>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Access groups' table-->
                        <dynamic-table :func="(params) => $dataProvider.accessGroups.getWithPaging(params)"
                            ref="tableAg" :columns="colsAg" :filters="{}" :multiple="false" :search="true"
                            @row-dblclick="showDetails($event)" :hoverActions="true" :actions="false">
                            <template v-slot:filter-statusString="{ filter }">
                                <multiselect :options="statuses" label="label" v-model="statusAg"
                                    @update:modelValue="filter({ status: $event })" :can-clear="false"
                                    :showLabels="false"></multiselect>
                            </template>
                            <template v-slot:hoverActions="{ row }">
                                <ul>
                                    <li v-tooltip="$t('operation.general.modify')">
                                        <button class="btn btn-sm btn-primary" @click="addGroup(row.code, false)">
                                            <i class="las la-pen"></i>
                                        </button>
                                    </li>
                                    <li v-tooltip="$t('operation.general.deactivate')" v-if="row.status === 1">
                                        <button class="btn btn-sm btn-danger" @click="toggleGroupStatus(row.code)"><i class="las la-ban"></i></button>
                                    </li>
                                    <li v-tooltip="$t('operation.general.reactivate')" v-if="row.status === 0">
                                        <button class="btn btn-sm btn-primary" @click="toggleGroupStatus(row.code)"><i
                                                class="las la-redo-alt"></i></button>
                                    </li>
                                </ul>
                            </template>

                            <template v-slot:statusString="{ value }">
                                <div class="user-management-status"
                                    :style="{ backgroundColor: value == 'Active' ? '#149414' : '#bbb' }"></div>
                            </template>

                        </dynamic-table>
                    </div>
                </div>
            </div>
        </div>

        <add-user-feature v-if="modal != null && modal.type == 'addUser'" :code="modal.code" :newUser="modal.newUser"
            @done="modal = null, refresh(), modify($event)" @close="modal = null, refresh()" />
        <add-group-feature v-if="modal != null && modal.type == 'addGroup'" :newGroup="modal.newGroup"
            :groupCode="modal.groupCode" @done="modal = null, refresh(), modify($event)"
            @close="modal = null, refresh()" />

    </div>
</template>

<script>
import Multiselect from '@vueform/multiselect'
import DynamicTable from '@/widgets/Table.vue'
import RouteRefreshMixin from '@/mixins/routeRefreshMixin.js'
import AddUserFeature from './usermanagement/AddUser'
import AddGroupFeature from './usermanagement/AddGroup'

import { defineComponent } from 'vue';
export default defineComponent({
    name: 'UserManagement',
    mixins: [RouteRefreshMixin],
    components: { DynamicTable, Multiselect, AddUserFeature, AddGroupFeature },
    data() {
        return {
            modal: null,
            defaultFilter: {
                orderBy: 'Code',
                desc: true,
                pageSize: 20,
                pageNo: 1,
            },
            status: null,
            statusAg: null,
            statuses: [
                {
                    label: this.$t('userStatus.InActive'),
                    value: "InActive"
                },
                {
                    label: this.$t('userStatus.Active'),
                    value: "Active"
                },
                {
                    label: this.$t('userStatus.All'),
                    value: null
                }],
            cols: [
                {
                    data: 'code',
                    title: this.$t('codeHeader'),
                    sortable: true,
                    filter: true,
                    width: 110
                },
                {
                    data: 'firstName',
                    title: this.$t('firstNameHeader'),
                    sortable: true,
                    filter: true,
                    width: 230
                },
                {
                    data: 'lastName',
                    title: this.$t('lastNameHeader'),
                    sortable: true,
                    filter: true,
                    width: 230
                },
                {
                    data: 'groupCode',
                    title: this.$t('groupCodeHeader'),
                    sortable: true,
                    filter: true,
                    width: 110
                },
                {
                    data: 'statusString',
                    title: this.$t('statusHeader'),
                    sortable: true,
                    filter: true,
                    width: 100
                }
            ],
            colsAg: [
                {
                    data: 'code',
                    title: this.$t('codeHeader'),
                    sortable: true,
                    filter: true,
                    width: 150
                },
                {
                    data: 'description',
                    title: this.$t('descriptionHeader'),
                    sortable: true,
                    filter: true,
                    width: 250
                },
                {
                    data: 'statusString',
                    title: this.$t('statusHeader'),
                    sortable: true,
                    filter: true,
                    width: 100
                }
            ]
        }
    },
    methods: {
        refresh() {
            this.$refs.table.refresh();
            this.$refs.tableAg.refresh();
        },
        addUser(code, newUser) {
            this.modal = { type: 'addUser', code: code, newUser: newUser }
        },
        addGroup(code, newGroup) {
            this.modal = { type: 'addGroup', groupCode: code, newGroup: newGroup };
        },
        toggleUserStatus(code) {
            this.$dataProvider.users.toggleStatus(code).then(() => {
                this.refresh();
            })
        },
        toggleGroupStatus(code) {
            this.$dataProvider.accessGroups.toggleStatus(code).then(() => {
                this.refresh();
            })
        }
    }

})
</script>

