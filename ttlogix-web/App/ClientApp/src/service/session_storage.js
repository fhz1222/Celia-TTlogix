import { v4 as uuidv4 } from 'uuid';
const CLIENT_ID_KEY = 'CLIENT_ID'
export default {
    get clientId() {
        const v = window.sessionStorage.getItem(CLIENT_ID_KEY);
        if (v == null) {
            const id = uuidv4();
            window.sessionStorage.setItem(CLIENT_ID_KEY, id);
            return id;
        }
        else {
            return v;
        }
    }
}