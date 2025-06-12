export default {
    isTTK(refNo) {
        return refNo && refNo != '' && refNo.indexOf('TTK') >= 0
    }
}