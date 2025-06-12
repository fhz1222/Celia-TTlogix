<script lang="ts" setup>
    // import vue stuff
    import { store } from '@/store';
    import { useI18n } from 'vue-i18n'
    import moment from 'moment';
    import { ref, watch } from 'vue';

    // import types
    import { TableColumn } from '@/store/commonModels/config';
    import { Sorting } from '@/store/commonModels/Sorting';
    import DatePicker from '@/widgets/DatePicker';

    const { t } = useI18n({ useScope: 'global' });

    const x = store.state.config.images.x;
    const props = defineProps<{ columns: Array<TableColumn>, sorting: Sorting, t_core: string, sortable: boolean, filtreable: boolean }>();
    const emit = defineEmits<{ (e: 'applyFilters', column: string, value: any, load: boolean): void, (e: 'sort', by: string): void, (e: 'sortingClass', field: string): void, (e: 'toggleAll', selected: boolean): void }>();

    const columns = ref(props.columns);
    const allRowsState = ref(false);

    const specialClasses = (column: TableColumn, date: boolean = false) => {
        if (column.specialClasses) {
            return (date ? 'mx-input ' : '') + column.specialClasses.join(' ');
        }
    }

    const applyFilter = (column: TableColumn, load: boolean = false, action: string = 'apply') => {
        var filter1: any; var filter2: any;

        const resetToDefault = "resetToDefault" in column && column.resetToDefault;

        const resetCol1 = (column: TableColumn) => {
            column.f1 = column.resetTo;
            filter1 = column.columnType == 'date' ? moment(column.resetTo).format('YYYY-MM-DD') : column.resetTo;
        }

        const resetCol2 = (column: TableColumn) => {
            if ("resetF2To" in column && column.resetF2To) {
                column.f2 = column.resetF2To;
                filter2 = column.columnType == 'date' ? moment(column.resetF2To).format('YYYY-MM-DD') : column.resetF2To;
            }
        }

        if (action == 'clear') {
            column.f1 = filter1 = column.columnType == 'range' || column.columnType == 'date' ? null : '';
            column.f2 = filter2 = column.columnType == 'range' || column.columnType == 'date' ? null : '';

            if (column.resetTo)
                resetCol1(column);
            if ("resetF2To" in column && column.resetF2To)
                resetCol2(column);
        } else {
            filter1 = column.columnType == 'date' ? column.f1 ? moment(column.f1).format('yyyy-MM-DD') : null : column.f1;
            filter2 = column.columnType == 'date' ? column.f2 ? moment(column.f2).format('yyyy-MM-DD') : null : column.f2;

            if (!column.f1 && resetToDefault && column.resetTo)
                resetCol1(column);
            if (!column.f2 && resetToDefault && ("resetF2To" in column && column.resetF2To))
                resetCol2(column);
        }
        var filterVal = column.columnType == 'range' || column.columnType == 'date' ? { from: filter1 ? filter1 : null, to: filter2 ? filter2 : null } : filter1;
        var filter = column.alias;
        if ("filterName" in column) filter = column.filterName;
        emit('applyFilters', filter, filterVal, load);
    }

    const toggleAll = () => {
        emit('toggleAll', allRowsState.value);
    }

    const sortBy = (column: TableColumn) => {
        let by = null;
        if ("filterNameForSorting" in column) by = column.filterNameForSorting;
        emit('sort', by ?? column.alias);
    }

    const sortingClass = (column: TableColumn) => {
        return column.alias == props.sorting.by ? props.sorting.descending ? "la-sort-amount-down" : "la-sort-amount-up-alt" : "";
    }

</script>

<template>
    <thead>
        <tr>
            <th v-for="column in columns" :key="column.alias" :class="column.alias">
                <!--span class="sort-header" @click="$emit('sort', column.alias)" v-if="column.sortable"-->
                <span class="sort-header" @click="sortBy(column)" v-if="column.sortable">
                    {{column.alias.length == 0 ? '' : t(t_core + '.' + column.alias)}}
                </span>
                <i class='las ms-2' :class="sortingClass(column)" v-if="column.sortable"></i>
                <span class="non-sort-header" v-if="!column.sortable">{{column.alias.length == 0 ? '' : t(t_core + '.' + column.alias)}}</span>
            </th>
        </tr>
        <tr class="column-filter" v-if="filtreable">
            <th v-for="column in columns" :key="column.alias" :class="column.columnType">
                <div v-if="column.columnType == 'toggleAll'">
                    <input type="checkbox" v-model="allRowsState" @change="toggleAll" />
                </div>

                <div v-if="column.columnType == 'string'" class="filter-box">
                    <div class="filter-wrapper input">
                        <input type="text" v-model="column.f1" :class="['form-control-tab-filter', specialClasses(column)]"
                               @change="applyFilter(column)"
                               @keyup.enter="applyFilter(column,true)" />
                    </div>
                    <div class="filter-wrapper clear"
                         @click="applyFilter(column,true,'clear')"
                         :style="{'background-image': 'url(' + x + ')'}">
                    </div>
                </div>

                <div v-if="column.columnType == 'range'" class="filter-box">
                    <div class="filter-wrapper input">
                        <input type="number" v-model="column.f1" :class="['form-control-tab-filter-fromto','from', column.columnType, specialClasses(column)]"
                               @change="applyFilter(column)"
                               @keyup.enter="applyFilter(column,true)" placeholder="from" />
                    </div>
                    <div class="filter-wrapper input">
                        <input type="number" v-model="column.f2" :class="['form-control-tab-filter-fromto','to', column.columnType, specialClasses(column)]"
                               @change="applyFilter(column)"
                               @keyup.enter="applyFilter(column,true)" placeholder="to" />
                    </div>
                    <div class="filter-wrapper-range clear"
                         @click="applyFilter(column,true,'clear')"
                         :style="{'background-image': 'url(' + x + ')'}">
                    </div>
                </div>

                <div v-if="column.columnType == 'date'" class="filter-box">
                    <div>
                        <date-picker :date="column.f1" @set="(date) => {column.f1 = date; applyFilter(column,true)}" />
                        <date-picker :date="column.f2" @set="(date) => {column.f2 = date; applyFilter(column,true)}" />
                    </div>
                    <div class="filter-wrapper-date clear"
                         @click="applyFilter(column,true,'clear')"
                         :style="{'background-image': 'url(' + x + ')'}">
                    </div>
                </div>

                <div v-if="column.columnType == 'dropdown'" class="disabled-hidden filter-box">
                    <div class="filter-wrapper input">
                        <select :class="['form-control-tab-filter', column.f1 ? '' : 'disabled', specialClasses(column)]"
                                v-model="column.f1" @change="applyFilter(column,true)">
                            <option value='' disabled>select</option>
                            <option v-for="option, id in column.dropdown" :key=option.id :value=id>{{option}}</option>
                        </select>
                    </div>
                    <div class="filter-wrapper clear"
                         @click="applyFilter(column,true,'clear')"
                         :style="{'background-image': 'url(' + x + ')'}">
                    </div>
                </div>
            </th>
        </tr>
    </thead>
</template>