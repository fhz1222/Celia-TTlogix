<script lang="ts" setup>
    import auth from "../auth";
    import { RouterLink, useRouter } from 'vue-router';
    import { inject, ref, watch } from 'vue';
    import { useI18n } from 'vue-i18n';
    import EventBus from '../service/bus';

    const { t } = useI18n({ useScope: 'global' });
    const router = useRouter();

    const storageGroupsCheck = ref(false);
    const invoicingAvailable = ref(false);
    const userName = ref('');
    const showMenuForRoute = ref(true);

    watch(router.currentRoute, async (newrt, oldrt) => {
        showMenuForRoute.value = <boolean>newrt.meta.showMenu;
    });
    
    const dp = (inject('DP') as any)?.invoicing;
    const setMenu = () => {
        auth.fetch().then(() => {
            userName.value = auth.user() ? auth.user().firstName + ' ' + auth.user().lastName + ' (' + auth.user().code + ')' : '';
            storageGroupsCheck.value = auth.user() && !process.env.VUE_APP_DISABLE_STORAGE_GROUPS;
            dp?.isActive().then((x: any) => invoicingAvailable.value = x);
        }).catch(() => {
            userName.value = '';
            storageGroupsCheck.value = false;
        });
    };
    const logout = () => { auth.logout({ redirect: '/login' }); setMenu(); };

    EventBus.$on('app.authorized', setMenu);
    setMenu();
</script>

<template>
    <div class="collapse navbar-collapse nav-buttons text-end" id="navbarsExample06">
        <ul class="navbar-nav ms-auto mb-2 mb-lg-0">
            <li class="nav-item" v-if="auth.user() && showMenuForRoute">
                <router-link :to="{ name: 'partsmaster' }">Parts Master</router-link>
            </li>
            <li class="nav-item" v-if="auth.check('INVENTORY') && showMenuForRoute">
                <router-link :to="{ name: 'inventory' }">Inventory</router-link>
            </li>
            <!--li class="nav-item" v-if="$auth.check('INVOICING')"-->
            <li class="nav-item" v-if="auth.user() && invoicingAvailable">
                <router-link :to="{ name: 'invoicing' }">Invoicing</router-link>
            </li>
            <li class="nav-item" v-if="auth.check('STOCKTRANSFER') && showMenuForRoute">
                <router-link :to="{ name: 'stocktransfer' }">Stock Transfer</router-link>
            </li>
            <li class="nav-item" v-if="auth.check('INBOUND') && showMenuForRoute">
                <router-link :to="{ name: 'inbound' }">Inbound</router-link>
            </li>
            <li class="nav-item" v-if="auth.check('OUTBOUND') && showMenuForRoute">
                <router-link :to="{ name: 'outbound' }">Outbound</router-link>
            </li>
            <li class="nav-item" v-if="auth.check('LOADING') && showMenuForRoute">
                <router-link :to="{ name: 'loading' }">Loading</router-link>
            </li>
            <li class="nav-item" v-if="auth.check('USER') && auth.check('GROUP') && showMenuForRoute">
                <router-link :to="{ name: 'usermanagement' }">Users</router-link>
            </li>
            <li class="nav-item" v-if="storageGroupsCheck && showMenuForRoute">
                <router-link :to="{ name: 'storage-group' }">Storage Groups</router-link>
            </li>
            <li class="nav-item" v-if="auth.check('COMPANYPROFILE') && showMenuForRoute">
                <router-link :to="{ name: 'settings' }">Settings</router-link>
            </li>
            <li class="nav-item" v-if="showMenuForRoute">
                <router-link :to="{ name: 'stock-take' }">Stock Take</router-link>
            </li>
            <li class="nav-item top-link-no-bg" v-if="auth.user()">
                <router-link :to="{ name: 'home' }" class="name" v-if="showMenuForRoute">
                    <i class="las la-user"></i> {{ userName }}
                </router-link>
                <div class="text-light me-3 pt-1" v-if="!showMenuForRoute"><i class="las la-user"></i> {{ userName }}</div>
            </li>
            <li class="nav-item" v-if="auth.user()">
                <button class="btn btn-sm btn-outline-light" @click="logout()">
                    <i class="las la-sign-out-alt"></i> {{ t('user.logout') }}
                </button>
            </li>
        </ul>
    </div>
</template>