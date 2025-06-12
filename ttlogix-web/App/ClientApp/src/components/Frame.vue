<template>
    <div class="router-tab">
        <header class="router-tab__header">
            <div class="router-tab__scroll">
                <div class="router-tab__scroll-container">
                    <ul class="router-tab__nav">
                        <router-link :to='t.path' v-for="t, i in tabs" :key="i" custom
                            v-slot="{ navigate, isExactActive }">
                            <li class="router-tab__item" @click.stop="navigate" role="link"
                                :class="isExactActive ? 'is-active' : ''">
                                <span class="router-tab__item-title ">{{ t.title }}</span>
                                <i class="router-tab__item-close" @click.stop="remove(i)" />
                            </li>
                        </router-link>
                    </ul>
                </div>
            </div>
        </header>
        <div class="router-tab__container">
            <!-- <button @click="remove()">remove</button>
            <p>{{key}}</p> -->
            <router-view v-slot="{ Component }">
                <!-- <transition :name="route.meta.transition || 'fade'" mode="out-in"> -->
                <keep-alive :max="200">
                    <suspense>
                        <template #default>
                            <component :is="Component" :key="key" />
                        </template>
                        <template #fallback> Loading... </template>
                    </suspense>
                </keep-alive>
                <!-- </transition> -->
            </router-view>
        </div>
    </div>

</template>
<script lang="ts" setup>
//@ts-ignore
import { triggerRouteExit } from "@/route-exit-jobs";
import { enable, disable } from "@/service/refresh_behaviour"
import { onUnmounted, ref, watch } from 'vue';
import { onBeforeRouteLeave, useRoute, useRouter } from "vue-router";

class TabData {
    title: string
    path: string
    id: number
    get key() {return `${this.path}${this.id}`}
    constructor(title: string, path: string, id: number){
        this.title = title;
        this.path = path;
        this.id = id;
    }
}

const route = useRoute();
const tabsHistory = ref<Array<string>>([])
const router = useRouter();
const generateCurrentTabData = (): TabData => {
    const title = typeof route.meta.title == 'function' ? route.meta.title(route) : route.meta.title;
    return new TabData( title, route.path, getNewID());
}

const n = ref(0);
const getNewID = () => {
    n.value += 1;
    return n.value;
}

const tabs = ref<Array<TabData>>([]);

const currentTabData = generateCurrentTabData()
const key = ref(currentTabData.key)
tabs.value.push(currentTabData);

enable();
onUnmounted(() => {
    disable();
})
const remove = async (i: number) => {
    const changeTab = tabs.value[i].path == route.path;
    await triggerRouteExit(tabs.value[i].path);
    tabs.value.splice(i, 1)
    tabsHistory.value.pop()
    if (changeTab) {
        if (tabsHistory.value.length > 0) {
            router.push(tabsHistory.value[tabsHistory.value.length - 1])
        }
        else {
            router.push("/");
        }
    }
}
watch(route, () => {
    const v = tabs.value.find(x => x.path == route.path);
    if (!v) {
        const tabData = generateCurrentTabData()
        tabs.value.push(tabData);
        key.value = tabData.key
    }
    else {
        key.value = v.key
    }
})
router.beforeEach((to, from) => {
    const i = tabsHistory.value.indexOf(to.path);
    if (i >= 0)         
        tabsHistory.value.splice(i, 1);
    tabsHistory.value.push(to.path);
})
</script>
<style scoped lang="scss">
@import '../assets/style/vue-router-tab/routerTab.scss'
</style>