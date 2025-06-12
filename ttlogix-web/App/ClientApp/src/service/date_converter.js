import moment from 'moment'

export function toServer(date) {
    if (!date) return ''
    return moment(date).format('YYYY-MM-DDTHH:mm:ss');
}
export function fromServer(val) {
    return moment(val, 'YYYY-MM-DD\\THH:mm:ss').toDate()
}