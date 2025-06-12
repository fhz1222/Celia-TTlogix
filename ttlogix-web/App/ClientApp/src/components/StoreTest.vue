<script lang="ts" setup>
import { useStore } from '@/store';
import { computed } from 'vue';
import StoreTest1 from '@/components/StoreTest1.vue';

const store = useStore();
//computed
const computedProp = computed(() => store.state.inventoryList);


//store.dispatch(ActionTypes.UPDATE_LIST);

// const dataItem = ref(0);

</script>
<template>
<StoreTest1 :prop1="JSON.stringify(computedProp)"></StoreTest1>
</template>