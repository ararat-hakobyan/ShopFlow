// Keeps the access token and the signed-in user in localStorage.
// This is what the authentication cookie used to do, except that now the browser
// does not send it automatically: every request adds it as an Authorization header.
(function () {
    var STORAGE_KEY = 'shopflow.auth';

    function read() {
        try {
            var raw = localStorage.getItem(STORAGE_KEY);

            if (!raw) {
                return null;
            }

            var parsed = JSON.parse(raw);

            if (!parsed || !parsed.token) {
                return null;
            }

            if (parsed.expiresAtUtc && new Date(parsed.expiresAtUtc).getTime() <= Date.now()) {
                localStorage.removeItem(STORAGE_KEY);
                return null;
            }

            return parsed;
        } catch (error) {
            return null;
        }
    }

    function save(loginResponse) {
        try {
            localStorage.setItem(STORAGE_KEY, JSON.stringify({
                token: loginResponse.token,
                expiresAtUtc: loginResponse.expiresAtUtc,
                user: loginResponse.user
            }));
        } catch (error) {
            /* ignored */
        }
    }

    function clear() {
        try {
            localStorage.removeItem(STORAGE_KEY);
        } catch (error) {
            /* ignored */
        }
    }

    function user() {
        var session = read();
        return session ? session.user : null;
    }

    function homePageFor(role) {
        if (role === 'Admin') {
            return 'admin.html';
        }

        if (role === 'Courier') {
            return 'courier.html';
        }

        return 'shop.html';
    }

    // Guards a page the same way [Authorize(Roles = "...")] guarded a controller.
    function requireRole(role) {
        var session = read();

        if (!session) {
            window.location.replace('login.html?returnUrl=' +
                encodeURIComponent(window.location.pathname.split('/').pop() + window.location.search));
            return null;
        }

        if (session.user.role !== role) {
            window.location.replace('access-denied.html');
            return null;
        }

        return session;
    }

    window.Auth = {
        read: read,
        save: save,
        clear: clear,
        user: user,
        homePageFor: homePageFor,
        requireRole: requireRole
    };
})();
