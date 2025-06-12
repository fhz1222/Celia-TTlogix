<script lang="ts" setup>
    import { PaginationData } from '../store/commonModels/paginationData';
    import { state } from '@/store/state';
    import { ref } from 'vue';
 
    const config = state.config;
    const props = defineProps<{pagingData: PaginationData, padding: boolean}>();
    const emit = defineEmits<{(e: 'goToPage', page: number): void, (e: 'setItemsPerPage', pages: number): void}>();
    const setItemsPerPage = (e:Event) => {
        emit('setItemsPerPage', parseInt((e.target as HTMLInputElement).value));
    }
    let from = () => { return ((props.pagingData.pageNumber - 1) * props.pagingData.itemsPerPage + 1) };
    let to = () => { 
        var calculatedTo = props.pagingData.pageNumber * props.pagingData.itemsPerPage;
        return calculatedTo > props.pagingData.totalCount ? props.pagingData.totalCount : calculatedTo;
    };
    let totalPages = () => { return props.pagingData.totalPages };
    let pageNumber = () => {return props.pagingData.pageNumber };

    var pageToGoTo = ref(1);

    let pagingArray = () => {
		var a = [];
		var offset = totalPages() > 10 ? pageNumber() > 4 ? (pageNumber() - 3) : 0 : 0;
        var end = totalPages() > 10 ? pageNumber() > 4 ? (pageNumber() + 2) : 8 : totalPages(); 

		for (var p = offset; p < end; p++) {
			p < totalPages() ? a.push(p + 1) : null;
		}
		return a;
	}
</script>   

<template> 
    <div class="row" :class="padding ? 'pt-3 mb-4': ''" >
        <div class="col-md-10 ps-4">
            <div class="pagination">
                <div>
                    <div class="paging-items" v-if="totalPages() > 1">

                        <!-- FIRST + PREV -->
                        <div class="p-button">
                            <button class="btn btn-sm btn-primary" 
                                    @click="$emit('goToPage',1)"
                                    :disabled="!props.pagingData.hasPreviousPage">
                                <i class="las la-step-backward"></i>
                            </button>
                        </div>
                        <div class="p-button">
                            <button class="btn btn-sm btn-primary flip-text"
                                    @click="$emit('goToPage', pageNumber() - 1)"
                                    :disabled="!props.pagingData.hasPreviousPage">
                                <i class="las la-play"></i>
                            </button>
                        </div>

                        <!-- PAGES -->
                        <div class="p-numbers ps-2 pe-3">
                            <span v-if="totalPages() > 10 && pageNumber() > 4">
                                <span class="p-number" @click="$emit('goToPage', 1)">1</span>
                                <span>...</span>
                            </span>

                            <span class="p-number"
                                :class="pageNumber() == page ? 'current' : ''" 
                                v-for="page in pagingArray()" :key="page">
                                <a @click="$emit('goToPage', page)">{{page}}</a>
                            </span>

                            <span v-if="totalPages() > 10 && pageNumber() <= (totalPages()-3)">
                                <span v-if="pageNumber() != totalPages() - 3">...</span>
                                <span class="p-number" @click="$emit('goToPage', totalPages())">{{ totalPages() }}</span>
                            </span>
                        </div>

                        <!-- NEXT + LAST -->
                        <div class="p-button">
                            <button class="btn btn-sm btn-primary" 
                                    @click="$emit('goToPage', pageNumber() + 1)"
                                    :disabled="!props.pagingData.hasNextPage">
                                <i class="las la-play"></i>
                            </button>
                        </div>
                        <div class="p-button">
                            <button class="btn btn-sm btn-primary" 
                                    @click="$emit('goToPage', totalPages())"
                                    :disabled="!props.pagingData.hasNextPage">
                                <i class="las la-step-forward"></i>
                            </button>
                        </div>

                        <!-- GO TO PAGE No -->
                        <div class="p-numbers pt-1 ms-2 text-end ms-5">
                            go to page:
                        </div>
                        <div class="p-input ms-2">
                            <input type="number" 
                                @change="() => { if(pageToGoTo > totalPages() || pageToGoTo < 1) { pageToGoTo = 1 }}" 
                                @keyup.enter = "$emit('goToPage', pageToGoTo)" 
                                class="form-control form-control-sm" 
                                v-model="pageToGoTo" />
                        </div>
                        <div class="p-button ms-2">
                            <button class="btn btn-sm btn-primary" @click="$emit('goToPage', pageToGoTo)">GO</button>
                        </div>
                    </div>

                    <!-- ITEMS PER PAGE -->
                    <div class="paging-items" v-if="totalPages() > 1 || (totalPages() == 1 && props.pagingData.totalCount > config.pagination.itemsPerPage[0])">
                        <div class="p-select" :class="totalPages() > 1 ? 'ms-5' : ''">
                            <select class="form-select form-select-sm" :value="pagingData.itemsPerPage" @change="setItemsPerPage($event)">
                                <option v-for="num in config.pagination.itemsPerPage" :key="num" :value="num">{{num}}</option>
                            </select>
                        </div>
                        <div class="p-numbers pt-1 ms-2">
                            items per page
                        </div>
                    </div>

                    <div class="clearfix"></div>

                    <!-- Records info -->
                    <div class="pt-1" v-if="totalPages() > 1 || (totalPages() == 1 && props.pagingData.totalCount > config.pagination.itemsPerPage[0])">
                        Showing records <strong>{{ from() }}</strong> to <strong>{{ to() }}</strong> of <strong>{{ props.pagingData.totalCount }}</strong>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>