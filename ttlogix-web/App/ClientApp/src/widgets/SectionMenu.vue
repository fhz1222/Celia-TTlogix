<script lang="ts" setup>
    import { MenuItem } from '@/store/commonModels/config';
    import * as routerFn from '@/router';
    import { useI18n } from 'vue-i18n';

    const props = defineProps<{ menuItems: MenuItem[], t_orig: string, top?:boolean, background?:number }>();
    const emit = defineEmits<{(e: 'action', action: Function): void }>();
    // GoTo
    const goTo = (state: string, params?:any) => { return routerFn.goTo(state, params); }
    const { t } = useI18n({ useScope: 'global' });
    const background = () => {
        return props.background && props.background == 0 ? 'toolbar-new' : 'toolbar-new-grey';
    }
</script>

<template>
    <div :class="background()">
        <div class="row pt-2 pb-2">
            <div class="col-md-12 ps-4 pe-4 main-action-icons-small">
                <div v-for="item, i in props.menuItems" :key="i" class="d-inline-block link-box" :class="{'me-5': i < props.menuItems.length-1, 'float-end': 'float' in item && item.float == 'end'}">
                    <a v-if="item.state" href="#" @click.stop.prevent="goTo(item.state ?? '')" class="text-center">
                        <div><i class="las" :class="item.icon"></i></div>
                        <div>{{t(t_orig + '.' + (item.state ?? item.title))}}</div>
                    </a>
                    <a v-if="!item.state" href="#" @click.stop.prevent="emit('action', (item.function ?? function(){}))" class="text-center">
                        <div><i class="las" :class="item.icon"></i></div>
                        <div>{{t(t_orig + '.' + (item.state ?? item.title))}}</div>
                    </a>
                </div>
            </div>
        </div>
    </div>
</template>