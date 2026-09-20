// The green and red banners above the page.
//
// In the MVC version a successful POST redirected and TempData carried the message
// over to the next request. Here the page reloads itself instead, so the message is
// parked in sessionStorage for exactly one page load, which behaves the same way.
(function () {
    var KEY = 'shopflow.flash';

    function container() {
        return document.querySelector('[data-sf-alerts]');
    }

    function markup(type, message) {
        if (type === 'success') {
            return '<div class="sf-alert is-success mb-3" role="status">' +
                '<i class="bi bi-check-circle me-1"></i>' + window.Format.escapeHtml(message) +
                '</div>';
        }

        return '<div class="sf-alert is-error mb-3" role="alert">' +
            '<i class="bi bi-exclamation-triangle me-1"></i>' + window.Format.escapeHtml(message) +
            '</div>';
    }

    function show(type, message) {
        var host = container();

        if (!host || !message) {
            return;
        }

        host.innerHTML = markup(type, message);
        window.scrollTo({ top: 0, behavior: 'smooth' });
    }

    // Store a message and show it after the page has reloaded.
    function flash(type, message) {
        try {
            sessionStorage.setItem(KEY, JSON.stringify({ type: type, message: message }));
        } catch (error) {
            /* ignored */
        }
    }

    function drain() {
        var raw = null;

        try {
            raw = sessionStorage.getItem(KEY);
            sessionStorage.removeItem(KEY);
        } catch (error) {
            return;
        }

        if (!raw) {
            return;
        }

        try {
            var parsed = JSON.parse(raw);
            show(parsed.type, parsed.message);
        } catch (error) {
            /* ignored */
        }
    }

    function clear() {
        var host = container();

        if (host) {
            host.innerHTML = '';
        }
    }

    window.Alerts = {
        show: show,
        flash: flash,
        drain: drain,
        clear: clear,
        fromResult: function (result) { show(result.ok ? 'success' : 'error', result.message); },
        flashResult: function (result) { flash(result.ok ? 'success' : 'error', result.message); }
    };
})();
