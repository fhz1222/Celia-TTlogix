<script lang="ts" setup>
    // import vue stuff
    import { computed, ref, watch } from 'vue';
    import { useI18n } from 'vue-i18n';
    import { useStore } from '@/store';
    
    // import types
    import { Modal } from '@/store/commonModels/modal';
    import { CustomerInventory } from '@/store/models/customerInventory';
    import { ActionTypes } from '@/store/action-types';
    import { ControlProductCode } from '@/store/models/controlProductCode';

    // modals and widgets
    import ModalWidget from '@/widgets/Modal3.vue';

    // props & emits
    const props = defineProps<{ modalBase: Modal }>();
    const emit = defineEmits<{ (e: 'close'): void, (e: 'proceed', newCustomerInventory: CustomerInventory): void }>();

    // local vars
    const store = useStore();
    const { t } = useI18n({ useScope: 'global' });
    const productCodes = computed(() => store.state.productCodes);
    const controlCodes = computed(() => store.state.controlCodes);
    const loading = ref(false);
    const customerInventory = ref(new CustomerInventory());
    const toControlCodes = ref(new Array<ControlProductCode>());
    toControlCodes.value = [
        { code: 'CC1TYPE', label: 'Control Code 1' },
        { code: 'CC2TYPE', label: 'Control Code 2' },
        { code: 'CC3TYPE', label: 'Control Code 3' },
        { code: 'CC4TYPE', label: 'Control Code 4' },
        { code: 'CC5TYPE', label: 'Control Code 5' },
        { code: 'CC6TYPE', label: 'Control Code 6' }
    ]

    // functions
    const reload = async () => {
        loading.value = true;
        const inventory = await store.dispatch(ActionTypes.CS_GET_INVENTORY, {customerCode: props.modalBase.customProps});
        customerInventory.value = inventory;
        loading.value = false;
    }
    const proceed = async () => {
        emit('proceed', customerInventory.value);
        resetModal();
    }
    const cancel = async () => {
        emit('close');
        resetModal();
    }
    const resetModal = () => { return true; }

    watch(props.modalBase, (newV, oldV) => {
        if (newV.customProps) {
            customerInventory.value.customerCode = newV.customProps;
            reload();
        }
    })
</script>

<template>
    <modal-widget :modalBase=modalBase @close="emit('close', false)">
        <template name="title" v-slot:title>
            <h5>{{modalBase.title}}</h5>
        </template>
        <template name="body" v-slot:body>
            <div><strong>{{ t('customer.popups.inventoryControl.pCode') }}</strong></div>
            <div class="card">
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-4">{{ t('customer.popups.inventoryControl.pCode1') }}</div>
                        <div class="col-md-8">
                            <select class="form-select form-select-sm" v-model="customerInventory.pc1type">
                                <option v-for="pc in productCodes" :key="pc.code" :value="pc.code">{{pc.label}}</option>
                            </select>
                        </div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-4">{{ t('customer.popups.inventoryControl.pCode2') }}</div>
                        <div class="col-md-8">
                            <select class="form-select form-select-sm" v-model="customerInventory.pc2type">
                                <option v-for="pc in productCodes" :key="pc.code" :value="pc.code">{{pc.label}}</option>
                            </select>
                        </div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-4">{{ t('customer.popups.inventoryControl.pCode3') }}</div>
                        <div class="col-md-8">
                            <select class="form-select form-select-sm" v-model="customerInventory.pc3type">
                                <option v-for="pc in productCodes" :key="pc.code" :value="pc.code">{{pc.label}}</option>
                            </select>
                        </div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-4">{{ t('customer.popups.inventoryControl.pCode4') }}</div>
                        <div class="col-md-8">
                            <select class="form-select form-select-sm" v-model="customerInventory.pc4type">
                                <option v-for="pc in productCodes" :key="pc.code" :value="pc.code">{{pc.label}}</option>
                            </select>
                        </div>
                    </div>
                </div>
            </div>
            <div class="mt-2"><strong>{{ t('customer.popups.inventoryControl.cCode') }}</strong></div>
            <div class="card">
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-4">{{ t('customer.popups.inventoryControl.cCode1') }}</div>
                        <div class="col-md-8">
                            <select class="form-select form-select-sm" v-model="customerInventory.cc1type">
                                <option v-for="cc in controlCodes" :key="cc.code" :value="cc.code">{{cc.label}}</option>
                            </select>
                        </div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-4">{{ t('customer.popups.inventoryControl.cCode2') }}</div>
                        <div class="col-md-8">
                            <select class="form-select form-select-sm" v-model="customerInventory.cc2type">
                                <option v-for="cc in controlCodes" :key="cc.code" :value="cc.code">{{cc.label}}</option>
                            </select>
                        </div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-4">{{ t('customer.popups.inventoryControl.cCode3') }}</div>
                        <div class="col-md-8">
                            <select class="form-select form-select-sm" v-model="customerInventory.cc3type">
                                <option v-for="cc in controlCodes" :key="cc.code" :value="cc.code">{{cc.label}}</option>
                            </select>
                        </div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-4">{{ t('customer.popups.inventoryControl.cCode4') }}</div>
                        <div class="col-md-8">
                            <select class="form-select form-select-sm" v-model="customerInventory.cc4type">
                                <option v-for="cc in controlCodes" :key="cc.code" :value="cc.code">{{cc.label}}</option>
                            </select>
                        </div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-4">{{ t('customer.popups.inventoryControl.cCode5') }}</div>
                        <div class="col-md-8">
                            <select class="form-select form-select-sm" v-model="customerInventory.cc5type">
                                <option v-for="cc in controlCodes" :key="cc.code" :value="cc.code">{{cc.label}}</option>
                            </select>
                        </div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-4">{{ t('customer.popups.inventoryControl.cCode6') }}</div>
                        <div class="col-md-8">
                            <select class="form-select form-select-sm" v-model="customerInventory.cc6type">
                                <option v-for="cc in controlCodes" :key="cc.code" :value="cc.code">{{cc.label}}</option>
                            </select>
                        </div>
                    </div>
                </div>
            </div>
            <div class="mt-2"><strong>{{ t('customer.popups.inventoryControl.toControlBy') }}</strong></div>
            <div class="card">
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-4">{{ t('customer.popups.inventoryControl.productControlBy') }}</div>
                        <div class="col-md-8">
                            <select class="form-select form-select-sm" v-model="customerInventory.selectControlCode">
                                <option v-for="tc in toControlCodes" :key="tc.code" :value="tc.code">{{tc.label}}</option>
                            </select>
                        </div>
                    </div>
                </div>
            </div>
        </template>
        <template name="footer" v-slot:footer>
            <div class="row">
                <div class="col-md-12 text-end">
                    <button class="btn btn-sm btn-secondary me-2" @click="cancel"><i class="las la-ban"></i> {{ t('operation.general.cancel') }}</button>
                    <button class="btn btn-sm btn-secondary me-2" @click="proceed(false)"><i class="las la-save"></i> {{ t('operation.general.save') }}</button>
                </div>
            </div>
        </template>
    </modal-widget>
</template>