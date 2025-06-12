import mitt from 'mitt';
class EventBus {
    constructor(){
        this.emitter = mitt()
    }

    $on(name, handler){
        this.emitter.on(name, handler)
    }

    $off(name, handler){
        this.emitter.off(name, handler)
    }

    $emit(name, handler){
        this.emitter.emit(name, handler)
    }
}

const bus = new EventBus();

export default bus;