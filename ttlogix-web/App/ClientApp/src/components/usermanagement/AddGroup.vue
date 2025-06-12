<template>
    <modal containerClass="modal-add-group">
        <template name="header" v-slot:header>
            <span v-if="newGroup">{{ $t('userGroups.addGroup.addGroupTitle') }}</span>
            <span v-if="!newGroup">{{ $t('userGroups.addGroup.edit') }} {{ model.description }}</span>
        </template>
        <template name="body" v-slot:body>

            <!-- Group ID -->
            <div class="row">
                <div class="col-md-3 text-end pt-1 no-right-padding">
                    {{ $t('userGroups.addGroup.groupId') }}
                </div>
                <div class="col-md-9">
                    <input type="text" v-model="model.code" class="form-control form-control-sm"
                        :disabled="!newGroup" />
                    <form-error :errors="errors" field="JobNo" />
                </div>
            </div>

            <!-- Group Name -->
            <div class="row mt-2">
                <div class="col-md-3 text-end pt-1 no-right-padding">
                    {{ $t('userGroups.addGroup.groupName') }}
                </div>
                <div class="col-md-9">
                    <input type="text" v-model="model.description" class="form-control form-control-sm" />
                    <form-error :errors="errors" field="JobNo" />
                </div>
            </div>

            <!-- Priviliges Tree -->
            <div class="row mt-3" v-if="privTree.children.length > 0">
                <div class="col-md-12">
                    <div class="card">
                        <div class="card-body">
                            <div class="scrollable-45">
                                <ul class="recursive-wrapper">
                                    <recursive v-bind:folder="privTree"></recursive>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </template>

        <template name="footer" v-slot:footer>
            <button class="btn btn-sm btn-primary me-2" type="button" @click.stop="process()" :disabled="processing">
                <i v-bind:class="['las', privTree.children.length == 0 ? 'la-plus-circle' : 'la-save']"></i>
                {{ privTree.children.length == 0 ? $t('operation.general.add') : $t('operation.general.save') }}
            </button>
            <button v-if="privTree.children.length == 0" class="btn btn-sm btn-primary me-2" type="button"
                @click.stop="process(true)" :disabled="processing">
                <i class="las la-plus-circle"></i> {{ $t('userGroups.operation.addAndSetPriviliges') }}
            </button>
            <button class="btn btn-sm btn-secondary" type="button" @click.stop="$emit('close')" :disabled="processing">
                <i class="las la-times"></i> {{ $t('operation.general.close') }}
            </button>
        </template>
    </modal>
</template>
<script>
import FormError from '@/widgets/FormError'
import Recursive from '@/widgets/Recursive.vue'
/*import SelectAccessGroup from '@/widgets/SelectAccessGroup.vue'
import SelectWarehouse from '@/widgets/SelectWarehouse.vue'*/

//import SimpleTableScroll from '@/widgets/SimpleTableScroll'
import { defineComponent } from 'vue';
export default defineComponent({
    props: {
        title: {
            default: 'Add Loading Detail'
        },
        header: {},
        newGroup: {
            default: null
        },
        groupCode: {
            default: null
        },
        isSubmenu: {
            type: Boolean,
            default: false
        },
        menu: {
            type: Array
        }
    },
    created() {
        this.newGroupLocal = this.newGroup;
        this.refresh();
    },
    components: { FormError, Recursive/*SelectAccessGroup, SelectWarehouse, DateTime SimpleTableScroll*/ },
    data() {
        return {
            errors: {},
            processing: false,
            loading: true,
            accessGroups: [],
            warehouses: [],
            warehouse: null,
            accessGroup: null,
            newGroupLocal: false,
            privTree: {
                code: null,
                moduleName: null,
                shortName: null,
                children: [],
                isChecked: false,
                parent: null
            },
            model: {
                code: null,
                description: null,
                status: 1
            }
        }
    },
    methods: {
        refresh() {
            if (this.newGroupLocal === false) {
                this.$dataProvider.accessGroups.getAccessGroup(this.groupCode).then((group) => {
                    this.model = group;
                    this.originalGroupCode = this.model.code;
                    this.$dataProvider.accessGroups.getPrivilegesTree(this.model.code).then((privTree) => {
                        this.privTree = privTree;
                    })
                })
            }
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
        process(setPriviliges = false) {
            if (this.newGroupLocal) {
                this.$dataProvider.accessGroups.createAccessGroup(this.model).then((newGroup) => {
                    if (!setPriviliges) {
                        this.$emit('close');
                    } else {
                        this.newGroupLocal = false;
                        this.$dataProvider.accessGroups.getPrivilegesTree(newGroup.code).then((privTree) => {
                            this.privTree = privTree;
                        });
                    }
                });
            } else {
                this.$dataProvider.accessGroups.patchAccessGroup(this.model.code, this.model).then(() => {
                    this.$dataProvider.accessGroups.updatePrivilegesTree(this.model.code, this.privTree).then(() => {
                        this.$emit('close');
                    })
                })
            }
        },
    }
})
</script>
<style lang="scss">
.modal-add-group {
    .modal-container {
        width: 45vw;
    }

    ul {
        list-style-type: none;
    }
}
</style>