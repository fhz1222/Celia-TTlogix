const callbacks: {[route: string]: () => Promise<void> } = {}

export const onRouteExit = (path: string, callback: () => Promise<void>) => callbacks[path] = callback;
export const triggerRouteExit = async (path: string) => {
    if (callbacks[path]){
        await callbacks[path](); 
        delete callbacks[path];
    }
};