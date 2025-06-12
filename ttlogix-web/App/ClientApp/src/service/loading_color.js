export default function (loading) {
    if (loading.allowedForDispatch) return '#00ff0069'
    if (loading.calculatedStatusString == 'NewJob') return '#ff000069'
    if (loading.calculatedStatusString == 'Processing') return '#ffeb0069'
    if (loading.calculatedStatusString == 'Picked') return '#006df369'
    return '#ffffff69'
}