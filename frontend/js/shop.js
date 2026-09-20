// The customer screen: the JavaScript version of Views/Shop/Index.cshtml.
(function () {
    var root;
    var model;

    function esc(value) { return window.Format.escapeHtml(value); }

    function query() {
        var params = new URLSearchParams(window.location.search);

        return {
            categoryId: params.get('categoryId'),
            productId: params.get('productId')
        };
    }

    // Reloading the page with a new query string is what the MVC links and the
    // RedirectToAction calls did, so the behaviour stays exactly the same.
    function goTo(categoryId, productId) {
        var params = new URLSearchParams();

        if (categoryId) {
            params.set('categoryId', categoryId);
        }

        if (productId) {
            params.set('productId', productId);
        }

        var search = params.toString();

        window.location.href = 'shop.html' + (search ? '?' + search : '');
    }

    function profileCard() {
        return '<div class="col-lg-4">' +
            '<div class="sf-card">' +
                '<div class="sf-card-body text-center">' +
                    '<i class="bi bi-person-circle" style="font-size: 2.5rem; color: var(--sf-text-subtle);"></i>' +
                    '<h2 class="sf-card-title mt-2 mb-0">' + esc(model.username) + '</h2>' +
                    '<p class="sf-page-subtitle">' + esc(model.email) + '</p>' +
                '</div>' +
                '<div class="sf-card-footer d-flex justify-content-between small">' +
                    '<span class="sf-muted">Member since</span>' +
                    '<span class="fw-semibold">' + window.Format.toDate(model.memberSince) + '</span>' +
                '</div>' +
                '<div class="sf-card-footer d-flex justify-content-between small border-top-0 pt-0">' +
                    '<span class="sf-muted">Orders placed</span>' +
                    '<span class="fw-semibold">' + model.orders.length + '</span>' +
                '</div>' +
            '</div>' +
        '</div>';
    }

    function categoryCard() {
        var chips = '<a class="sf-chip ' + (model.selectedCategoryId ? '' : 'is-active') + '" href="#" data-category="">All</a>';

        model.categories.forEach(function (category) {
            var active = model.selectedCategoryId === category.categoryId ? 'is-active' : '';

            chips += '<a class="sf-chip ' + active + '" href="#" data-category="' + category.categoryId + '">' +
                esc(category.name) + '</a>';
        });

        return '<div class="sf-card mb-4">' +
            '<div class="sf-card-body d-flex flex-wrap align-items-center gap-2">' +
                '<span class="sf-muted small fw-semibold text-uppercase me-1">Category</span>' +
                chips +
            '</div>' +
        '</div>';
    }

    function pickProductCard() {
        var inner;

        if (model.products.length === 0) {
            inner = '<div class="sf-empty">' +
                '<i class="bi bi-inbox"></i>' +
                'There are no products in this category yet.' +
            '</div>';
        } else {
            var options = '<option value="">Choose a product</option>';

            model.products.forEach(function (product) {
                var selected = product.productId === model.selectedProductId ? ' selected' : '';

                options += '<option value="' + product.productId + '"' + selected + '>' +
                    esc(product.productName) + '</option>';
            });

            inner = '<div class="row g-3 align-items-end">' +
                    '<div class="col-sm-8">' +
                        '<label class="form-label" for="productId">Product</label>' +
                        '<select class="form-select" id="productId" name="productId">' + options + '</select>' +
                    '</div>' +
                    '<div class="col-sm-4"></div>' +
                '</div>' +
                variantForm();
        }

        return '<div class="sf-card mb-4">' +
            '<div class="sf-card-header">' +
                '<h2 class="sf-card-title"><i class="bi bi-shop me-2"></i>Pick a product</h2>' +
            '</div>' +
            '<div class="sf-card-body">' + inner + '</div>' +
        '</div>';
    }

    function variantForm() {
        var selectedProduct = model.products.filter(function (product) {
            return product.productId === model.selectedProductId;
        })[0];

        if (!selectedProduct) {
            return '';
        }

        if (model.selectedProductVariants.length === 0) {
            return '<hr class="my-4" />' +
                '<p class="sf-muted mb-0">This product has no variants on sale at the moment.</p>';
        }

        var options = '<option value="">Choose a variant</option>';

        model.selectedProductVariants.forEach(function (variant) {
            var stockText = variant.isInStock
                ? '(' + variant.stockQuantity + ' in stock)'
                : '(out of stock)';

            options += '<option value="' + variant.variantId + '"' + (variant.isInStock ? '' : ' disabled') + '>' +
                esc(variant.label) + ' &mdash; ' + window.Format.toMoney(variant.price) + ' ' + stockText +
                '</option>';
        });

        return '<hr class="my-4" />' +
            '<form class="row g-3 align-items-end" id="add-to-cart-form">' +
                '<div class="col-sm-7">' +
                    '<label class="form-label" for="VariantId">Colour and size</label>' +
                    '<select class="form-select" id="VariantId" name="VariantId" required>' + options + '</select>' +
                '</div>' +
                '<div class="col-sm-2">' +
                    '<label class="form-label" for="Quantity">Qty</label>' +
                    '<input class="form-control sf-numeric text-center" id="Quantity" name="Quantity" ' +
                           'type="number" min="1" max="100" value="1" required />' +
                '</div>' +
                '<div class="col-sm-3">' +
                    '<button type="submit" class="btn btn-primary w-100">' +
                        '<i class="bi bi-cart-plus me-1"></i>Add' +
                    '</button>' +
                '</div>' +
            '</form>';
    }

    function basketCard() {
        var basket = model.basket;
        var inner;

        if (basket.isEmpty) {
            inner = '<div class="sf-empty">' +
                '<i class="bi bi-cart-x"></i>' +
                'Your basket is empty. Pick a product above to get started.' +
            '</div>';
        } else {
            var rows = basket.items.map(function (item) {
                return '<tr>' +
                    '<td class="fw-semibold">' + esc(item.productName) + '</td>' +
                    '<td class="sf-muted">' + esc(item.color) + ' / ' + esc(item.size) + '</td>' +
                    '<td class="text-center sf-numeric">' + item.quantity + '</td>' +
                    '<td class="text-end sf-numeric fw-semibold">' + window.Format.toMoney(item.lineTotal) + '</td>' +
                    '<td class="text-end">' +
                        '<button type="button" class="sf-icon-btn" title="Remove from basket" ' +
                                'data-remove-variant="' + item.variantId + '">' +
                            '<i class="bi bi-trash3"></i>' +
                        '</button>' +
                    '</td>' +
                '</tr>';
            }).join('');

            inner = '<div class="table-responsive">' +
                    '<table class="sf-table">' +
                        '<thead>' +
                            '<tr>' +
                                '<th>Product</th>' +
                                '<th>Variant</th>' +
                                '<th class="text-center">Qty</th>' +
                                '<th class="text-end">Total</th>' +
                                '<th></th>' +
                            '</tr>' +
                        '</thead>' +
                        '<tbody>' + rows + '</tbody>' +
                    '</table>' +
                '</div>' +
                '<div class="sf-card-footer d-flex flex-wrap justify-content-between align-items-center gap-2">' +
                    '<div>' +
                        '<span class="sf-muted small">Total</span>' +
                        '<strong class="ms-2 sf-numeric">' + window.Format.toMoney(basket.total) + '</strong>' +
                    '</div>' +
                    '<button type="button" class="btn btn-primary" id="place-order">' +
                        '<i class="bi bi-bag-check me-1"></i>Place order' +
                    '</button>' +
                '</div>';
        }

        return '<div class="sf-card mb-4">' +
            '<div class="sf-card-header">' +
                '<h2 class="sf-card-title"><i class="bi bi-basket me-2"></i>Your basket</h2>' +
                '<span class="sf-badge is-neutral">' + basket.itemCount + ' item(s)</span>' +
            '</div>' +
            inner +
        '</div>';
    }

    function historyCard() {
        if (model.orders.length === 0) {
            return '<div class="sf-card">' +
                '<div class="sf-card-header">' +
                    '<h2 class="sf-card-title"><i class="bi bi-clock-history me-2"></i>Order history</h2>' +
                '</div>' +
                '<div class="sf-empty">' +
                    '<i class="bi bi-receipt"></i>' +
                    'You have not placed any orders yet.' +
                '</div>' +
            '</div>';
        }

        var rows = model.orders.map(function (order) {
            return '<tr>' +
                '<td class="fw-semibold">#' + order.orderId + '</td>' +
                '<td class="sf-muted">' + window.Format.toDateTime(order.createdAt) + '</td>' +
                '<td><span class="sf-badge ' + window.Format.toBadgeClass(order.status) + '">' +
                    esc(window.Format.toDisplayName(order.status)) + '</span></td>' +
                '<td class="text-end sf-numeric">' + window.Format.toMoney(order.totalAmount) + '</td>' +
                '<td class="text-end">' +
                    '<button type="button" class="btn btn-quiet btn-sm" ' +
                            'data-bs-toggle="modal" data-bs-target="#order-' + order.orderId + '">' +
                        'Details' +
                    '</button>' +
                '</td>' +
            '</tr>';
        }).join('');

        var modals = model.orders.map(function (order) {
            return '<div class="modal fade" id="order-' + order.orderId + '" tabindex="-1" aria-hidden="true" ' +
                        'data-order-id="' + order.orderId + '">' +
                '<div class="modal-dialog modal-lg modal-dialog-centered">' +
                    '<div class="modal-content">' +
                        '<div class="modal-header">' +
                            '<h3 class="sf-card-title">Order #' + order.orderId + '</h3>' +
                            '<button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>' +
                        '</div>' +
                        '<div class="modal-body" data-order-details-body>' +
                            '<p class="sf-muted mb-0">Loading…</p>' +
                        '</div>' +
                    '</div>' +
                '</div>' +
            '</div>';
        }).join('');

        return '<div class="sf-card">' +
            '<div class="sf-card-header">' +
                '<h2 class="sf-card-title"><i class="bi bi-clock-history me-2"></i>Order history</h2>' +
            '</div>' +
            '<div class="table-responsive">' +
                '<table class="sf-table">' +
                    '<thead>' +
                        '<tr>' +
                            '<th>Order</th>' +
                            '<th>Date</th>' +
                            '<th>Status</th>' +
                            '<th class="text-end">Total</th>' +
                            '<th></th>' +
                        '</tr>' +
                    '</thead>' +
                    '<tbody>' + rows + '</tbody>' +
                '</table>' +
            '</div>' +
            modals +
        '</div>';
    }

    function render() {
        root.innerHTML =
            '<div class="row g-4">' +
                profileCard() +
                '<div class="col-lg-8">' +
                    categoryCard() +
                    pickProductCard() +
                    basketCard() +
                    historyCard() +
                '</div>' +
            '</div>';

        wireEvents();
        window.OrderDetails.wire(root);
    }

    function wireEvents() {
        root.querySelectorAll('[data-category]').forEach(function (chip) {
            chip.addEventListener('click', function (event) {
                event.preventDefault();
                goTo(chip.dataset.category, null);
            });
        });

        var productSelect = root.querySelector('#productId');

        if (productSelect) {
            productSelect.addEventListener('change', function () {
                goTo(model.selectedCategoryId, productSelect.value);
            });
        }

        var addForm = root.querySelector('#add-to-cart-form');

        if (addForm) {
            addForm.addEventListener('submit', function (event) {
                event.preventDefault();

                var payload = {
                    variantId: parseInt(addForm.elements['VariantId'].value, 10) || 0,
                    quantity: parseInt(addForm.elements['Quantity'].value, 10) || 0
                };

                window.Api.post('/shop/cart/items', payload).then(reloadWith);
            });
        }

        root.querySelectorAll('[data-remove-variant]').forEach(function (button) {
            button.addEventListener('click', function () {
                window.Api.post('/shop/cart/items/remove', {
                    variantId: parseInt(button.dataset.removeVariant, 10)
                }).then(reloadWith);
            });
        });

        var placeOrder = root.querySelector('#place-order');

        if (placeOrder) {
            placeOrder.addEventListener('click', function () {
                window.Api.post('/shop/orders').then(reloadWith);
            });
        }
    }

    function reloadWith(result) {
        window.Alerts.flashResult(result);
        window.location.reload();
    }

    document.addEventListener('DOMContentLoaded', function () {
        if (!window.Auth.requireRole('Customer')) {
            return;
        }

        window.Layout.render();

        root = document.getElementById('shop-root');

        var current = query();
        var params = new URLSearchParams();

        if (current.categoryId) {
            params.set('categoryId', current.categoryId);
        }

        if (current.productId) {
            params.set('productId', current.productId);
        }

        var search = params.toString();

        window.Api.get('/shop/storefront' + (search ? '?' + search : '')).then(function (result) {
            if (!result.ok) {
                window.Alerts.show('error', result.message);
                return;
            }

            model = result.data;
            render();
        });
    });
})();
