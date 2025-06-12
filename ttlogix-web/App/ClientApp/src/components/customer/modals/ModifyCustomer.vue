<script lang="ts" setup>
    // import vue stuff
    import { ref, watch, computed } from 'vue';
    import { useI18n } from 'vue-i18n';
    import { useStore } from '@/store';

    // import types
    import { Modal } from '@/store/commonModels/modal';
    import { CompanyProfileShort } from '@/store/models/companyProfile';
    import { CompanyProfileAddressBook, CompanyProfileAddressBookShort } from '@/store/models/companyProfileAddressBook';
    import { CompanyProfileAddressContact } from '@/store/models/companyProfileAddressContact';
    import { CustomerFull } from '@/store/models/customer';
    import { ActionTypes } from '@/store/action-types';
    import TabPanel from 'primevue/tabpanel';
    import TabView from 'primevue/tabview';

    // modals and widgets
    import ModalWidget from '@/widgets/Modal3.vue';
    import AddCompanyProfilePopup from '../../companyProfile/modals/AddCompanyProfile.vue'
    import AddAddressBookPopup from '../../companyProfile/modals/AddAddressBook.vue'
    import AddAddressContactPopup from '../../companyProfile/modals/AddAddressContact.vue'
    import ProcessingPopup from '@/widgets/ProcessingPopup.vue';

    // props & emits
    const props = defineProps<{ modalBase: Modal }>();
    const emit = defineEmits<{ (e: 'close'): void, (e: 'proceed', currentCompany: CustomerFull, addNext: boolean): void }>();
    const addressBooks = ref(new Array<CompanyProfileAddressBook>());
    const addressContacts = ref(new Array<CompanyProfileAddressContact>());
    const primaryAddress = ref(new CompanyProfileAddressBook());
    const billingAddress = ref(new CompanyProfileAddressBook());
    const shippingAddress = ref(new CompanyProfileAddressBook());
    const pic1 = ref(new CompanyProfileAddressContact());
    const pic2 = ref(new CompanyProfileAddressContact());
    const currentCustomer = ref(new CustomerFull());
    const companyProfiles = computed(() => store.state.companyProfileList.filter((cp) => cp.status == 1));

    // local vars
    const store = useStore();
    const { t } = useI18n({ useScope: 'global' });
    const currentCompany = ref(new CompanyProfileShort);
    const loading = ref(false);
    const active = ref(0);

    // functions
    const loadCustomer = async () => {
        loading.value = true;
        if (props.modalBase.customProps) {
            currentCustomer.value = await store.dispatch(ActionTypes.CS_GET_CUSTOMER, { customerCode: props.modalBase.customProps });
            setAddressBooks(currentCustomer.value.companyCode);
            setAddressContacts(currentCustomer.value.primaryAddress);
            setPrimaryAddress(currentCustomer.value.primaryAddress);
            setBillingAddress(currentCustomer.value.billingAddress);
            setShippingAddress(currentCustomer.value.shippingAddress);
            setPic(currentCustomer.value.pic1);
            setPic(currentCustomer.value.pic2, false);
        }
        loading.value = false;
    }
    const setAddressBooks = (cpCode: string | null = null) => {
        if (cpCode) addressBooks.value = (companyProfiles.value.filter((cp) => cp.code == cpCode))[0].addressBooks;
        else addressBooks.value = new Array<CompanyProfileAddressBook>();
    }
    const setAddressContacts = (abCode: string | null = null) => {
        if (abCode) addressContacts.value = (addressBooks.value.filter((ab) => ab.code == abCode))[0].addressContacts;
        else addressContacts.value = new Array<CompanyProfileAddressContact>();
    }
    const setPrimaryAddress = (abCode: string | null = null) => {
        if (abCode) primaryAddress.value = (addressBooks.value.filter((ab) => ab.code == (abCode ? abCode : currentCustomer.value.primaryAddress)))[0];
        else primaryAddress.value = new CompanyProfileAddressBook();
    }
    const setBillingAddress = (abCode: string | null = null) => {
        if (abCode) billingAddress.value = (addressBooks.value.filter((ab) => ab.code == (abCode ? abCode : currentCustomer.value.billingAddress)))[0];
        else billingAddress.value = new CompanyProfileAddressBook();
    }
    const setShippingAddress = (abCode: string | null = null) => {
        if (abCode) shippingAddress.value = (addressBooks.value.filter((ab) => ab.code == (abCode ? abCode : currentCustomer.value.shippingAddress)))[0];
        else shippingAddress.value = new CompanyProfileAddressBook();
    }
    const setPic = (selectedContactCode: string | null = null, isPic1: boolean = true) => {
        if (selectedContactCode) {
            if (isPic1) pic1.value = (addressContacts.value.filter((ac) => ac.code == (selectedContactCode ? selectedContactCode : currentCustomer.value.pic1)))[0];
            else pic2.value = (addressContacts.value.filter((ac) => ac.code == (selectedContactCode ? selectedContactCode : currentCustomer.value.pic2)))[0];
        }
        else {
            if (isPic1) pic1.value = new CompanyProfileAddressContact();
            else pic2.value = new CompanyProfileAddressContact();
        }
    }
    const primaryAddressChange = () => {
        setPrimaryAddress(currentCustomer.value.primaryAddress);
        setAddressContacts(currentCustomer.value.primaryAddress);
        currentCustomer.value.pic1 = null;
        currentCustomer.value.pic2 = null;
        pic1.value = new CompanyProfileAddressContact();
        pic2.value = new CompanyProfileAddressContact();
    }
    const companyChange = () => {
        setAddressBooks(currentCustomer.value.companyCode);
        setAddressContacts();
        currentCustomer.value.pic1 = null;
        currentCustomer.value.pic2 = null;
        currentCustomer.value.primaryAddress = null;
        currentCustomer.value.shippingAddress = null;
        currentCustomer.value.billingAddress = null;
        primaryAddress.value = new CompanyProfileAddressBook();
        billingAddress.value = new CompanyProfileAddressBook();
        shippingAddress.value = new CompanyProfileAddressBook();
        pic1.value = new CompanyProfileAddressContact();
        pic2.value = new CompanyProfileAddressContact();
    }
    const canDoNext = () => {
        return !props.modalBase.customProps
    }
    const canSave = () => {
        let cst = currentCustomer.value;
        return cst.code && cst.companyCode && cst.name && cst.primaryAddress && cst.billingAddress
            && cst.shippingAddress && cst.pic1;
    }

    // Company Profile related functions
    const addNewCompany = () => {
        addCompanyProfileModal.value.on = true;
    }
    const addNewCompanyExecute = async (company: CompanyProfileShort, addNext: boolean) => {
        if (!addNext) closeModal(addCompanyProfileModal.value);
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.CP_UPDATE_COMPANY, { companyProfile: company }).catch(() => { closeModal(waitModal.value) })
        await store.dispatch(ActionTypes.CP_LOAD_LIST).catch(() => { closeModal(waitModal.value) })
        closeModal(waitModal.value);
    }

    const addNewAddressBook = () => {
        addAddressBookModal.value.customProps = currentCustomer.value.companyCode;
        addAddressBookModal.value.on = true;
    }
    const addNewAddressBookExecute = async (addressBook: CompanyProfileAddressBookShort, addNext: boolean) => {
        if (!addNext) closeModal(addAddressBookModal.value);
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.CP_UPDATE_ADDRESS_BOOK, { addressBook: addressBook }).catch(() => { closeModal(waitModal.value) })
        closeModal(waitModal.value);
    }

    const addNewAddressContact = () => {
        addAddressContactModal.value.on = true;
        addAddressContactModal.value.customProps = currentCustomer.value.primaryAddress;
    }
    const addNewAddressExecute = async (addressContact: CompanyProfileAddressContact, addNext: boolean) => {
        if (!addNext) closeModal(addAddressContactModal.value);
        waitModal.value.on = true;
        await (store.dispatch(ActionTypes.CP_UPDATE_ADDRESS_CONTACT, { companyCode: currentCustomer.value.companyCode, addressContact: addressContact }))
                    .catch(() => { closeModal(waitModal.value) })
        closeModal(waitModal.value);
    }

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const waitModal = ref(new Modal(500, t('generic.processing'), false));
    const addCompanyProfileModal = ref(new Modal(700, t('companyProfile.addCompanyProfile'), true));
    const addAddressBookModal = ref(new Modal(750, t('companyProfile.addAddressBook'), true));
    const addAddressContactModal = ref(new Modal(700, t('companyProfile.addAddressContact'), true));

    const proceed = async (addNext: boolean) => {
        emit('proceed', currentCustomer.value, addNext);
        resetModal();
    };
    const cancel = async () => {
        emit('close');
        resetModal();
    };

    watch(props.modalBase, (nV, oV) => {
        if (nV.on == false) resetModal();
        if (nV.on == true && nV.customProps != null) loadCustomer();
    })
    const resetModal = () => {
        active.value = 0;
        addressBooks.value = new Array<CompanyProfileAddressBook>();
        addressContacts.value = new Array<CompanyProfileAddressContact>();
        primaryAddress.value = new CompanyProfileAddressBook();
        billingAddress.value = new CompanyProfileAddressBook();
        shippingAddress.value = new CompanyProfileAddressBook();
        pic1.value = new CompanyProfileAddressContact();
        pic2.value = new CompanyProfileAddressContact();
        currentCustomer.value = new CustomerFull();
    }
</script>

<template>
    <modal-widget :modalBase=modalBase @close="emit('close', false)">
        <template name="title" v-slot:title>
            <h5>{{modalBase.title}}</h5>
        </template>
        <template name="body" v-slot:body>
            <TabView v-model:activeIndex="active">
                <TabPanel :header="t('customer.popups.modifyCustomer.general')">
                    <div class="card mt-3">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-md-3 pt-1 text-end no-right-padding">{{t('customer.popups.modifyCustomer.customerCode')}}</div>
                                <div class="col-md-3">
                                    <input class="form-control form-control-sm" :maxlength="10" :disabled="props.modalBase.customProps" v-model="currentCustomer.code" />
                                </div>
                            </div>
                            <div class="row mt-3">
                                <div class="col-md-3 pt-1 text-end no-right-padding">{{t('customer.popups.modifyCustomer.customerName')}}</div>
                                <div class="col-md-9">
                                    <input class="form-control form-control-sm" v-model="currentCustomer.name" />
                                </div>
                            </div>
                            <div class="row mt-3">
                                <div class="col-md-3 pt-1 text-end no-right-padding">{{t('companyProfile.companyName')}}</div>
                                <div class="col-md-8">
                                    <select class="form-select form-select-sm" v-model="currentCustomer.companyCode" @change="companyChange()">
                                        <option v-for="cp in companyProfiles" :key="cp.code" :value="cp.code">{{cp.name}}</option>
                                    </select>
                                </div>
                                <div class="col-md-1 no-left-padding text-end">
                                    <button class="btn btn-sm btn-primary" @click="addNewCompany"><i class="las la-home"></i></button>
                                </div>
                            </div>
                            <div class="row mt-3">
                                <div class="col-md-3 pt-1 text-end no-right-padding">{{t('customer.popups.modifyCustomer.businessUnit')}}</div>
                                <div class="col-md-3">
                                    <input class="form-control form-control-sm" :maxlength="10" v-model="currentCustomer.bizUnit" />
                                </div>
                            </div>
                            <div class="row mt-3">
                                <div class="col-md-3 pt-1 text-end no-right-padding">{{t('customer.popups.modifyCustomer.businessUnit')}}</div>
                                <div class="col-md-9">
                                    <input class="form-control form-control-sm" v-model="currentCustomer.buname" />
                                </div>
                            </div>
                        </div>
                    </div>
                </TabPanel>
                <!-- Primary Address -->
                <TabPanel :header="t('customer.popups.modifyCustomer.primaryAddress')">
                    <div class="row mt-3">
                        <div class="col-md-3 pt-1 text-end">{{t('companyProfile.addressBook')}}</div>
                        <div class="col-md-8">
                            <select class="form-select form-select-sm" v-model="currentCustomer.primaryAddress"
                                    @change="primaryAddressChange()">
                                <option v-for="ab in addressBooks.filter((aab)=>aab.status>0)" :key="ab.code" :value="ab.code">{{ab.code}}</option>
                            </select>
                        </div>
                        <div class="col-md-1 no-left-padding text-end">
                            <button class="btn btn-sm btn-primary button-i" :disabled="!currentCustomer.companyCode" @click="addNewAddressBook">
                                <i class="las la-address-book big-glyphs"></i>
                            </button>
                        </div>
                    </div>
                    <div class="card mt-3">
                        <div class="card-header"><h5>{{t('customer.popups.modifyCustomer.primaryAddress')}}</h5></div>
                        <div class="card-body">
                            <div class="row">
                                <div class="col-md-3 text-end">{{t('companyProfile.address')}}</div>
                                <div class="col-md-9">
                                    <div>{{primaryAddress.address1 ?? '-'}}</div>
                                    <div class="mt-1">{{primaryAddress.address2 ?? ''}}</div>
                                    <div class="mt-1">{{primaryAddress.address3 ?? ''}}</div>
                                    <div class="mt-1">{{primaryAddress.address4 ?? ''}}</div>
                                </div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-3 text-end">{{t('companyProfile.postalCode')}}</div>
                                <div class="col-md-9">{{primaryAddress.postCode ?? '-'}}</div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-3 text-end">{{t('companyProfile.country')}}</div>
                                <div class="col-md-9">{{primaryAddress.country ?? '-'}}</div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-3 text-end">{{t('companyProfile.telNo')}}</div>
                                <div class="col-md-9">{{primaryAddress.telNo ?? '-'}}</div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-3 text-end">{{t('companyProfile.faxNo')}}</div>
                                <div class="col-md-9">{{primaryAddress.faxNo ?? '-'}}</div>
                            </div>
                        </div>
                    </div>
                </TabPanel>
                <!-- Billing Address -->
                <TabPanel :header="t('customer.popups.modifyCustomer.billingAddress')">
                    <div class="row mt-3">
                        <div class="col-md-3 pt-1 text-end">{{t('companyProfile.addressBook')}}</div>
                        <div class="col-md-8">
                            <select class="form-select form-select-sm" v-model="currentCustomer.billingAddress"
                                    @change="setBillingAddress(currentCustomer.billingAddress)">
                                <option v-for="ab in addressBooks.filter((aab)=>aab.status>0)" :key="ab.code" :value="ab.code">{{ab.code}}</option>
                            </select>
                        </div>
                        <div class="col-md-1 no-left-padding text-end">
                            <button class="btn btn-sm btn-primary button-i" :disabled="!currentCustomer.companyCode" @click="addNewAddressBook">
                                <i class="las la-address-book big-glyphs"></i>
                            </button>
                        </div>
                    </div>
                    <div class="card mt-3">
                        <div class="card-header">{{t('customer.popups.modifyCustomer.billingAddress')}}</div>
                        <div class="card-body">
                            <div class="row">
                                <div class="col-md-3 text-end">{{t('companyProfile.address')}}</div>
                                <div class="col-md-9">
                                    <div>{{billingAddress.address1 ?? '-'}}</div>
                                    <div class="mt-1">{{billingAddress.address2 ?? ''}}</div>
                                    <div class="mt-1">{{billingAddress.address3 ?? ''}}</div>
                                    <div class="mt-1">{{billingAddress.address4 ?? ''}}</div>
                                </div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-3 text-end">{{t('companyProfile.postalCode')}}</div>
                                <div class="col-md-9">{{billingAddress.postCode ?? '-'}}</div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-3 text-end">{{t('companyProfile.country')}}</div>
                                <div class="col-md-9">{{billingAddress.country ?? '-'}}</div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-3 text-end">{{t('companyProfile.telNo')}}</div>
                                <div class="col-md-9">{{billingAddress.telNo ?? '-'}}</div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-3 text-end">{{t('companyProfile.faxNo')}}</div>
                                <div class="col-md-9">{{billingAddress.faxNo ?? '-'}}</div>
                            </div>
                        </div>
                    </div>
                </TabPanel>
                <!-- Shipping Address -->
                <TabPanel :header="t('customer.popups.modifyCustomer.shippingAddress')">
                    <div class="row mt-3">
                        <div class="col-md-3 pt-1 text-end">{{t('companyProfile.addressBook')}}</div>
                        <div class="col-md-8">
                            <select class="form-select form-select-sm" v-model="currentCustomer.shippingAddress"
                                    @change="setShippingAddress(currentCustomer.shippingAddress)">
                                <option v-for="ab in addressBooks.filter((aab)=>aab.status>0)" :key="ab.code" :value="ab.code">{{ab.code}}</option>
                            </select>
                        </div>
                        <div class="col-md-1 no-left-padding text-end">
                            <button class="btn btn-sm btn-primary button-i" :disabled="!currentCustomer.companyCode" @click="addNewAddressBook">
                                <i class="las la-address-book big-glyphs"></i>
                            </button>
                        </div>
                    </div>
                    <div class="card mt-3">
                        <div class="card-header">{{t('customer.popups.modifyCustomer.shippingAddress')}}</div>
                        <div class="card-body">
                            <div class="row">
                                <div class="col-md-3 text-end">{{t('companyProfile.address')}}</div>
                                <div class="col-md-9">
                                    <div>{{shippingAddress.address1 ?? '-'}}</div>
                                    <div class="mt-1">{{shippingAddress.address2 ?? ''}}</div>
                                    <div class="mt-1">{{shippingAddress.address3 ?? ''}}</div>
                                    <div class="mt-1">{{shippingAddress.address4 ?? ''}}</div>
                                </div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-3 text-end">{{t('companyProfile.postalCode')}}</div>
                                <div class="col-md-9">{{shippingAddress.postCode ?? '-'}}</div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-3 text-end">{{t('companyProfile.country')}}</div>
                                <div class="col-md-9">{{shippingAddress.country ?? '-'}}</div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-3 text-end">{{t('companyProfile.telNo')}}</div>
                                <div class="col-md-9">{{shippingAddress.telNo ?? '-'}}</div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-3 text-end">{{t('companyProfile.faxNo')}}</div>
                                <div class="col-md-9">{{shippingAddress.faxNo ?? '-'}}</div>
                            </div>
                        </div>
                    </div>
                </TabPanel>
                <!-- PIC 1 -->
                <TabPanel :header="t('customer.popups.modifyCustomer.pic1')">
                    <div class="row mt-3">
                        <div class="col-md-3 pt-1 text-end">{{t('customer.popups.modifyCustomer.addressContact')}}</div>
                        <div class="col-md-8">
                            <select class="form-select form-select-sm" v-model="currentCustomer.pic1" @change="setPic(currentCustomer.pic1)">
                                <option v-for="ac in addressContacts.filter((acc)=>acc.status>0)" :key="ac.code" :value="ac.code">{{ac.name}}</option>
                            </select>
                        </div>
                        <div class="col-md-1 no-left-padding text-end">
                            <button class="btn btn-sm btn-primary button-i" :disabled="!currentCustomer.primaryAddress" @click="addNewAddressContact">
                                <i class="las la-address-book big-glyphs"></i>
                            </button>
                        </div>
                    </div>
                    <div class="card mt-3">
                        <div class="card-header">{{t('customer.popups.modifyCustomer.pic1Full')}}</div>
                        <div class="card-body">
                            <div class="row">
                                <div class="col-md-3 text-end">{{t('companyProfile.picName')}}</div>
                                <div class="col-md-9">
                                    <div>{{pic1.name ?? '-'}}</div>
                                </div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-3 text-end">{{t('companyProfile.telNo')}}</div>
                                <div class="col-md-9">{{pic1.telNo ?? '-'}}</div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-3 text-end">{{t('companyProfile.faxNo')}}</div>
                                <div class="col-md-9">{{pic1.faxNo ?? '-'}}</div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-3 text-end">{{t('companyProfile.email')}}</div>
                                <div class="col-md-9">{{pic1.email ?? '-'}}</div>
                            </div>
                        </div>
                    </div>
                </TabPanel>
                <!-- PIC 2 -->
                <TabPanel :header="t('customer.popups.modifyCustomer.pic2')">
                    <div class="row mt-3">
                        <div class="col-md-3 pt-1 text-end">{{t('customer.popups.modifyCustomer.addressContact')}}</div>
                        <div class="col-md-8">
                            <select class="form-select form-select-sm" v-model="currentCustomer.pic2" @change="setPic(currentCustomer.pic2, false)">
                                <option v-for="ac in addressContacts.filter((acc)=>acc.status>0)" :key="ac.code" :value="ac.code">{{ac.name}}</option>
                            </select>
                        </div>
                        <div class="col-md-1 no-left-padding text-end">
                            <button class="btn btn-sm btn-primary button-i" :disabled="!currentCustomer.primaryAddress" @click="addNewAddressContact">
                                <i class="las la-address-book big-glyphs"></i>
                            </button>
                        </div>
                    </div>
                    <div class="card mt-3">
                        <div class="card-header">{{t('customer.popups.modifyCustomer.pic2Full')}}</div>
                        <div class="card-body">
                            <div class="row">
                                <div class="col-md-3 text-end">{{t('companyProfile.picName')}}</div>
                                <div class="col-md-9">
                                    <div>{{pic2.name ?? '-'}}</div>
                                </div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-3 text-end">{{t('companyProfile.telNo')}}</div>
                                <div class="col-md-9">{{pic2.telNo ?? '-'}}</div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-3 text-end">{{t('companyProfile.faxNo')}}</div>
                                <div class="col-md-9">{{pic2.faxNo ?? '-'}}</div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-3 text-end">{{t('companyProfile.email')}}</div>
                                <div class="col-md-9">{{pic2.email ?? '-'}}</div>
                            </div>
                        </div>
                    </div>
                </TabPanel>
            </TabView>
            <add-company-profile-popup :modalBase="addCompanyProfileModal" @proceed="addNewCompanyExecute" @close="closeModal(addCompanyProfileModal)" />
            <add-address-book-popup :modalBase="addAddressBookModal" @close="closeModal(addAddressBookModal)" @proceed="addNewAddressBookExecute" />
            <add-address-contact-popup :modalBase="addAddressContactModal" @close="closeModal(addAddressContactModal)" @proceed="addNewAddressExecute" />
            <processing-popup :modalBase=waitModal />
        </template>
        <template name="footer" v-slot:footer>
            <div class="row">
                <div class="col-md-12 text-end">
                    <button class="btn btn-sm btn-secondary" @click="cancel"><i class="las la-ban"></i> {{ t('operation.general.cancel') }}</button>
                    <button class="btn btn-sm btn-secondary ms-2" @click="proceed(false)" :disabled="!canSave()"><i class="las la-save"></i> {{ t('operation.general.save') }}</button>
                    <button class="btn btn-sm btn-secondary ms-2" @click="proceed(true)" v-if="canDoNext()" :disabled="!canSave()"><i class="las la-save"></i> {{t('companyProfile.saveNext')}}</button>
                </div>
            </div>
        </template>
    </modal-widget>
</template>