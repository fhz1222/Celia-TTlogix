export default function (v) {
    let e = v.charAt(0) === '%'
    let s = v.charAt(v.length - 1) === '%'
    if (e) v = v.substring(1)
    if (s) v = v.substring(0, v.length - 1)

    let filterType = null
    if (s && e) filterType = 'Contains'
    else if (s) filterType = 'StartsWith'
    else if (e) filterType = 'EndsWith'
    else filterType = 'Equals'

    return { filter: v, filterType: filterType }
}