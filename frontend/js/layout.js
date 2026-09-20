// Draws the navigation bar and the footer, the job _Layout.cshtml used to do.
(function () {
    function renderHeader() {
        var host = document.querySelector('[data-sf-header]');

        if (!host) {
            return;
        }

        var user = window.Auth.user();
        var right;

        if (user) {
            right =
                '<div class="d-flex align-items-center gap-2">' +
                    '<span class="sf-user-chip">' +
                        '<i class="bi bi-person-circle"></i> ' +
                        window.Format.escapeHtml(user.username) +
                        ' <span class="sf-subtle">&middot; ' + window.Format.escapeHtml(user.role) + '</span>' +
                    '</span>' +
                    '<form class="m-0" data-logout-form>' +
                        '<button type="submit" class="btn btn-quiet btn-sm">' +
                            '<i class="bi bi-box-arrow-right me-1"></i>Sign out' +
                        '</button>' +
                    '</form>' +
                '</div>';
        } else {
            right =
                '<div class="d-flex align-items-center gap-2">' +
                    '<a class="btn btn-quiet btn-sm" href="login.html">Sign in</a>' +
                    '<a class="btn btn-primary btn-sm" href="register.html">Create account</a>' +
                '</div>';
        }

        host.innerHTML =
            '<nav class="sf-navbar">' +
                '<div class="container d-flex align-items-center justify-content-between py-2">' +
                    '<a class="sf-brand" href="index.html">' +
                        '<span class="sf-brand-mark"><i class="bi bi-bag-check-fill"></i></span> ' +
                        'ShopFlow' +
                    '</a>' +
                    right +
                '</div>' +
            '</nav>';

        var logoutForm = host.querySelector('[data-logout-form]');

        if (logoutForm) {
            logoutForm.addEventListener('submit', function (event) {
                event.preventDefault();

                window.Api.post('/account/logout').then(function () {
                    window.Auth.clear();
                    window.location.href = 'login.html';
                });
            });
        }
    }

    function renderFooter() {
        var host = document.querySelector('[data-sf-footer]');

        if (!host) {
            return;
        }

        host.className = 'sf-footer';
        host.innerHTML =
            '<div class="container d-flex flex-wrap justify-content-between gap-2">' +
                '<span>&copy; ' + new Date().getUTCFullYear() + ' ShopFlow</span>' +
                '<span class="sf-subtle">ASP.NET Core Web API &middot; Entity Framework Core</span>' +
            '</div>';
    }

    window.Layout = {
        render: function () {
            renderHeader();
            renderFooter();
            window.Alerts.drain();
        }
    };
})();
