// The administrator screen: the JavaScript version of Views/Admin/Index.cshtml
// and its three panels.
(function () {
    var TAB_KEY = 'shopflow.admin.tab';
    var STATUSES = ['Pending', 'OutForDelivery', 'Delivered', 'Rejected'];

    var NEXT_STATUSES = {
        Pending: ['OutForDelivery', 'Rejected'],
        OutForDelivery: ['Delivered', 'Pending', 'Rejected'],
        Delivered: [],
        Rejected: []
    };

    var root;
    var model;

    function esc(value) { return window.Format.escapeHtml(value); }

    function searchQuery() {
        return new URLSearchParams(window.location.search).get('search') || '';
    }

    function pageQuery() {
        return parseInt(new URLSearchParams(window.location.search).get('page'), 10) || 1;
    }

    function ordersUrl(page) {
        var params = new URLSearchParams();
        var search = searchQuery();

        if (search) {
            params.set('search', search);
        }

        params.set('page', page);

        return '/admin/orders?' + params.toString();
    }

    function loadOrders(page) {
        window.Api.get(ordersUrl(page)).then(function (result) {
            if (!result.ok) {
                window.Alerts.show('error', result.message);
                return;
            }

            model.orders = result.data;

            var pane = document.getElementById('pane-orders');
            pane.innerHTML = ordersPanel();
            wireOrders(pane);

            var params = new URLSearchParams(window.location.search);
            params.set('page', page);
            window.history.replaceState(null, '', '?' + params.toString());
        });
    }

    function metric(icon, tone, label, value) {
        return '<div class="col-6 col-lg-3">' +
            '<div class="sf-metric">' +
                '<span class="sf-metric-icon ' + tone + '"><i class="bi ' + icon + '"></i></span>' +
                '<div>' +
                    '<p class="sf-metric-label">' + label + '</p>' +
                    '<p class="sf-metric-value sf-numeric">' + value + '</p>' +
                '</div>' +
            '</div>' +
        '</div>';
    }

    function header() {
        var clear = model.searchQuery
            ? '<a class="btn btn-quiet" href="admin.html">Clear</a>'
            : '';

        return '<div class="d-flex flex-wrap justify-content-between align-items-end gap-3 mb-4">' +
                '<div>' +
                    '<h1 class="sf-page-title"><i class="bi bi-speedometer2 me-2"></i>Admin dashboard</h1>' +
                    '<p class="sf-page-subtitle">Orders, catalogue and couriers in one place.</p>' +
                '</div>' +
                '<form class="d-flex gap-2" role="search" id="search-form">' +
                    '<input type="search" name="search" class="form-control" style="min-width: 16rem;" ' +
                           'value="' + esc(model.searchQuery || '') + '" ' +
                           'placeholder="Search by order id, courier or product" />' +
                    '<button type="submit" class="btn btn-quiet"><i class="bi bi-search"></i></button>' +
                    clear +
                '</form>' +
            '</div>' +
            '<div class="row g-3 mb-4">' +
                metric('bi-bag', 'is-primary', 'Total orders', model.totalOrders) +
                metric('bi-hourglass-split', 'is-warning', 'Pending', model.pendingOrders) +
                metric('bi-box-seam', 'is-success', 'Products', model.totalProducts) +
                metric('bi-truck', 'is-info', 'Couriers', model.totalCouriers) +
            '</div>';
    }

    function tabs() {
        return '<ul class="nav sf-tabs mb-4" id="adminTabs" role="tablist">' +
                '<li class="nav-item" role="presentation">' +
                    '<button class="nav-link active" id="tab-orders" data-bs-toggle="tab" data-bs-target="#pane-orders" ' +
                            'data-tab-key="orders" type="button" role="tab">' +
                        '<i class="bi bi-cart-check me-1"></i>Orders' +
                    '</button>' +
                '</li>' +
                '<li class="nav-item" role="presentation">' +
                    '<button class="nav-link" id="tab-products" data-bs-toggle="tab" data-bs-target="#pane-products" ' +
                            'data-tab-key="products" type="button" role="tab">' +
                        '<i class="bi bi-box-seam me-1"></i>Catalogue' +
                    '</button>' +
                '</li>' +
                '<li class="nav-item" role="presentation">' +
                    '<button class="nav-link" id="tab-couriers" data-bs-toggle="tab" data-bs-target="#pane-couriers" ' +
                            'data-tab-key="couriers" type="button" role="tab">' +
                        '<i class="bi bi-truck me-1"></i>Couriers' +
                    '</button>' +
                '</li>' +
            '</ul>';
    }

    // ---------------------------------------------------------------- orders panel

    function ordersPanel() {
        if (model.orders.items.length === 0) {
            return '<div class="sf-card">' +
                '<div class="sf-card-header">' +
                    '<h2 class="sf-card-title">Orders</h2>' +
                    '<span class="sf-muted small">0 shown</span>' +
                '</div>' +
                '<div class="sf-empty"><i class="bi bi-cart-x"></i>No orders match this search.</div>' +
            '</div>';
        }

        var rows = model.orders.items.map(function (order) {
            var courierCell = order.courierName
                ? '<div class="fw-semibold">' + esc(order.courierName) + '</div>' +
                  '<div class="sf-subtle small">' + esc(order.courierPhone) + '</div>'
                : '<span class="sf-subtle">Not assigned</span>';

            var allowed = NEXT_STATUSES[order.status] || [];

            var statusOptions = STATUSES.filter(function (status) {
                return status === order.status || allowed.indexOf(status) !== -1;
            }).map(function (status) {
                return '<option value="' + status + '"' + (order.status === status ? ' selected' : '') + '>' +
                    esc(window.Format.toDisplayName(status)) + '</option>';
            }).join('');

            var courierOptions = '<option value="">Choose a courier</option>' +
                model.couriers.filter(function (courier) {
                    return courier.isActive;
                }).map(function (courier) {
                    return '<option value="' + courier.courierId + '"' +
                        (order.courierId === courier.courierId ? ' selected' : '') + '>' +
                        esc(courier.fullName) + '</option>';
                }).join('');

            return '<tr>' +
                '<td class="fw-semibold">#' + order.orderId + '</td>' +
                '<td class="sf-muted">' + window.Format.toDate(order.createdAt) + '</td>' +
                '<td class="text-end sf-numeric">' + window.Format.toMoney(order.totalAmount) + '</td>' +
                '<td><span class="sf-badge ' + window.Format.toBadgeClass(order.status) + '">' +
                    esc(window.Format.toDisplayName(order.status)) + '</span></td>' +
                '<td>' + courierCell + '</td>' +
                '<td>' + (allowed.length === 0
                    ? '<span class="sf-subtle small">Final, cannot be changed</span>'
                    :
                    '<form class="d-flex flex-wrap gap-2 m-0" data-status-form data-order-id="' + order.orderId + '">' +
                        '<select name="Status" class="form-select form-select-sm" style="width: auto;" data-status-select>' +
                            statusOptions +
                        '</select>' +
                        '<select name="CourierId" class="form-select form-select-sm" style="width: auto;" data-courier-select>' +
                            courierOptions +
                        '</select>' +
                        '<button type="submit" class="btn btn-quiet btn-sm">Save</button>' +
                    '</form>') +
                '</td>' +
            '</tr>';
        }).join('');

        return '<div class="sf-card">' +
            '<div class="sf-card-header">' +
                '<h2 class="sf-card-title">Orders</h2>' +
                '<span class="sf-muted small">' + model.orders.totalCount + ' order(s)</span>' +
            '</div>' +
            '<div class="table-responsive">' +
                '<table class="sf-table">' +
                    '<thead>' +
                        '<tr>' +
                            '<th>Order</th>' +
                            '<th>Placed</th>' +
                            '<th class="text-end">Total</th>' +
                            '<th>Status</th>' +
                            '<th>Courier</th>' +
                            '<th>Update</th>' +
                        '</tr>' +
                    '</thead>' +
                    '<tbody>' + rows + '</tbody>' +
                '</table>' +
            '</div>' +
            window.Pager.render(model.orders, 'page') +
        '</div>';
    }

    // -------------------------------------------------------------- products panel

    function field(name, label, attributes, id) {
        return '<div class="mb-3">' +
            '<label class="form-label" for="' + id + '">' + label + '</label>' +
            '<input class="form-control" id="' + id + '" name="' + name + '" ' + attributes + ' />' +
            '<span class="field-validation-error" data-validation-for="' + name + '"></span>' +
        '</div>';
    }

    function productRow(product) {
        var variants;

        if (product.variants.length === 0) {
            variants = '<span class="sf-subtle small">No variants yet.</span>';
        } else {
            variants = '<ul class="list-unstyled m-0 d-grid gap-2">' +
                product.variants.map(function (variant) {
                    return '<li class="d-flex flex-wrap align-items-center gap-2">' +
                        '<span class="sf-badge is-neutral">' + esc(variant.label) + '</span>' +
                        '<span class="sf-subtle small sf-numeric">stock ' + variant.stockQuantity + '</span>' +
                        '<form class="d-flex gap-1 m-0" data-price-form data-variant-id="' + variant.variantId + '">' +
                            '<input class="form-control form-control-sm sf-numeric" name="NewPrice" ' +
                                   'type="number" step="0.01" min="0.01" style="width: 7rem;" ' +
                                   'value="' + variant.price + '" required />' +
                            '<button type="submit" class="btn btn-quiet btn-sm" title="Save price">' +
                                '<i class="bi bi-check2"></i>' +
                            '</button>' +
                        '</form>' +
                    '</li>';
                }).join('') +
            '</ul>';
        }

        return '<tr>' +
            '<td style="min-width: 12rem;">' +
                '<div class="fw-semibold">' + esc(product.productName) + '</div>' +
                '<button class="btn btn-quiet btn-sm mt-2" type="button" ' +
                        'data-bs-toggle="collapse" data-bs-target="#variant-' + product.productId + '">' +
                    '<i class="bi bi-plus-lg me-1"></i>Variant' +
                '</button>' +
                '<div class="collapse mt-2" id="variant-' + product.productId + '">' +
                    '<form class="row g-2" data-add-variant-form data-product-id="' + product.productId + '">' +
                        '<div class="col-6">' +
                            '<input class="form-control form-control-sm" name="Color" placeholder="Colour" maxlength="50" />' +
                            '<span class="field-validation-error" data-validation-for="Color"></span>' +
                        '</div>' +
                        '<div class="col-6">' +
                            '<input class="form-control form-control-sm" name="Size" placeholder="Size" maxlength="20" />' +
                            '<span class="field-validation-error" data-validation-for="Size"></span>' +
                        '</div>' +
                        '<div class="col-6">' +
                            '<input class="form-control form-control-sm" name="Price" type="number" step="0.01" min="0.01" placeholder="Price" />' +
                            '<span class="field-validation-error" data-validation-for="Price"></span>' +
                        '</div>' +
                        '<div class="col-6">' +
                            '<input class="form-control form-control-sm" name="StockQuantity" type="number" min="0" placeholder="Stock" />' +
                            '<span class="field-validation-error" data-validation-for="StockQuantity"></span>' +
                        '</div>' +
                        '<div class="col-12">' +
                            '<div class="validation-summary-errors" data-validation-summary></div>' +
                            '<button type="submit" class="btn btn-primary btn-sm w-100">Save variant</button>' +
                        '</div>' +
                    '</form>' +
                '</div>' +
            '</td>' +
            '<td>' + variants + '</td>' +
            '<td class="text-end" style="width: 4rem;">' +
                '<button type="button" class="sf-icon-btn" title="Remove product" ' +
                        'data-delete-product="' + product.productId + '">' +
                    '<i class="bi bi-trash3"></i>' +
                '</button>' +
            '</td>' +
        '</tr>';
    }

    function productsPanel() {
        var newProductBody;

        if (model.categories.length === 0) {
            newProductBody = '<p class="sf-muted mb-0">Create a category first.</p>';
        } else {
            var categoryOptions = '<option value="">Choose a category</option>' +
                model.categories.map(function (category) {
                    return '<option value="' + category.categoryId + '">' + esc(category.name) + '</option>';
                }).join('');

            newProductBody =
                '<form data-add-product-form>' +
                    '<div class="validation-summary-errors mb-3" data-validation-summary></div>' +
                    field('ProductName', 'Name', 'maxlength="150" placeholder="Runner sneakers"', 'product-name') +
                    '<div class="mb-3">' +
                        '<label class="form-label" for="product-category">Category</label>' +
                        '<select class="form-select" id="product-category" name="CategoryId">' + categoryOptions + '</select>' +
                        '<span class="field-validation-error" data-validation-for="CategoryId"></span>' +
                    '</div>' +
                    '<button type="submit" class="btn btn-primary w-100">Add product</button>' +
                '</form>';
        }

        var catalogue;

        if (model.categories.length === 0) {
            catalogue = '<div class="sf-empty"><i class="bi bi-tags"></i>There are no categories yet.</div>';
        } else {
            var body = model.categories.map(function (category) {
                var products = model.products.filter(function (product) {
                    return product.categoryId === category.categoryId;
                });

                var groupRow = '<tr class="sf-group-row">' +
                    '<td colspan="3">' +
                        '<i class="bi bi-tag-fill me-1"></i>' + esc(category.name) +
                        '<span class="sf-badge is-neutral ms-2">' + products.length + '</span>' +
                    '</td>' +
                '</tr>';

                if (products.length === 0) {
                    return groupRow + '<tr><td colspan="3" class="sf-subtle">No products in this category.</td></tr>';
                }

                return groupRow + products.map(productRow).join('');
            }).join('');

            catalogue = '<div class="table-responsive">' +
                '<table class="sf-table"><tbody>' + body + '</tbody></table>' +
            '</div>';
        }

        return '<div class="row g-4">' +
            '<div class="col-lg-4">' +
                '<div class="sf-card mb-4">' +
                    '<div class="sf-card-header"><h2 class="sf-card-title">New category</h2></div>' +
                    '<div class="sf-card-body">' +
                        '<form data-add-category-form>' +
                            '<div class="validation-summary-errors mb-3" data-validation-summary></div>' +
                            field('Name', 'Name', 'maxlength="100" placeholder="Electronics"', 'category-name') +
                            '<button type="submit" class="btn btn-primary w-100">Add category</button>' +
                        '</form>' +
                    '</div>' +
                '</div>' +
                '<div class="sf-card">' +
                    '<div class="sf-card-header"><h2 class="sf-card-title">New product</h2></div>' +
                    '<div class="sf-card-body">' + newProductBody + '</div>' +
                '</div>' +
            '</div>' +
            '<div class="col-lg-8">' +
                '<div class="sf-card">' +
                    '<div class="sf-card-header">' +
                        '<h2 class="sf-card-title">Catalogue</h2>' +
                        '<span class="sf-muted small">' + model.products.length + ' product(s)</span>' +
                    '</div>' +
                    catalogue +
                '</div>' +
            '</div>' +
        '</div>';
    }

    // -------------------------------------------------------------- couriers panel

    function couriersPanel() {
        var list;

        if (model.couriers.length === 0) {
            list = '<div class="sf-empty"><i class="bi bi-person-badge"></i>No couriers have been registered yet.</div>';
        } else {
            var rows = model.couriers.map(function (courier) {
                return '<tr>' +
                    '<td class="fw-semibold">' + esc(courier.fullName) + '</td>' +
                    '<td class="sf-muted">' + esc(courier.phone) + '</td>' +
                    '<td class="sf-muted">' + esc(courier.vehicleType || '—') + '</td>' +
                    '<td>' +
                        '<span class="sf-badge ' + (courier.isActive ? 'is-delivered' : 'is-neutral') + '">' +
                            (courier.isActive ? 'Active' : 'Inactive') +
                        '</span>' +
                    '</td>' +
                    '<td class="text-end text-nowrap">' +
                        '<button type="button" class="btn btn-quiet btn-sm me-2" ' +
                                'data-toggle-courier="' + courier.courierId + '" data-active="' + courier.isActive + '">' +
                            (courier.isActive ? 'Deactivate' : 'Activate') +
                        '</button>' +
                        '<button type="button" class="sf-icon-btn" title="Delete courier" ' +
                                'data-delete-courier="' + courier.courierId + '">' +
                            '<i class="bi bi-trash3"></i>' +
                        '</button>' +
                    '</td>' +
                '</tr>';
            }).join('');

            list = '<div class="table-responsive">' +
                '<table class="sf-table">' +
                    '<thead>' +
                        '<tr>' +
                            '<th>Name</th>' +
                            '<th>Phone</th>' +
                            '<th>Vehicle</th>' +
                            '<th>Status</th>' +
                            '<th></th>' +
                        '</tr>' +
                    '</thead>' +
                    '<tbody>' + rows + '</tbody>' +
                '</table>' +
            '</div>';
        }

        return '<div class="row g-4">' +
            '<div class="col-lg-4">' +
                '<div class="sf-card">' +
                    '<div class="sf-card-header"><h2 class="sf-card-title">Register a courier</h2></div>' +
                    '<div class="sf-card-body">' +
                        '<form data-add-courier-form>' +
                            '<div class="validation-summary-errors mb-3" data-validation-summary></div>' +
                            field('Username', 'Username', 'maxlength="50"', 'courier-username') +
                            field('Email', 'Email', 'type="email"', 'courier-email') +
                            field('Password', 'Temporary password', 'type="password" autocomplete="new-password"', 'courier-password') +
                            '<div class="row g-2 mb-3">' +
                                '<div class="col-6">' +
                                    '<label class="form-label" for="courier-first">First name</label>' +
                                    '<input class="form-control" id="courier-first" name="FirstName" maxlength="50" />' +
                                    '<span class="field-validation-error" data-validation-for="FirstName"></span>' +
                                '</div>' +
                                '<div class="col-6">' +
                                    '<label class="form-label" for="courier-last">Last name</label>' +
                                    '<input class="form-control" id="courier-last" name="LastName" maxlength="50" />' +
                                    '<span class="field-validation-error" data-validation-for="LastName"></span>' +
                                '</div>' +
                            '</div>' +
                            field('Phone', 'Phone', 'placeholder="+37499123456"', 'courier-phone') +
                            field('VehicleType', 'Vehicle', 'maxlength="30" placeholder="Scooter"', 'courier-vehicle') +
                            '<button type="submit" class="btn btn-primary w-100">Register courier</button>' +
                        '</form>' +
                    '</div>' +
                '</div>' +
            '</div>' +
            '<div class="col-lg-8">' +
                '<div class="sf-card">' +
                    '<div class="sf-card-header">' +
                        '<h2 class="sf-card-title">Couriers</h2>' +
                        '<span class="sf-muted small">' + model.couriers.length + ' registered</span>' +
                    '</div>' +
                    list +
                '</div>' +
            '</div>' +
        '</div>';
    }

    // ------------------------------------------------------------------ behaviour

    function reloadWith(result) {
        window.Alerts.flashResult(result);
        window.location.reload();
    }

    // A failed form keeps the page as it is and shows the messages under the fields,
    // which is what re-rendering the view used to achieve.
    function submitForm(form, request) {
        request.then(function (result) {
            if (!result.ok) {
                window.Forms.apply(form, result);
                window.Alerts.show('error', result.message);
                return;
            }

            reloadWith(result);
        });
    }

    // Shows the courier list only when the chosen status actually needs one.
    function syncCourierSelect(statusSelect) {
        var form = statusSelect.closest('form');
        var courierSelect = form.querySelector('[data-courier-select]');

        if (!courierSelect) {
            return;
        }

        var needsCourier = statusSelect.value === 'OutForDelivery';

        courierSelect.classList.toggle('d-none', !needsCourier);
        courierSelect.required = needsCourier;

        if (!needsCourier) {
            courierSelect.value = '';
        }
    }

    // Remembers which tab was open, so a reload after a command does not throw the
    // administrator back to the first tab.
    function restoreTab() {
        var storedKey = null;

        try {
            storedKey = localStorage.getItem(TAB_KEY);
        } catch (error) {
            /* ignored */
        }

        if (storedKey) {
            var storedTab = document.querySelector('#adminTabs button[data-tab-key="' + storedKey + '"]');

            if (storedTab) {
                new bootstrap.Tab(storedTab).show();
            }
        }

        document.querySelectorAll('#adminTabs button[data-bs-toggle="tab"]').forEach(function (tab) {
            tab.addEventListener('shown.bs.tab', function (event) {
                try {
                    localStorage.setItem(TAB_KEY, event.target.dataset.tabKey);
                } catch (error) {
                    /* ignored */
                }
            });
        });
    }

    function wireOrders(container) {
        container.querySelectorAll('[data-status-select]').forEach(function (select) {
            syncCourierSelect(select);
            select.addEventListener('change', function () { syncCourierSelect(select); });
        });

        container.querySelectorAll('[data-status-form]').forEach(function (form) {
            form.addEventListener('submit', function (event) {
                event.preventDefault();

                var courierValue = form.elements['CourierId'].value;

                submitForm(form, window.Api.put('/admin/orders/status', {
                    orderId: parseInt(form.dataset.orderId, 10),
                    status: form.elements['Status'].value,
                    courierId: courierValue ? parseInt(courierValue, 10) : null
                }));
            });
        });

        window.Pager.wire(container, loadOrders);
    }

    function wireEvents() {
        document.getElementById('search-form').addEventListener('submit', function (event) {
            event.preventDefault();

            var value = this.elements['search'].value.trim();

            window.location.href = 'admin.html' + (value ? '?search=' + encodeURIComponent(value) : '');
        });

        wireOrders(root);

        root.querySelectorAll('[data-price-form]').forEach(function (form) {
            form.addEventListener('submit', function (event) {
                event.preventDefault();

                submitForm(form, window.Api.put('/admin/variants/price', {
                    variantId: parseInt(form.dataset.variantId, 10),
                    newPrice: parseFloat(form.elements['NewPrice'].value)
                }));
            });
        });

        var categoryForm = root.querySelector('[data-add-category-form]');

        categoryForm.addEventListener('submit', function (event) {
            event.preventDefault();

            submitForm(categoryForm, window.Api.post('/admin/categories', {
                name: categoryForm.elements['Name'].value
            }));
        });

        var productForm = root.querySelector('[data-add-product-form]');

        if (productForm) {
            productForm.addEventListener('submit', function (event) {
                event.preventDefault();

                submitForm(productForm, window.Api.post('/admin/products', {
                    productName: productForm.elements['ProductName'].value,
                    categoryId: parseInt(productForm.elements['CategoryId'].value, 10) || 0
                }));
            });
        }

        root.querySelectorAll('[data-add-variant-form]').forEach(function (form) {
            form.addEventListener('submit', function (event) {
                event.preventDefault();

                submitForm(form, window.Api.post('/admin/variants', {
                    productId: parseInt(form.dataset.productId, 10),
                    color: form.elements['Color'].value,
                    size: form.elements['Size'].value,
                    price: parseFloat(form.elements['Price'].value) || 0,
                    stockQuantity: parseInt(form.elements['StockQuantity'].value, 10) || 0
                }));
            });
        });

        var courierForm = root.querySelector('[data-add-courier-form]');

        courierForm.addEventListener('submit', function (event) {
            event.preventDefault();

            submitForm(courierForm, window.Api.post('/admin/couriers', {
                username: courierForm.elements['Username'].value,
                email: courierForm.elements['Email'].value,
                password: courierForm.elements['Password'].value,
                firstName: courierForm.elements['FirstName'].value,
                lastName: courierForm.elements['LastName'].value,
                phone: courierForm.elements['Phone'].value,
                vehicleType: courierForm.elements['VehicleType'].value
            }));
        });

        root.querySelectorAll('[data-delete-product]').forEach(function (button) {
            button.addEventListener('click', function () {
                if (!confirm('Remove this product from the shop?')) {
                    return;
                }

                window.Api.del('/admin/products/' + button.dataset.deleteProduct).then(reloadWith);
            });
        });

        root.querySelectorAll('[data-toggle-courier]').forEach(function (button) {
            button.addEventListener('click', function () {
                var action = button.dataset.active === 'true' ? 'deactivate' : 'activate';

                window.Api.post('/admin/couriers/' + button.dataset.toggleCourier + '/' + action).then(reloadWith);
            });
        });

        root.querySelectorAll('[data-delete-courier]').forEach(function (button) {
            button.addEventListener('click', function () {
                if (!confirm('Delete this courier account?')) {
                    return;
                }

                window.Api.del('/admin/couriers/' + button.dataset.deleteCourier).then(reloadWith);
            });
        });

        restoreTab();
    }

    function render() {
        root.innerHTML = header() + tabs() +
            '<div class="tab-content">' +
                '<div class="tab-pane fade show active" id="pane-orders" role="tabpanel">' + ordersPanel() + '</div>' +
                '<div class="tab-pane fade" id="pane-products" role="tabpanel">' + productsPanel() + '</div>' +
                '<div class="tab-pane fade" id="pane-couriers" role="tabpanel">' + couriersPanel() + '</div>' +
            '</div>';

        wireEvents();
    }

    document.addEventListener('DOMContentLoaded', function () {
        if (!window.Auth.requireRole('Admin')) {
            return;
        }

        window.Layout.render();

        root = document.getElementById('admin-root');

        var search = searchQuery();

        Promise.all([
            window.Api.get('/admin/dashboard' + (search ? '?search=' + encodeURIComponent(search) : '')),
            window.Api.get(ordersUrl(pageQuery()))
        ]).then(function (results) {
            var dashboard = results[0];
            var orders = results[1];

            if (!dashboard.ok || !orders.ok) {
                window.Alerts.show('error', dashboard.ok ? orders.message : dashboard.message);
                return;
            }

            model = dashboard.data;
            model.orders = orders.data;
            render();
        });
    });
})();
