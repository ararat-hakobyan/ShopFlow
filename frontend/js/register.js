document.addEventListener('DOMContentLoaded', function () {
    window.Layout.render();

    var form = document.getElementById('register-form');

    form.addEventListener('submit', function (event) {
        event.preventDefault();
        window.Alerts.clear();

        var payload = {
            username: form.elements['Username'].value,
            email: form.elements['Email'].value,
            password: form.elements['Password'].value,
            confirmPassword: form.elements['ConfirmPassword'].value
        };

        window.Api.post('/account/register', payload, { allowAnonymous: true }).then(function (result) {
            if (!result.ok) {
                window.Forms.apply(form, result);
                return;
            }

            window.Alerts.flash('success', result.message);
            window.location.href = 'login.html';
        });
    });
});
