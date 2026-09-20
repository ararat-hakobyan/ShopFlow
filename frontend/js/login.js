document.addEventListener('DOMContentLoaded', function () {
    window.Layout.render();

    var form = document.getElementById('login-form');

    form.addEventListener('submit', function (event) {
        event.preventDefault();
        window.Alerts.clear();

        var payload = {
            email: form.elements['Email'].value,
            password: form.elements['Password'].value
        };

        // allowAnonymous, because a wrong password answers with 401 and that must show
        // a message here instead of bouncing the visitor back to this same page.
        window.Api.post('/account/login', payload, { allowAnonymous: true }).then(function (result) {
            if (!result.ok) {
                window.Forms.apply(form, result);
                return;
            }

            window.Auth.save(result.data);

            var returnUrl = new URLSearchParams(window.location.search).get('returnUrl');

            window.location.href = returnUrl || window.Auth.homePageFor(result.data.user.role);
        });
    });
});
