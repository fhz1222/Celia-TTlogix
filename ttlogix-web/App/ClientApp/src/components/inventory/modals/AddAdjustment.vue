<script lang="ts" setup>
    // import vue stuff
    import { store } from '@/store';
    import { computed, ref } from 'vue';

    // import types
    import { Modal } from '@/store/commonModels/modal';
    import { ActionTypes } from '@/store/action-types';
    import { Customer } from '@/store/models/customer';

    // import widgets and modals
    import ModalWidget from '@/widgets/Modal3.vue';
    
    // props & emits
    const props = defineProps<{modalBase: Modal}>();
    const emit = defineEmits<{(e: 'close'): void, (e: 'addNew', customerCode: string | null): void }>();

    // local vars
    const customers = computed(() => store.state.customers);
    if(customers.value.length == 0) await store.dispatch(ActionTypes.INV_LOAD_CUSTOMERS);
    const customer = ref<Customer | null>(null);

    // functions
    const changeCustomer = (cust: Customer) => {
        customer.value = cust ? cust : null;
    }
    const createDisabled = () => {
        return customer.value == null;
    }
</script>

<template>
    <modal-widget :modalBase=modalBase @close="emit('close')">
        <template name="title" v-slot:title>
            <h5>{{modalBase.title}}</h5>
        </template>
        <template name="body" v-slot:body>
            <v-select :options="customers"                     
                      @update:modelValue="changeCustomer"
                      v-model="customer"
                      placeholder="Select a customer">
                      <template v-slot:selected-option="option">{{ option.code + ' - ' + option.name }}</template>
                      <template v-slot:option="option">{{option.code}} - {{option.name}}</template>
            </v-select>
        </template>
        <template name="footer" v-slot:footer>
            <div class="text-end">
                <button class="btn btn-sm btn-primary" 
                        :disabled="createDisabled()" 
                        @click="emit('addNew', customer ? customer.code : null)">
                    <i class="las la-plus-circle"></i> Create
                </button>
            </div>
        </template>
    </modal-widget>
</template>