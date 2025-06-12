<template>
    <div class="login">
        <form @submit.prevent="onSubmit">
            <div :class="formClass">
                <h3>TT-Logix</h3>
                <div class="error" v-if="error">{{error}}</div>
                <input type="text" class="txt" v-model="username" :placeholder="$t('login.username')" name="username" />
                <input type="password" class="txt" v-model="password" :placeholder="$t('login.password')" name="password" />
                <select v-model="whs" class="txt">
                    <option :value="null">{{ $t('login.warehouse') }}</option>
                    <option v-for="w in whsList" :value="w.code" :key="w.code">{{ w.name }}</option>
                </select>
                <select v-model="lang">
                    <option v-for="(v,k) in $parent.languages" :key="k" :value="k">{{ v }}</option>
                </select>

                <div class="bar">
                    <button type="submit">
                        {{$t('login.submit')}} <i></i>
                    </button>
                </div>
            </div>
        </form>
    </div>
</template>

<script>
    import { defineComponent } from 'vue';
    import { useStore } from '@/store';
import { MutationTypes } from '@/store/mutation-types';
export default defineComponent({
        data() {
            return {
               username: null,
               password : null,
               whs : null,
               whsList : [],
               error : null,
               formClass: 'form',
               lang : this.$i18n.locale
            }
        },
        created() {
            this.$dataProvider.warehouses.getWarehouses().then(resp => this.whsList = resp)
        },
        mounted() {
            this.$emit('app.authorize')
        },
        methods: {
            onSubmit : function() {
                const username = this.username;
                const password = this.password;
                const warehouse = this.whs
                const redirect = this.$auth.redirect();
                this.formClass = 'form bounceOut animated';
                this.error = null;
                this.$auth.login({
                    data : {
                        password, username, warehouse
                    },
                    redirect : {path : redirect ? redirect.from.path : '/'}
                }).then(response => {
                    this.$bus.$emit('app.authorized')
                    useStore().commit(MutationTypes.SET_WHS, response.data.whsCode);
                    return response
                }, error => {
                    this.formClass = 'form bounceIn animated';
                    this.error = this.$t('login.error')
                    if (error.response) {
                        
                        this.error = this.error || error.response.data.message
                    }
                })
            },
            setLang(lang) {
                this.$bus.$emit('lang', lang)
            },
        },
        watch: {
            lang() {
                this.setLang(this.lang);
            }
        },

    })
</script>
<style lang="scss" scoped>
.form {
    margin: 50px auto;
    background: #326195;
    padding: 24px;
    color: white;
    width: 250px;
    .error {
        font-weight: bold;
        text-align: center;
        padding-bottom: 10px;
    }
    h3 {
        font-weight: normal;
        padding-bottom: 10px;
        span {
            color: #95b8d6;
        }
    }
    .txt, select {
        display: block;
        width: 100%;
        border: 0px solid;
        margin-bottom: 10px;
        font-size: 14px;
        height: 34px;
        outline: none;
        line-height: 34px;
        color: black;
        padding: 0px 17px;
        box-sizing: border-box;
    }
    .bar {
        text-align: right;
        button {
            color: white;
            border: 1px solid white;
            background: #326195;
            padding: 0px 18px;
            font-size: 14px;
            cursor: pointer;
        }
    }
}
</style>

