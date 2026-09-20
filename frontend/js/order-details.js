// The body of the order history modal: the JavaScript version of _OrderDetails.cshtml.
(function () {

    function esc(value) { return window.Format.escapeHtml(value); }

    function statusBlock(order) {
        if (order.status === 'Pending') {
            return '<div class="sf-alert is-muted mb-3">' +
                '<i class="bi bi-hourglass-split me-1"></i>' +
                'We have received your order. A courier will pick it up shortly.' +
                '</div>';
        }

        if (order.status === 'Rejected') {
            return '<div class="sf-alert is-error mb-3">' +
                '<i class="bi bi-x-circle me-1"></i>' +
                'This order was cancelled. If this was not expected, please contact support.' +
                '</div>';
        }

        if (order.status === 'OutForDelivery' && order.courierName) {
            var track = '';

            if (order.courierCoordinates && order.courierCoordinates.trim() !== '') {
                track = '<a class="btn btn-quiet btn-sm mt-2" ' +
                    'href="https://maps.google.com/?q=' + encodeURIComponent(order.courierCoordinates) + '" ' +
                    'target="_blank" rel="noopener noreferrer">' +
                    '<i class="bi bi-geo-alt me-1"></i>Track on the map</a>';
            }

            return '<div class="sf-card mb-3">' +
                '<div class="sf-card-body">' +
                    '<div class="fw-semibold mb-1"><i class="bi bi-truck me-1"></i>On the way</div>' +
                    '<div class="small">' +
                        '<div>Courier: <strong>' + esc(order.courierName) + '</strong></div>' +
                        '<div>Phone: <a href="tel:' + esc(order.courierPhone) + '">' + esc(order.courierPhone) + '</a></div>' +
                    '</div>' +
                    track +
                '</div>' +
            '</div>';
        }

        if (order.status === 'Delivered') {
            return '<div class="sf-alert is-success mb-3">' +
                '<i class="bi bi-check-circle me-1"></i>' +
                'Delivered' + (order.courierName ? ' by ' + esc(order.courierName) : '') + '.' +
                '</div>';
        }

        return '';
    }

    function render(order) {
        var lines = (order.lines || []).map(function (line) {
            return '<tr>' +
                '<td class="fw-semibold">' + esc(line.productName) + '</td>' +
                '<td class="sf-muted">' + esc(line.color) + ' / ' + esc(line.size) + '</td>' +
                '<td class="text-center sf-numeric">' + line.quantity + '</td>' +
                '<td class="text-end sf-numeric">' + window.Format.toMoney(line.unitPrice) + '</td>' +
                '<td class="text-end sf-numeric fw-semibold">' + window.Format.toMoney(line.lineTotal) + '</td>' +
            '</tr>';
        }).join('');

        return '<div class="d-flex flex-wrap align-items-center gap-2 mb-3">' +
                '<span class="sf-badge ' + window.Format.toBadgeClass(order.status) + '">' +
                    esc(window.Format.toDisplayName(order.status)) + '</span>' +
                '<span class="sf-muted small">Placed on ' + window.Format.toDateTime(order.createdAt) + '</span>' +
            '</div>' +
            statusBlock(order) +
            '<div class="table-responsive">' +
                '<table class="sf-table">' +
                    '<thead>' +
                        '<tr>' +
                            '<th>Product</th>' +
                            '<th>Variant</th>' +
                            '<th class="text-center">Qty</th>' +
                            '<th class="text-end">Unit price</th>' +
                            '<th class="text-end">Total</th>' +
                        '</tr>' +
                    '</thead>' +
                    '<tbody>' + lines + '</tbody>' +
                    '<tfoot>' +
                        '<tr>' +
                            '<td colspan="4" class="text-end fw-semibold">Order total</td>' +
                            '<td class="text-end sf-numeric fw-semibold">' + window.Format.toMoney(order.totalAmount) + '</td>' +
                        '</tr>' +
                    '</tfoot>' +
                '</table>' +
            '</div>';
    }

    // Loads the details of an order into its modal the first time the modal is opened.
    function wire(root) {
        root.querySelectorAll('[data-order-id]').forEach(function (modal) {
            modal.addEventListener('show.bs.modal', function () {
                if (modal.dataset.loaded === 'true') {
                    return;
                }

                var body = modal.querySelector('[data-order-details-body]');

                window.Api.get('/shop/orders/' + modal.dataset.orderId).then(function (result) {
                    if (!result.ok) {
                        body.innerHTML = '<p class="sf-alert is-error mb-0">The details of this order could not be loaded.</p>';
                        return;
                    }

                    body.innerHTML = render(result.data);
                    modal.dataset.loaded = 'true';
                });
            });
        });
    }

    window.OrderDetails = { render: render, wire: wire };
})();
