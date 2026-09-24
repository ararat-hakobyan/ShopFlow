(function () {
    function link(param, page, label, disabled) {
        if (disabled) {
            return '<span class="btn btn-quiet btn-sm disabled">' + label + '</span>';
        }

        var params = new URLSearchParams(window.location.search);
        params.set(param, page);

        return '<a class="btn btn-quiet btn-sm" href="?' + params.toString() + '" data-pager-page="' + page + '">' +
            label +
        '</a>';
    }

    function render(paged, param) {
        if (!paged || paged.totalPages <= 1) {
            return '';
        }

        return '<div class="d-flex justify-content-between align-items-center px-3 py-2 border-top">' +
            link(param, paged.page - 1, '<i class="bi bi-chevron-left"></i> Previous', paged.page <= 1) +
            '<span class="sf-muted small">Page ' + paged.page + ' of ' + paged.totalPages + '</span>' +
            link(param, paged.page + 1, 'Next <i class="bi bi-chevron-right"></i>', paged.page >= paged.totalPages) +
        '</div>';
    }

    function wire(container, onPageChange) {
        container.querySelectorAll('[data-pager-page]').forEach(function (anchor) {
            anchor.addEventListener('click', function (event) {
                event.preventDefault();
                onPageChange(parseInt(anchor.dataset.pagerPage, 10));
            });
        });
    }

    window.Pager = {
        render: render,
        wire: wire
    };
})();
