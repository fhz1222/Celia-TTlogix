import {
    createStore,
    createLogger,
    Store as VuexStore,
    CommitOptions,
    DispatchOptions,
    Module
} from 'vuex'
import { state } from './state'
import { State } from './state';
import { Getters, getters } from './getters'
import { Mutations, mutations } from './mutations'
import { Actions, actions } from './actions'

export type Store<S = State> = Omit<VuexStore<S>, 'getters' | 'commit' | 'dispatch'>
    & {
        commit<K extends keyof Mutations, P extends Parameters<Mutations[K]>[1]>(
            key: K,
            payload: P,
            options?: CommitOptions
        ): ReturnType<Mutations[K]>
    } & {
        dispatch<K extends keyof Actions>(
            key: K,
            payload?: Parameters<Actions[K]>[1],
            options?: DispatchOptions
        ): ReturnType<Actions[K]>
    } & {
        getters: {
            [K in keyof Getters]: ReturnType<Getters[K]>
        }
    }

const ttStore = {
    state,
    getters,
    mutations,
    actions
};

// Plug in logger when in development environment
const debug = process.env.NODE_ENV !== 'production';
const plugins = debug ? [createLogger({})] : [];

export const store = createStore({
    plugins, ...ttStore
});

export function useStore(): Store {
    return store as Store;
}
