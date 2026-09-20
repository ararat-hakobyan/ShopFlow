// A thin wrapper around fetch that attaches the token and always hands back
// the same shape, so no page has to repeat the error handling.
(function () {
    function request(path, options) {
        options = options || {};

        var session = window.Auth.read();
        var headers = {};

        if (options.body !== undefined) {
            headers['Content-Type'] = 'application/json';
        }

        if (session) {
            headers['Authorization'] = 'Bearer ' + session.token;
        }

        return fetch(window.ShopFlowConfig.apiBaseUrl + path, {
            method: options.method || 'GET',
            headers: headers,
            body: options.body === undefined ? undefined : JSON.stringify(options.body)
        }).then(function (response) {
            return response.text().then(function (text) {
                var data = null;

                if (text) {
                    try {
                        data = JSON.parse(text);
                    } catch (error) {
                        data = null;
                    }
                }

                if (response.status === 401 && !options.allowAnonymous) {
                    window.Auth.clear();
                    window.location.replace('login.html');
                }

                return {
                    ok: response.ok,
                    status: response.status,
                    data: data,
                    message: (data && data.message) || '',
                    errors: (data && data.errors) || null
                };
            });
        }).catch(function () {
            return {
                ok: false,
                status: 0,
                data: null,
                message: 'The server could not be reached. Is the API running?',
                errors: null
            };
        });
    }

    window.Api = {
        get: function (path) { return request(path); },
        post: function (path, body, extra) {
            return request(path, Object.assign({ method: 'POST', body: body === undefined ? {} : body }, extra || {}));
        },
        put: function (path, body) { return request(path, { method: 'PUT', body: body === undefined ? {} : body }); },
        del: function (path) { return request(path, { method: 'DELETE' }); }
    };
})();
