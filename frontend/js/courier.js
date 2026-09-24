// The courier screen: the JavaScript version of Views/Courier/Index.cshtml
// together with the _OrderCard partial.
(function () {
    var root;
    var model;

    function esc(value) { return window.Format.escapeHtml(value); }

    // The partial _OrderCard.cshtml. "action" is Accept, Complete or None,
    // exactly like the ViewData entry the Razor version passed in.
    function orderCard(order, action) {
        var body;

        if (order.lines.length === 0) {
            body = '<div class="sf-card-body sf-muted">This order has no lines.</div>';
        } else {
            var rows = order.lines.map(function (line) {
                return '<tr>' +
                    '<td class="fw-semibold">' + esc(line.productName) + '</td>' +
                    '<td class="sf-muted">' + esc(line.color) + ' / ' + esc(line.size) + '</td>' +
                    '<td class="text-center sf-numeric">' + line.quantity + '</td>' +
                    '<td class="text-end sf-numeric">' + window.Format.toMoney(line.lineTotal) + '</td>' +
                '</tr>';
            }).join('');

            body = '<div class="table-responsive">' +
                '<table class="sf-table">' +
                    '<thead>' +
                        '<tr>' +
                            '<th>Product</th>' +
                            '<th>Variant</th>' +
                            '<th class="text-center">Qty</th>' +
                            '<th class="text-end">Total</th>' +
                        '</tr>' +
                    '</thead>' +
                    '<tbody>' + rows + '</tbody>' +
                '</table>' +
            '</div>';
        }

        var due = order.estimatedDeliveryTime
            ? '<span> &middot; due ' + window.Format.toShortDateTime(order.estimatedDeliveryTime) + '</span>'
            : '';

        var button = '';

        if (action === 'Accept') {
            button = '<button type="button" class="btn btn-primary btn-sm" data-accept="' + order.orderId + '">' +
                '<i class="bi bi-box-arrow-in-down me-1"></i>Accept' +
            '</button>';
        } else if (action === 'Complete') {
            button = '<button type="button" class="btn btn-primary btn-sm" data-complete="' + order.orderId + '">' +
                '<i class="bi bi-check2 me-1"></i>Mark as delivered' +
            '</button>';
        }

        return '<div class="sf-card mb-3">' +
            '<div class="sf-card-header">' +
                '<h3 class="sf-card-title">Order #' + order.orderId + '</h3>' +
                '<span class="sf-badge ' + window.Format.toBadgeClass(order.status) + '">' +
                    esc(window.Format.toDisplayName(order.status)) + '</span>' +
            '</div>' +
            body +
            '<div class="sf-card-footer d-flex flex-wrap justify-content-between align-items-center gap-2">' +
                '<div class="small sf-muted">' +
                    '<span class="sf-numeric">' + order.itemCount + '</span> item(s) &middot; ' +
                    '<strong class="sf-numeric">' + window.Format.toMoney(order.totalAmount) + '</strong>' +
                    due +
                '</div>' +
                button +
            '</div>' +
        '</div>';
    }

    function pane(orders, action, emptyIcon, emptyText) {
        if (orders.length === 0) {
            return '<div class="sf-card"><div class="sf-empty"><i class="bi ' + emptyIcon + '"></i>' +
                emptyText + '</div></div>';
        }

        return orders.map(function (order) { return orderCard(order, action); }).join('');
    }

    function historyPane() {
        return pane(model.deliveredOrders.items, 'None', 'bi-clock-history', 'Your delivered orders will appear here.') +
            window.Pager.render(model.deliveredOrders, 'page');
    }

    function loadHistory(page) {
        window.Api.get('/courier/console?page=' + page).then(function (result) {
            if (!result.ok) {
                window.Alerts.show('error', result.message);
                return;
            }

            model.deliveredOrders = result.data.deliveredOrders;

            var container = document.getElementById('history-pane');
            container.innerHTML = historyPane();
            window.Pager.wire(container, loadHistory);
        });
    }

    function render() {
        root.innerHTML =
            '<div class="d-flex flex-wrap justify-content-between align-items-end gap-2 mb-4">' +
                '<div>' +
                    '<h1 class="sf-page-title"><i class="bi bi-truck me-2"></i>Courier console</h1>' +
                    '<p class="sf-page-subtitle">Pick up new deliveries and close the ones you finished.</p>' +
                '</div>' +
            '</div>' +
            '<ul class="nav sf-tabs mb-4" role="tablist">' +
                '<li class="nav-item" role="presentation">' +
                    '<button class="nav-link active" data-bs-toggle="tab" data-bs-target="#available-pane" type="button" role="tab">' +
                        'Available' +
                        '<span class="sf-badge is-pending ms-1">' + model.availableOrders.length + '</span>' +
                    '</button>' +
                '</li>' +
                '<li class="nav-item" role="presentation">' +
                    '<button class="nav-link" data-bs-toggle="tab" data-bs-target="#active-pane" type="button" role="tab">' +
                        'My deliveries' +
                        '<span class="sf-badge is-delivery ms-1">' + model.activeOrders.length + '</span>' +
                    '</button>' +
                '</li>' +
                '<li class="nav-item" role="presentation">' +
                    '<button class="nav-link" data-bs-toggle="tab" data-bs-target="#history-pane" type="button" role="tab">' +
                        'History' +
                        '<span class="sf-badge is-delivered ms-1">' + model.deliveredOrders.totalCount + '</span>' +
                    '</button>' +
                '</li>' +
            '</ul>' +
            '<div class="tab-content">' +
                '<div class="tab-pane fade show active" id="available-pane" role="tabpanel">' +
                    pane(model.availableOrders, 'Accept', 'bi-clipboard-x', 'No orders are waiting for a courier right now.') +
                '</div>' +
                '<div class="tab-pane fade" id="active-pane" role="tabpanel">' +
                    pane(model.activeOrders, 'Complete', 'bi-emoji-smile', 'You have no deliveries in progress.') +
                '</div>' +
                '<div class="tab-pane fade" id="history-pane" role="tabpanel">' +
                    historyPane() +
                '</div>' +
            '</div>';

        window.Pager.wire(document.getElementById('history-pane'), loadHistory);

        root.querySelectorAll('[data-accept]').forEach(function (button) {
            button.addEventListener('click', function () {
                window.Api.post('/courier/orders/' + button.dataset.accept + '/accept').then(reloadWith);
            });
        });

        root.querySelectorAll('[data-complete]').forEach(function (button) {
            button.addEventListener('click', function () {
                if (!confirm('Confirm that this order has been delivered?')) {
                    return;
                }

                window.Api.post('/courier/orders/' + button.dataset.complete + '/complete').then(reloadWith);
            });
        });
    }

    function reloadWith(result) {
        window.Alerts.flashResult(result);
        window.location.reload();
    }

    document.addEventListener('DOMContentLoaded', function () {
        if (!window.Auth.requireRole('Courier')) {
            return;
        }

        window.Layout.render();

        root = document.getElementById('courier-root');

        window.Api.get('/courier/console').then(function (result) {
            if (!result.ok) {
                window.Alerts.show('error', result.message);
                return;
            }

            model = result.data;
            render();
        });
    });
})();
