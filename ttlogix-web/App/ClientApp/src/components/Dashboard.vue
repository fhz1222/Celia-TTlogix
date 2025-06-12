<template>
    <div class="wrap hello">
        <h1 v-if="$auth.user()">{{$t('app.logged')}}: {{ $auth.user().firstName }} {{ $auth.user().lastName }} [{{ $auth.user().code }}]</h1>
    </div>
</template>

<script>
    import { defineComponent } from 'vue';
export default defineComponent({
        data() {
            return {
               
            }
        },
        methods: {
        }

    })
</script>

