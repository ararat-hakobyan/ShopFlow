// The JavaScript twin of FormattingExtensions.cs, so every screen prints money,
// dates and status badges exactly the way the Razor views did.
(function () {
    var MONTHS = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

    function escapeHtml(value) {
        if (value === null || value === undefined) {
            return '';
        }

        return String(value)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;');
    }

    // "1,250 AMD" - the same as decimal.ToString("N0", InvariantCulture) + " AMD".
    function toMoney(amount) {
        var number = Number(amount) || 0;

        return Math.round(number).toLocaleString('en-US', { maximumFractionDigits: 0 }) + ' AMD';
    }

    function toBadgeClass(status) {
        switch (status) {
            case 'Pending': return 'is-pending';
            case 'OutForDelivery': return 'is-delivery';
            case 'Delivered': return 'is-delivered';
            case 'Rejected': return 'is-rejected';
            default: return 'is-neutral';
        }
    }

    function toDisplayName(status) {
        return status === 'OutForDelivery' ? 'Out for delivery' : status;
    }

    // The API sends a date as text. Reading the numbers straight out of that text
    // (instead of going through the Date object) keeps the displayed value identical
    // to the one the server produced, whatever time zone the visitor sits in.
    function parts(value) {
        if (!value) {
            return null;
        }

        var match = /^(\d{4})-(\d{2})-(\d{2})[T ](\d{2}):(\d{2})/.exec(String(value));

        if (!match) {
            return null;
        }

        return {
            year: match[1],
            month: MONTHS[parseInt(match[2], 10) - 1],
            day: match[3],
            hour: match[4],
            minute: match[5]
        };
    }

    // "05 Jan 2026"
    function toDate(value) {
        var p = parts(value);
        return p ? p.day + ' ' + p.month + ' ' + p.year : '';
    }

    // "05 Jan 2026, 14:30"
    function toDateTime(value) {
        var p = parts(value);
        return p ? p.day + ' ' + p.month + ' ' + p.year + ', ' + p.hour + ':' + p.minute : '';
    }

    // "05 Jan, 14:30"
    function toShortDateTime(value) {
        var p = parts(value);
        return p ? p.day + ' ' + p.month + ', ' + p.hour + ':' + p.minute : '';
    }

    window.Format = {
        escapeHtml: escapeHtml,
        toMoney: toMoney,
        toBadgeClass: toBadgeClass,
        toDisplayName: toDisplayName,
        toDate: toDate,
        toDateTime: toDateTime,
        toShortDateTime: toShortDateTime
    };
})();
