export default {

    request: function (req, token) {
        this.drivers.http.setHeaders.call(this, req, {
            Authorization: 'Bearer ' + token
        });
    },

    response: function (res) {
        var token   = res.data.token

        if (token) {
            token = token.split(/Bearer:?\s?/i)

            return token[token.length > 1 ? 1 : 0].trim()
        }
    }
};