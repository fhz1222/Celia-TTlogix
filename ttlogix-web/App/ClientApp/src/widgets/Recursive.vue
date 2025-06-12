<template>
    <li v-bind:class="[!hasChildren || folder.parent === null ? 'is-leaf' : expanded ? 'is-folder folder-expanded' : 'is-folder']">
        <span>
            <input v-if="folder.parent !== null" type="checkbox" :checked="modelValue" @change="$emit('update:modelValue', $event.target.checked)" />&nbsp;
            <span v-on:click="expand()">{{ folder.shortName }}</span>
        </span>
        <ul class="sub-folders" v-if="hasChildren" v-show="expanded || folder.parent === null">
            <folder v-for="child in folder.children" :folder="child" :key="child.code" v-model="child.isChecked"></folder>
        </ul>
        <div class="folder-empty" v-else v-show="!hasChildren && expanded">{{$t('widgets.main.title')}}</div>
    </li>
</template>
<script>
    import { defineComponent } from 'vue';
export default defineComponent({
        name: "folder",
        props: {
            folder: Object,
            modelValue: Boolean
        },
        data() {
            return {
                expanded: false
            }
        },
        methods: {
            expand() {
                if (!this.hasChildren) {
                    return;
                }

                this.expanded = !this.expanded;
            }
        },
        computed: {
            hasChildren() {
                return this.folder.children.length > 0;
            }
        }
    })
</script>