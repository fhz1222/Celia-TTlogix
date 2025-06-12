<template>
    <!-- Page header -->
    <div class="container-fluid">
        <div class="row">
            <nav class="navbar navbar-expand-xl navbar-dark bg-dark position-relative" aria-label="Sixth navbar example">
                <!-- Home Link -->
                <h2 class="float-start brand-header-link">
                    <router-link :to="{ name: 'home' }">
                        TT-Logix <span v-if="$auth.user()"> / {{ $auth.user().whsCode }}</span>
                    </router-link>
                </h2>
                <!-- Menu hamburger -->
                <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarsExample06"
                        aria-controls="navbarsExample06" aria-expanded="false" aria-label="Toggle navigation">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <!-- Main menu -->
                <main-menu />
            </nav>
        </div>
    </div>

    <!-- Content -->
    <div id="content">
        <div class="loader-wrapper">
            <div class="wrap">
                <loading v-show="load > 0" class="loader" color="#ffffff" />
            </div>
        </div>
        <router-view></router-view>
    </div>

    <footer>
        <div class="container-fluid">
            <div class="row">
                <div class="col-md-6"><p>&copy; Toyota Tsusho Europe</p></div>
                <div class="col-md-6 text-end">{{ version }}-{{ commit.substring(0, 8) }}</div>
            </div>
        </div>
    </footer>

    <notifications group="temporary"  position="bottom center" width="40%" :duration="3000"/>
    <notifications group="permanent" position="bottom center" width="40%" :duration="-1">
        <template v-slot:body="{ item, close }">
            <div class="vue-notification-template vue-notification clickable" :class="item.type"
                @click.stop.prevent="close">
                <div class="row">
                    <div class="col-md-11">
                        <div class="notification-title">{{ item.title }}</div>
                        <div class="notification-content">{{ item.text }}</div>
                    </div>
                    <div class="col-md-1 text-end d-flex align-items-center">
                        <i class="las la-window-close error-close-x"></i>
                    </div>
                </div>
            </div>
            <div class="click-to-close mb-2">
                <button class="btn btn-danger" @click.stop.prevent="closeAllNotifications">
                    <i class="las la-window-close"></i>&nbsp; ({{ $t('notifications.clickToClose') }})
                </button>
            </div>
        </template>
    </notifications>

    <modal v-if="!online" containerClass="modal-reconnect">
        <template name="body" v-slot:body>
            {{ $t('app.reconnecting') }}
            <loading class="loader" color="#2C5A81" />
        </template>
        <template name="footer" v-slot:footer>
            <button class="btn btn-sm" @click="logout(); online = true">
                <i class="las la-sign-out-alt"></i> {{ $t('user.logout') }}
            </button>
        </template>
    </modal>

</template>

<script>
import Loading from './widgets/Loading.vue'
import { defineComponent } from 'vue';
import { useStore } from '@/store';
import { MutationTypes } from '@/store/mutation-types';
    import version from '@/app_version'
    import MainMenu from '@/components/MainMenu.vue';
export default defineComponent({
    name: 'App',
    mounted() {
        document.title = 'TT-Logix';
        this.$bus.$on('lang', this.setLang);
        this.version = version;
        this.$auth.fetch().then(() => {
            useStore().commit(MutationTypes.SET_WHS, this.$auth?.user()?.whsCode ?? '');
            useStore().commit(MutationTypes.SET_CAN_EDIT_UNLOADING_POINT, this.$auth.user().roles.indexOf("PARTMASTERUNLOADING") > -1 ? true : false);
        });
    },
    props: {
        languages: {
            default: function () {
                return {
                    en: 'English',
                    de: 'German',
                    pl: 'Polish',
                    hu: 'Hungarian',
                    it: 'Italian'
                }
            }
        }
    },
    components: {
        Loading, MainMenu
    },
    data() {
        return {
            load: 0,
            online: true,
            commit: process.env.VUE_APP_COMMIT_HASH,
            version: "",
            env: process.env
        }
    },
    methods: {
        onLoad(state) {
            this.load += state ? 1 : -1;
        },
        logout() {
            this.$auth.logout({
                redirect: '/login'
            });
        },
        setLang(lang) {
            this.$i18n.locale = lang
            this.$dataProvider.setAccpetLang(lang)
            if (typeof (Storage) !== "undefined") {
                window.localStorage.setItem('lang', lang)
            }
        },
        onOnline() {
            this.online = true
        },
        onOffline() {
            this.online = false
        },
        closeAllNotifications() {
            this.$notify({ clean: true, group: 'permanent' });
        }
    },
    computed: {
        devMode() {
            return process.env.NODE_ENV != 'production'
        }
    },
    created() {
        this.$dataProvider.setAccpetLang(this.$i18n.locale)
        this.$bus.$on('load', this.onLoad)
        this.$bus.$on('app.offline', this.onOffline)
        this.$bus.$on('app.online', this.onOnline)
        this.$bus.$emit('app.start')
    },
    beforeUnmount() {
        this.$bus.$off('load', this.onLoad)
        this.$bus.$off('app.offline')
        this.$bus.$off('app.online')
        this.$bus.$off('lang', this.setLang)
        this.$bus.$emit('app.stop')
    }
})
</script>
<style lang="scss">
.modal-reconnect {
    .modal-container {
        width: 200px;
    }
}

.vue-notification-wrapper:last-child {
    .click-to-close {
        display: block;
    }
}

.clickable {
    cursor: pointer;
}

.click-to-close {
    .background {
        background-color: #00000060;
        padding: 5px;
    }

    padding: 0px 5px;
    display: none;
    width: 100%;
    text-align: center;
    color: #ffffff;
}
</style>