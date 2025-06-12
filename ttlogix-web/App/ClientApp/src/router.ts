import { createRouter, createWebHashHistory } from "vue-router";
import routes from "./routes";

const router = createRouter({
    history: createWebHashHistory(),
    routes
});

export default router;

// GoTo
export function goTo(state: string, params:any | null = null){ 
    if(!params) {
        router.push({ name: state });
    } else {
        router.push({ name: state, params: params });
    }
}