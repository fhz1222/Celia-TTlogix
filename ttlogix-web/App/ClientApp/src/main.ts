document.title = 'TT-Logix'

import 'bootstrap/scss/bootstrap.scss'
import 'bootstrap/dist/js/bootstrap.js'
import 'floating-vue/dist/style.css'
import 'animate.css/animate.min.css'
import 'vue2-animate/dist/vue2-animate.min.css'
import "@babel/polyfill"
import "@babel/helpers"
import { createApp, h } from 'vue';
import './assets/style/app.scss'
import './assets/line-awesome-1.3.0/css/line-awesome.css'
import axiosInst from './axios-instance'
import App from './App.vue'
import DataProvider from './service/data_provider.js'


const app = createApp({
    render: () => h(App),
});

import EventBus from './service/bus.js'
app.config.globalProperties.$bus = EventBus

import auth from './auth'
router.beforeEach((to:any, _:any, next:any) => {
    const granted = to.matched.map((route:any) => {
        const r = route.meta.accessRight
        if (!r) return true
        if (Array.isArray(r)) return r.map(rr => auth.check(rr)).reduce((acc, v) => acc && v)
        return auth.check(r)
    }).reduce((acc:any, v:any) => acc && v)
    next(granted)
})
app.use(auth);

import router from './router'
app.use(router);

import PrimeVue from 'primevue/config';
import Aura from '@primevue/themes/aura';

app.use(PrimeVue, {
    theme: {
        preset: Aura,
        options: {
            prefix: 'p',
            darkModeSelector: 'system'
        }
    }
});

import Notifications from '@kyvg/vue3-notification'
app.use(Notifications)


import { createI18n } from 'vue-i18n'
const numberFormats = {
    'en': {
        currency: {
            style: 'currency', currency: 'EUR', currencyDisplay: 'symbol'
        },
        simple: {
            style: 'decimal',
            minimumFractionDigits: 0
        }
    }
};
let locale = 'en';
if (typeof (Storage) !== "undefined") {
    locale = window.localStorage.getItem('lang') || locale
}
const i18n = createI18n({
    legacy: false,
    locale: locale,
    fallbackLocale: 'en',
    messages: {
        en: require('./i18n/en.json'),
        pl: require('./i18n/pl.json'),
        it: require('./i18n/it.json'),
        hu: require('./i18n/hu.json'),
        de: require('./i18n/de.json'),
    },
    numberFormats: numberFormats,
    globalInjection: true
});
app.use(i18n);
app.use(DataProvider, { axios: axiosInst.$axios, i18n: i18n })


import DatePicker from 'vue-datepicker-next';
import 'vue-datepicker-next/index.css';
app.component('datepicker', DatePicker)

import FloatingVue from 'floating-vue'
app.use(FloatingVue, { distance: 10, strategy: 'fixed', disposeTimeout: 1000000 })
//@ts-ignore
app.directive('tooltip', FloatingVue.VTooltip)
//@ts-ignore
app.component('VTooltip', FloatingVue.Tooltip)

import Modal from './widgets/Modal.vue'
import Alert from './widgets/Alert.vue'
import Confirm from './widgets/Confirm.vue'
app.component('modal', Modal)
app.component('alert', Alert)
app.component('confirm', Confirm)

//@ts-ignore
import vSelect from 'vue-select'
app.component('v-select', vSelect)

import InfiniteLoading from "v3-infinite-loading";
import "v3-infinite-loading/lib/style.css";
app.component("infinite-loading", InfiniteLoading);

// import RouterTab from 'vue-router-tab'
// Vue.use(RouterTab)


// The class .non-arrow-operated is meant for marking the input and textarea fields
// that shouldn't be accessible by navigating the arrow keys
window.addEventListener("keydown", (evt) => {

    if (evt.key == 'ArrowRight') {
        //@ts-ignore
        const inputs = [...document.querySelectorAll('input:not(.non-arrow-operated),textarea:not(.non-arrow-operated)')].filter(x => !x.disabled && !x.date);
        for (let i = 0; i < inputs.length; i++) {
            evt.preventDefault();
            if (evt.target === inputs[i] && i + 1 < inputs.length) {
                //@ts-ignore
                inputs[i + 1].focus();
                break;
            }
        }
    }
    if (evt.key == 'ArrowLeft') {
        //@ts-ignore
        const inputs = [...document.querySelectorAll('input:not(.non-arrow-operated),textarea:not(.non-arrow-operated)')].filter(x => !x.disabled && !x.date);
        for (let i = 0; i < inputs.length; i++) {
            evt.preventDefault();
            if (evt.target === inputs[i] && i - 1 >= 0) {
                //@ts-ignore
                inputs[i - 1].focus();
                break;
            }
        }
    }
});

import { store } from './store'
app.use(store)

import { ApplicationInsights } from '@microsoft/applicationinsights-web'
var xhr = new XMLHttpRequest();
xhr.open('GET', '/config.json', true);
xhr.responseType = 'json';
xhr.onload = function () {
    var status = xhr.status;
    if (status === 200) {
        const appInsights = new ApplicationInsights({
            config: {
                connectionString: xhr.response.appInsights.connectionString
            }
        });
        appInsights.loadAppInsights();
        appInsights.trackPageView();
    }
    else {
        console.error("Could not find app insight connection string.");
    }
};
xhr.send();

app.mount('#app')

