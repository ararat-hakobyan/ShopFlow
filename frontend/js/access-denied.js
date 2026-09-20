document.addEventListener('DOMContentLoaded', function () {
    window.Layout.render();

    document.getElementById('sign-in-as-someone-else').addEventListener('submit', function (event) {
        event.preventDefault();

        window.Api.post('/account/logout').then(function () {
            window.Auth.clear();
            window.location.href = 'login.html';
        });
    });
});
