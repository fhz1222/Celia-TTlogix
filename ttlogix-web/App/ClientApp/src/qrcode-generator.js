import QRCode from 'qrcode'

export default function toBase64QR(text) {
    return new Promise((resolve, reject) => {
        let opts ={
            errorCorrectionLevel: 'H',
            margin: 0
        }
        QRCode.toDataURL(text, opts, function (err, url) {
            if (err) {
                reject(err)
            }
            resolve(url)
        })
    })
}