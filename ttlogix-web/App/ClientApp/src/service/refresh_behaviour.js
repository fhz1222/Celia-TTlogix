import bus from '../event_bus'
function disableF5(e) {
    if ((e.which || e.keyCode) == 116) {
        e.preventDefault();
        bus.emit('refresh');
    }
}

export function enable() {
    window.addEventListener("keydown", disableF5)
}
export function disable() {
    window.removeEventListener("keydown", disableF5)
}