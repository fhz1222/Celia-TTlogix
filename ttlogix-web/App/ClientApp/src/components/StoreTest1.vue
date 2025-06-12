<script lang="ts" setup>
import { useStore } from '@/store';
import { ActionTypes } from '@/store/action-types';
import { onMounted, onUnmounted, ref, defineProps} from 'vue';

//local variable
const store = useStore();
//method
const updateList = () => store.dispatch(ActionTypes.INV_UPDATE_LIST);
//data
const dataItem = ref(0);
const dataItem1 = ref<{stuff: string} | null>();

//lifecycle hooks
onMounted(() => {});
onUnmounted(() => {});

//watch
//watch(dataItem, (value, oldValue) => {})

//props
const props = defineProps<{prop1: string}>();

console.log(props.prop1);
</script>
<template>
<button @click="updateList">click me</button>
<p>{{dataItem}}</p>
<p>{{dataItem1?.stuff}}</p>
<p>{{prop1}}</p>
</template>