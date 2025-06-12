<script lang="ts" setup>

    import bus from '@/event_bus.js'
    // import vue stuff
    import { useStore } from '@/store';
    import { computed, ref, onActivated, onDeactivated, watch } from 'vue';
    import { ActionTypes } from '@/store/action-types';
    import { useI18n } from 'vue-i18n';
    import Tree from 'primevue/tree';
    import { TreeNode } from 'primevue/treenode';

    // import types
    import { MenuItem, TableColumn } from '@/store/commonModels/config';
    import { Modal } from '@/store/commonModels/modal';
    import { CompanyProfile } from '@/store/models/companyProfile';
    import { CompanyProfileShort } from '@/store/models/companyProfile';
    import { CompanyProfileAddressBook, CompanyProfileAddressBookShort } from '../../store/models/companyProfileAddressBook';
    import { CompanyProfileAddressContact } from '../../store/models/companyProfileAddressContact';

    // import widgets and modals
    import ProcessingPopup from '@/widgets/ProcessingPopup.vue';
    import SectionMenu from '@/widgets/SectionMenu.vue';
    import ConfirmPopup from '@/widgets/ConfirmPopup.vue';
    import SelectCountry from '@/widgets/SelectCountry.vue'
    import AddCompanyProfilePopup from './modals/AddCompanyProfile.vue'
    import AddAddressBookPopup from './modals/AddAddressBook.vue'
    import AddAddressContactPopup from './modals/AddAddressContact.vue'
    
    // import common logic
    import * as pagingFn from '@/store/commonFunctions/paging';
    import * as sortingFn from '@/store/commonFunctions/sorting';
    import * as filteringFn from '@/store/commonFunctions/filtering';
    import * as miscFn from '@/store/commonFunctions/miscellaneous';
    import * as routerFn from '@/router';

    // init vue stuff
    const store = useStore();
    const { t } = useI18n({ useScope: 'global' });

    // local vars
    const list_CP = computed(() => store.state.companyProfileList);
    const customersDict: { [id: string]: string } = {};
    const loadingList = ref(false);
    const companyTreeNodes = ref(new Array<TreeNode>());
    const selectedKey = ref('0');
    const showCompanyCard = ref(false);
    const showAddresBookCard = ref(false);
    const showAddressCard = ref(false);
    const currentCompany = ref(new CompanyProfileShort());
    const currentAddressBook = ref(new CompanyProfileAddressBookShort());
    const currentAddressContact = ref(new CompanyProfileAddressContact());

    
    // functions
    const reloadList = async () => {
        loadingList.value = true;
        await store.dispatch(ActionTypes.CP_LOAD_LIST)
            .then(() => {
                buildTheTree();
                loadingList.value = false;
            }).catch(() => loadingList.value = false);
    }
    const buildTheTree = () => {
        companyTreeNodes.value = [];
        list_CP.value.forEach((cp, cpindex) => {
            companyTreeNodes.value.push({ key: '' + cpindex, label: cp.name,  icon: 'las la-home' + (cp.status == 1 ? '' : ' deactivated'), type: 'company', data: { code: cp.code }, style: cp.status == 1 ? '' : 'color: #ccc', children: [] })
            if (cp.addressBooks.length > 0) {
                cp.addressBooks.forEach((ab, abindex) => {
                    companyTreeNodes.value[cpindex].children?.push({ key: cpindex + '-' + abindex, label: ab.code, icon: 'las la-address-book' + (ab.status == 1 ? '' : ' deactivated'), type: 'addressBook', data: { cpCode: cp.code, code: ab.code }, style: ab.status == 1 ? '' : 'color: #ccc', children: [] })
                    if (ab.addressContacts.length > 0) {
                        ab.addressContacts.forEach((ac, aindex) => {
                            //@ts-ignore
                            companyTreeNodes.value[cpindex].children[abindex].children?.push({
                                key: cpindex + '-' + abindex + '-' + aindex, label: ac.name, icon: 'las la-address-card' + (ac.status == 1 ? '' : ' deactivated'), type: 'addressContact', data: { cpCode: cp.code, abCode: ab.code, code: ac.code }, style: ac.status == 1 ? '' : 'color: #ccc'
                            })
                            
                        })
                    }
                })
            }
        });
    }
    const showNode = (item: any) => {
        showCompanyCard.value = item.type == 'company' ? true : false;
        showAddresBookCard.value = item.type == 'addressBook' ? true : false;
        showAddressCard.value = item.type == 'addressContact' ? true : false;
        currentCompany.value = item.type == 'company' ? getCompany(item.data.code) : new CompanyProfileShort();
        currentAddressBook.value = item.type == 'addressBook' ? getAddressBook(item.data.cpCode, item.label) : new CompanyProfileAddressBookShort();
        if (item.type == 'addressContact') {
            currentAddressContact.value = getAddress(item.data.cpCode, item.data.abCode, item.data.code)
            currentAddressContact.value.companyProfileCode = item.data.cpCode;
        }
    }
    const getCompany = (code: string): CompanyProfileShort => {
        const cp = list_CP.value.find((cp_el) => cp_el.code == code);
        return cp ? cp : new CompanyProfileShort();
    }
    const getAddressBook = (companyCode: string, code: string): CompanyProfileAddressBookShort => {
        const cp = list_CP.value.find((cp_el) => cp_el.code == companyCode);
        const ab = cp ? cp.addressBooks.find((ab_el) => ab_el.code == code) : new CompanyProfileAddressBookShort();
        return ab ? ab : new CompanyProfileAddressBookShort();
    }
    const getAddress = (companyCode: string, addressBookCode: string, code: string): CompanyProfileAddressContact => {
        const cp = list_CP.value.find((cp_el) => cp_el.code == companyCode);
        const ab = cp ? cp.addressBooks.find((ab_el) => ab_el.code == addressBookCode) : new CompanyProfileAddressBook();
        const ac = ab ? ab.addressContacts.find((ac_el) => ac_el.code == code) : new CompanyProfileAddressContact();
        return ac ? ac : new CompanyProfileAddressContact();
    }

    // Company Profile
    const toggleCompany = async () => {
        waitModal.value.on = true;
        loadingList.value = true
        await store.dispatch(ActionTypes.CP_TOGGLE_COMPANY, { code: currentCompany.value.code })
            .then((result) => { updateCompanyOnTree(result) })
            .catch(() => { closeModal(waitModal.value); loadingList.value = false })
        loadingList.value = false;
        closeModal(waitModal.value);
    }
    const updateCompany = async (newCompany: CompanyProfileShort | null = null) => {
        waitModal.value.on = true;
        loadingList.value = true
        await store.dispatch(ActionTypes.CP_UPDATE_COMPANY, { companyProfile: newCompany ? newCompany : currentCompany.value })
            .then((result) => updateCompanyOnTree(result))
            .catch(() => { closeModal(waitModal.value); loadingList.value = false })
        loadingList.value = false;
        closeModal(waitModal.value);
    }
    const updateCompanyOnTree = (updatedCompany: CompanyProfileShort) => {
        currentCompany.value = updatedCompany;
        companyTreeNodes.value.every((node) => {
            if (node.data.code == updatedCompany.code) {
                node.style = updatedCompany.status == 0 ? 'color: #ccc' : '';
                node.icon = 'las la-home' + (updatedCompany.status == 1 ? '' : ' deactivated');
                node.label = updatedCompany.name;
                return false;
            } return true;
        })
    }
    const addNewCompany = () => {
        addCompanyProfileModal.value.on = true;
    }
    const addNewCompanyExecute = async (company: CompanyProfileShort, addNext: boolean) => {
        if (!addNext) closeModal(addCompanyProfileModal.value);
        await updateCompany(company);
        reloadList();
    }

    // Address Book
    const canSaveAddressBook = () => {
        return !currentAddressBook.value.address1 && !currentAddressBook.value.address2 && !currentAddressBook.value.address3 && !currentAddressBook.value.address4;
    }
    const toggleAddressBook = async () => {
        waitModal.value.on = true;
        loadingList.value = true
        await store.dispatch(ActionTypes.CP_TOGGLE_ADDRESS_BOOK, { code: currentAddressBook.value.code })
                   .then((result) => { updateAddressBookOnTree(result); })
                   .catch(() => { closeModal(waitModal.value); loadingList.value = false })
        loadingList.value = false
        closeModal(waitModal.value);
    }
    const updateAddressBook = async (newAddressBook: CompanyProfileAddressBookShort | null = null) => {
        waitModal.value.on = true;
        loadingList.value = true;
        if (status) currentAddressBook.value.status = currentAddressBook.value.status == 1 ? 0 : 1;
        await store.dispatch(ActionTypes.CP_UPDATE_ADDRESS_BOOK, { addressBook: newAddressBook ? newAddressBook : currentAddressBook.value })
                   .then((result) => { newAddressBook ? addNewAddressBookToTree(result) : updateAddressBookOnTree(result) })
                   .catch(() => { closeModal(waitModal.value); loadingList.value = false })
        loadingList.value = false;
        closeModal(waitModal.value);
    }
    const updateAddressBookOnTree = (updatedAddressBook: CompanyProfileAddressBookShort) => {
        var go = false;
        currentAddressBook.value = updatedAddressBook
        companyTreeNodes.value.every((node) => {
            if (node.children && node.children.length > 0) {
                node.children.every((abnode) => {
                    if (abnode.data.code == updatedAddressBook.code) {
                        abnode.style = updatedAddressBook.status == 0 ? 'color: #ccc' : '';
                        abnode.icon = 'las la-address-book' + (updatedAddressBook.status == 1 ? '' : ' deactivated');
                        abnode.label = updatedAddressBook.code;
                        go = false; return false;
                    } return true;
                })
            } if (!go) return true;
        })
    }
    const addNewAddressBookToTree = (newAddressBook: CompanyProfileAddressBookShort) => {
        companyTreeNodes.value.every((node) => {
            if (node.data.code == newAddressBook.companyCode) {
                var keys: Array<number> = [];
                node.children?.forEach((nch) => { keys.push(parseInt((nch.key.split('-'))[1])) });
                const index = keys.length > 0 ? (Math.max(...keys) + 1) : 0;
                node.children?.push({ key: node.key + '-' + index, label: newAddressBook.code, icon: 'las la-address-book', type: 'addressBook', data: { cpCode: newAddressBook.companyCode, code: newAddressBook.code }, style: '', children: [] });
                return false;
            } return true;
        })
    }
    const addNewAddressBook = () => {
        addAddressBookModal.value.customProps = currentCompany.value.code;
        addAddressBookModal.value.on = true;
    }
    const addNewAddressBookExecute = async (addressBook: CompanyProfileAddressBookShort, addNext: boolean) => {
        if (!addNext) closeModal(addAddressBookModal.value);
        await updateAddressBook(addressBook);
    }

    // Adres Contact
    const toggleAddress = async () => {
        waitModal.value.on = true;
        loadingList.value = true
        await store.dispatch(ActionTypes.CP_TOGGLE_ADDRESS_CONTACT, { companyCode: getCompanyProfileByAb(currentAddressContact.value.addressCode), code: currentAddressContact.value.code })
                   .then((result) => { updateAddressOnTree(result) })
                   .catch(() => { closeModal(waitModal.value); loadingList.value = false })
        loadingList.value = false
        closeModal(waitModal.value);
    }
    const updateAddressContact = async (newAddress: CompanyProfileAddressContact | null = null) => {
        waitModal.value.on = true;
        loadingList.value = true;
        const cCode = getCompanyProfileByAb(newAddress ? newAddress.addressCode : currentAddressContact.value.addressCode); 
        if (status) currentAddressContact.value.status = currentAddressContact.value.status == 1 ? 0 : 1;
        await (store.dispatch(ActionTypes.CP_UPDATE_ADDRESS_CONTACT, { companyCode: cCode, addressContact: newAddress ? newAddress : currentAddressContact.value }))
                    .then((result) => { newAddress ? addNewAddressToTree(result) : updateAddressOnTree(result) })
                    .catch(() => { closeModal(waitModal.value); loadingList.value = false })
        loadingList.value = false
        closeModal(waitModal.value);
    }

    const updateAddressOnTree = (updatedAddress: CompanyProfileAddressContact) => {
        var go = false;
        currentAddressContact.value = updatedAddress;
        companyTreeNodes.value.every((cp) => {
            if (cp.children && cp.children.length > 0) {
                cp.children.every((ab) => {
                    if (ab.children && ab.children.length > 0) {
                        ab.children.every((a) => {
                            if (a.data.code == updatedAddress.code) {
                                a.style = updatedAddress.status == 0 ? 'color: #ccc' : '';
                                a.icon = 'las la-address-card' + (updatedAddress.status == 1 ? '' : ' deactivated');
                                a.label = updatedAddress.name;
                                go = true; return false;
                            } return true;
                        })
                    } if(!go) return true;
                })
            } if(!go) return true;
        })
    }
    const addNewAddressToTree = (newAddress: CompanyProfileAddressContact) => {
        companyTreeNodes.value.every((cp) => {
            if (cp.children && cp.children.length > 0) {
                cp.children.every((ab) => { 
                    if (ab.data.code == newAddress.addressCode) {
                        var keys: Array<number> = [];
                        ab.children?.forEach((a) => { keys.push(parseInt((a.key.split('-'))[2])) });
                        const index = keys.length > 0 ? (Math.max(...keys) + 1) : 0;
                        ab.children?.push({ key: ab.key + '-' + index, label: newAddress.name, icon: 'las la-address-card', type: 'addressContact', data: { cpCode: cp.data.code, abCode: newAddress.addressCode, code: newAddress.code }, style: '', children: [] });
                        return false;
                    } return true;
                })
            } return true;
        })
    }

    const addNewAddress = () => {
        addAddressContactModal.value.on = true;
        addAddressContactModal.value.customProps = currentAddressBook.value.code;
    }
    const addNewAddressExecute = async (addressContact: CompanyProfileAddressContact, addNext: boolean) => {
        if (!addNext) closeModal(addAddressContactModal.value);
        addressContact.addressCode = currentAddressBook.value.code;
        await updateAddressContact(addressContact);
    }

    const getCompanyProfileByAb = (addressBookCode: string) => {
        var ab = new CompanyProfileAddressBook();
        list_CP.value.every((cp) => {
            cp.addressBooks.every((_ab) => {
                if (_ab.code == addressBookCode) { ab = _ab; return false; } return true;
            }); return true;
        })
        const company = list_CP.value.find((cp) => cp.code == ab.companyCode);
        return company ? company.code : '';
    }

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const waitModal = ref(new Modal(500, t('generic.processing'), false));
    const addCompanyProfileModal = ref(new Modal(700, t('companyProfile.addCompanyProfile'), true));
    const addAddressBookModal = ref(new Modal(750, t('companyProfile.addAddressBook'), true));
    const addAddressContactModal = ref(new Modal(700, t('companyProfile.addAddressContact'), true));
    
    // Menu Action
    const menuAction = (menuFunction: Function) => { menuFunction() };

    // get customers if there are no any in the store yet
    if (store.state.customers.length == 0) {
        await store.dispatch(ActionTypes.INV_LOAD_CUSTOMERS);
        const customers = computed(() => store.state.customers);
        store.state.customers.filter(c => c.code != null).forEach(c => customersDict[c.code!] = c.name);
    }

    // section Menu
    const menuItems_IR:Array<MenuItem> = [
        { state:null, stateParams:null, title:'addNew',  function:addNewCompany, icon:'la-plus-circle' },
        { state:null, stateParams:null, title:'refresh', function:reloadList, icon:'la-redo-alt', float: 'end' },
    ];

    // init the screen
    reloadList();
    onActivated(() => bus.on("refresh", reloadList))
    onDeactivated(() => bus.off("refresh", reloadList))
</script>
<template>
    <section-menu :menu-items="menuItems_IR" t_orig="inboundReversal.operation" @action="menuAction" />
    <div class="container-fluid">
        <div class="row my-3">
            <div class="col-md-4">
                <div class="card" style="height: calc(100vh - 250px); overflow:scroll">
                    <div class="card-body">
                        <div v-if="loadingList && companyTreeNodes.length == 0" class="bars1-wrapper">
                            <div id="bars1">
                                <span></span>
                                <span></span>
                                <span></span>
                                <span></span>
                                <span></span>
                            </div>
                            <h5>{{t('generic.loading')}}</h5>
                        </div>
                        <Tree v-if="!loadingList || companyTreeNodes.length > 0" v-model:selectionKeys="selectedKey" 
                              :value="companyTreeNodes" @nodeSelect="showNode" selectionMode="single" :filter="true" 
                              filterMode="lenient" class="w-full md:w-[30rem]">
                        </Tree>
                    </div>
                </div>
            </div>
            <div class="col-md-5">
                <div v-if="showCompanyCard">
                    <div class="card">
                        <div class="card-header">
                            <strong>{{t('companyProfile.companyProfile')}}</strong>
                        </div>
                        <div class="card-body">
                            <div class="row">
                                <div class="col-md-4 text-end pt-1">{{t('companyProfile.companyCode')}}</div>
                                <div class="col-md-8">
                                    <input type="text" class="form-control form-control-sm" v-model="currentCompany.code" disabled />
                                </div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-4 text-end pt-1">{{t('companyProfile.companyName')}}</div>
                                <div class="col-md-8">
                                    <input type="text" class="form-control form-control-sm" v-model="currentCompany.name" :disabled="currentCompany.status == 0" :maxlength="50" />
                                </div>
                            </div>
                        </div>
                        <div class="card-footer text-end">
                            <button class="btn btn-sm btn-primary me-2" v-if="currentCompany.status == 1" :disabled="loadingList" @click="addNewAddressBook()"><i class="las la-plus-circle"></i> {{t('companyProfile.newAddressBook')}}</button>
                            <button class="btn btn-sm btn-primary me-2" @click="toggleCompany()" :disabled="loadingList"><i :class="['las',currentCompany.status == 1 ? 'la-ban' : 'la-redo-alt']"></i> {{currentCompany.status == 1 ? t('operation.general.deactivate') : t('operation.general.reactivate')}}</button>
                            <button class="btn btn-sm btn-primary" v-if="currentCompany.status == 1" :disabled="loadingList || !currentCompany.name" @click="updateCompany()"><i class="las la-save"></i> {{t('operation.general.save')}}</button>
                        </div>
                    </div>
                </div>

                <div v-if="showAddresBookCard">
                    <div class="card">
                        <div class="card-header">
                            <strong>{{t('companyProfile.addressBook')}}</strong>
                        </div>
                        <div class="card-body">
                            <div class="row">
                                <div class="col-md-6 no-right-padding pt-1">{{t('companyProfile.addressBookCode')}}: <strong>{{currentAddressBook.code}}</strong></div>
                                <div class="col-md-6 no-right-padding pt-1">{{t('companyProfile.companyCode')}}: <strong>{{currentAddressBook.companyCode}}</strong></div>
                            </div>
                            <div class="row mt-3">
                                <div class="col-md-2 no-right-padding text-end pt-1">{{t('companyProfile.address')}}</div>
                                <div class="col-md-10">
                                    <input type="text" class="form-control form-control-sm" v-model="currentAddressBook.address1" :disabled="currentAddressBook.status == 0" :maxlength="50" />
                                </div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-2 no-right-padding text-end pt-1"></div>
                                <div class="col-md-10">
                                    <input type="text" class="form-control form-control-sm" v-model="currentAddressBook.address2" :disabled="currentAddressBook.status == 0" :maxlength="50" />
                                </div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-2 no-right-padding text-end pt-1"></div>
                                <div class="col-md-10">
                                    <input type="text" class="form-control form-control-sm" v-model="currentAddressBook.address3" :disabled="currentAddressBook.status == 0" :maxlength="50" />
                                </div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-2 no-right-padding text-end pt-1"></div>
                                <div class="col-md-10">
                                    <input type="text" class="form-control form-control-sm" v-model="currentAddressBook.address4" :disabled="currentAddressBook.status == 0" :maxlength="50" />
                                </div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-2 no-right-padding text-end pt-1">{{t('companyProfile.postalCode')}}</div>
                                <div class="col-md-2">
                                    <input type="text" class="form-control form-control-sm" v-model="currentAddressBook.postCode" :disabled="currentAddressBook.status == 0" :maxlength="6" />
                                </div>
                                <div class="col-md-2 text-end pt-1">{{t('companyProfile.country')}}</div>
                                <div class="col-md-6 no-left-padding multiselect-as-select">
                                    <select-country v-model="currentAddressBook.country" :disabled="currentAddressBook.status == 0"></select-country>
                                </div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-2 no-right-padding text-end pt-1">{{t('companyProfile.telNo')}}</div>
                                <div class="col-md-4">
                                    <input type="text" class="form-control form-control-sm" v-model="currentAddressBook.telNo" :disabled="currentAddressBook.status == 0" :maxlength="20" />
                                </div>
                                <div class="col-md-2 no-right-padding text-end pt-1">{{t('companyProfile.faxNo')}}</div>
                                <div class="col-md-4">
                                    <input type="text" class="form-control form-control-sm" v-model="currentAddressBook.faxNo" :disabled="currentAddressBook.status == 0" :maxlength="20" />
                                </div>
                            </div>
                        </div>
                        <div class="card-footer text-end">
                            <button class="btn btn-sm btn-primary me-2" v-if="currentAddressBook.status == 1" @click="addNewAddress"><i class="las la-plus-circle"></i> {{t('companyProfile.newContact')}}</button>
                            <button class="btn btn-sm btn-primary me-2" @click="toggleAddressBook()"><i :class="['las',currentAddressBook.status == 1 ? 'la-ban' : 'la-redo-alt']"></i> {{currentAddressBook.status == 1 ? t('operation.general.deactivate') : t('operation.general.reactivate')}}</button>
                            <button class="btn btn-sm btn-primary" v-if="currentAddressBook.status == 1" :disabled="loadingList || canSaveAddressBook()" @click="updateAddressBook()"><i class="las la-save"></i> {{t('operation.general.save')}}</button>
                        </div>
                    </div>
                </div>

                <div v-if="showAddressCard">
                    <div class="card">
                        <div class="card-header">
                            <strong>{{t('companyProfile.contact')}}</strong>
                        </div>
                        <div class="card-body">
                            <div class="row">
                                <div class="col-md-5 pt-1">{{t('companyProfile.contactCode')}}: <strong>{{ currentAddressContact.code }}</strong></div>
                                <div class="col-md-7 pt-1">{{t('companyProfile.addressBookCode')}}: <strong>{{ currentAddressContact.addressCode }}</strong></div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-4 no-right-padding text-end pt-1">{{t('companyProfile.picName')}}</div>
                                <div class="col-md-8">
                                    <input type="text" class="form-control form-control-sm" v-model="currentAddressContact.name" :disabled="currentAddressContact.status == 0" :maxlength="30" />
                                </div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-4 no-right-padding text-end pt-1">{{t('companyProfile.telNo')}}</div>
                                <div class="col-md-8">
                                    <input type="text" class="form-control form-control-sm" v-model="currentAddressContact.telNo" :disabled="currentAddressContact.status == 0" :maxlength="20" />
                                </div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-4 no-right-padding text-end pt-1">{{t('companyProfile.faxNo')}}</div>
                                <div class="col-md-8">
                                    <input type="text" class="form-control form-control-sm" v-model="currentAddressContact.faxNo" :disabled="currentAddressContact.status == 0" :maxlength="20" />
                                </div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-4 no-right-padding text-end pt-1">{{t('companyProfile.email')}}</div>
                                <div class="col-md-8">
                                    <input type="text" class="form-control form-control-sm" v-model="currentAddressContact.email" :disabled="currentAddressContact.status == 0" :maxlength="50" />
                                </div>
                            </div>
                        </div>
                        <div class="card-footer text-end">
                            <button class="btn btn-sm btn-primary me-2" @click="toggleAddress()"><i :class="['las',currentAddressContact.status == 1 ? 'la-ban' : 'la-redo-alt']"></i> {{currentAddressContact.status == 1 ? t('operation.general.deactivate') : t('operation.general.reactivate')}}</button>
                            <button class="btn btn-sm btn-primary" v-if="currentAddressContact.status == 1" @click="updateAddressContact()" :disabled="!currentAddressContact.name"><i class="las la-save"></i> {{t('operation.general.save')}}</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <processing-popup :modalBase=waitModal />
    <add-company-profile-popup :modalBase="addCompanyProfileModal" @proceed="addNewCompanyExecute" @close="closeModal(addCompanyProfileModal)" />
    <add-address-book-popup :modalBase="addAddressBookModal" @close="closeModal(addAddressBookModal)" @proceed="addNewAddressBookExecute" />
    <add-address-contact-popup :modalBase="addAddressContactModal" @close="closeModal(addAddressContactModal)" @proceed="addNewAddressExecute" />
</template>