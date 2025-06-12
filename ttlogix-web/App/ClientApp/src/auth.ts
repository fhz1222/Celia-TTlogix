
import axiosInst from './axios-instance'
//@ts-ignore
import { createAuth } from '@websanova/vue-auth';
import authBearer from './service/bearer.js';
//@ts-ignore
import httpAxios from '@websanova/vue-auth/dist/drivers/http/axios.1.x.esm.js';
//@ts-ignore
import routerVueRouter from '@websanova/vue-auth/dist/drivers/router/vue-router.2.x.esm.js';
import router from './router';
const pathname = window.location.pathname;

const auth = createAuth({
    plugins: {
        http: axiosInst.$axios,
        router: router
    },
    drivers: {
        http: httpAxios,
        auth: authBearer,
        router: routerVueRouter
    },
    options: {
        storage: ['storage', 'cookie'],
        rolesKey: 'roles',
        tokenDefaultKey: 'token' + pathname.replaceAll('/', '_'),
        cookie: {
            path: pathname
        },
        logoutData: { url: 'auth/logout', method: 'POST', redirect: '/', makeRequest: false },
        loginData: { url: 'auth', method: 'POST', redirect: '/', staySigned: false, fetchUser: true },
        fetchData: { url: 'auth', method: 'GET', redirect: '/', staySigned: false, fetchUser: true },
        refreshData: { enabled: false },
        parseUserData: function (response: any) {
            return response
        }
    }
});

export default auth