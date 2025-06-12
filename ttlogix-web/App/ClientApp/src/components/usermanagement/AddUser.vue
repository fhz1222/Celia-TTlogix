<template>
    <modal containerClass="modal-add-user">
        <template name="header" v-slot:header>
            <span v-if="newUser">{{$t('users.addUser.addUserTitle')}}</span>
            <span v-if="!newUser">{{$t('users.addUser.edit')}} {{model.firstName}} {{model.lastName}}</span>
        </template>
        <template name="body" v-slot:body>

            <!-- User ID -->
            <div class="row">
                <div class="col-md-3 text-end pt-1 no-right-padding">
                    {{$t('users.addUser.userId')}}
                </div>
                <div class="col-md-9">
                    <input type="text" v-model="model.code" class="form-control form-control-sm" :disabled="!newUser" />
                    <form-error :errors="errors" field="JobNo" />
                </div>
            </div>

            <!-- First Name -->
            <div class="row mt-2">
                <div class="col-md-3 text-end pt-1 no-right-padding">
                    {{$t('users.addUser.firstName')}}
                </div>
                <div class="col-md-9">
                    <input type="text" v-model="model.firstName" class="form-control form-control-sm" />
                    <form-error :errors="errors" field="JobNo" />
                </div>
            </div>

            <!-- Last Name -->
            <div class="row mt-2">
                <div class="col-md-3 text-end pt-1 no-right-padding">
                    {{$t('users.addUser.lastName')}}
                </div>
                <div class="col-md-9">
                    <input type="text" v-model="model.lastName" class="form-control form-control-sm" />
                    <form-error :errors="errors" field="JobNo" />
                </div>
            </div>

            <!-- Password -->
            <div class="row mt-2">
                <div class="col-md-3 text-end pt-1 no-right-padding">
                    {{$t('users.addUser.password')}}
                </div>
                <div class="col-md-9">
                    <input type="password" v-model="model.password" class="form-control form-control-sm" />
                    <form-error :errors="errors" field="JobNo" />
                </div>
            </div>

            <!-- Confirm Password -->
            <div class="row mt-2">
                <div class="col-md-3 text-end pt-1 no-right-padding">
                    {{$t('users.addUser.confirmPassword')}}
                </div>
                <div class="col-md-9">
                    <input type="password" v-model="model.confirmPassword" class="form-control form-control-sm" />
                    <form-error :errors="errors" field="JobNo" />
                </div>
            </div>

            <!-- Group -->
            <div class="row mt-2">
                <div class="col-md-3 text-end pt-1 no-right-padding">
                    {{$t('users.addUser.group')}}
                </div>
                <div class="col-md-9 multiselect-as-select">
                    <select-access-group v-model="model.groupCode" :accessGroup="accessGroup"></select-access-group>
                    <form-error :errors="errors" field="accessGroups" />
                </div>
            </div>

            <!-- Warehouse -->
            <div class="row mt-2 multiselect-as-select">
                <div class="col-md-3 text-end pt-1 no-right-padding">
                    {{$t('users.addUser.warehouse')}}
                </div>
                <div class="col-md-9">
                    <select-warehouse v-model="model.whsCode" :warehouse="warehouse"></select-warehouse>
                    <form-error :errors="errors" field="warehouses" />
                </div>
            </div>

        </template>
        <template name="footer" v-slot:footer>
            <button class="btn btn-sm btn-primary me-2" type="button" @click.stop="process()" :disabled="processing">
                <i :class="['las', newUser ? 'la-plus-circle' : 'la-save']"></i> {{newUser ? $t('operation.general.add') : $t('operation.general.save')}}
            </button>
            <button v-if="newUser" class="btn btn-sm btn-primary me-5" type="button" @click.stop="process(true)" :disabled="processing">
                <i class="las la-plus-circle"></i> {{$t('operation.general.saveAndAddNext')}}
            </button>
            <button class="btn btn-sm btn-secondary" type="button" @click.stop="$emit('close')" :disabled="processing">
                <i class="las la-times"></i> {{$t('operation.general.close')}}
            </button>
        </template>
    </modal>
</template>
<script>
    import FormError from '@/widgets/FormError'
    import SelectAccessGroup from '@/widgets/SelectAccessGroup.vue'
    import SelectWarehouse from '@/widgets/SelectWarehouse.vue'
   
    //import SimpleTableScroll from '@/widgets/SimpleTableScroll'
import { defineComponent } from 'vue';
export default defineComponent({
    props: {
        title: {
            default: 'Add Loading Detail'
        },
        header: {},
        code: {
            default: null
        },
        newUser: {
            default: null
        }
    },
    created() {
        this.refresh()
    },
        components: { FormError, SelectAccessGroup, SelectWarehouse/*DateTime SimpleTableScroll*/},
    data() {
        return {
            errors: {},
            processing: false,
            loading: true,
            accessGroups: [],
            warehouses: [],
            warehouse: null,
            accessGroup: null,
            model: {
                statusString: 'Active',
                code: null,
                firstName: null,
                lastName: null,
                whsCode: this.$auth.user().whsCode,
                groupCode: null,
                status: 1,
                confirmPassword: null,
                password: null
            }
        }
    },
    methods: {
        refresh() {
            if (this.newUser === false) {
                this.$dataProvider.users.getUser(this.code).then((user) => {
                    this.model = user;
                });
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
        process(addNext = false) {
            if (this.newUser) {
                this.$dataProvider.users.createUser(this.model).then(() => {
                    if (!addNext) {
                        this.$emit('close');
                    } else {
                        this.reset();
                    }
                });
            } else {
                this.$dataProvider.users.patchUser(this.model).then(() => {
                    this.$emit('close');
                })
            }
            return true;
        },
        reset() {
            this.model.statusString = "";
            this.model.code = "";
            this.model.firstName = "";
            this.model.lastName = "";
            this.model.whsCode = "PL";
            this.model.groupCode = "";
            this.model.status = 0;
            this.model.confirmPassword = "";
            this.model.password = "";
        }
    }
})
</script>
<style lang="scss">
    .modal-add-user {
        .modal-container {
            width: 45vw;
        }
    }
</style>