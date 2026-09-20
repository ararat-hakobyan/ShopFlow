// Puts the validation messages the API sent back next to the field they belong to.
//
// The MVC version re-rendered the form and ModelState provided the message and the
// red outline. Here the page never goes away, so the same two things are applied by
// hand: the "is-invalid" class on the input and the text in its message span.
(function () {
    function clear(form) {
        form.querySelectorAll('.is-invalid').forEach(function (input) {
            input.classList.remove('is-invalid');
        });

        form.querySelectorAll('[data-validation-for]').forEach(function (span) {
            span.textContent = '';
        });

        var summary = form.querySelector('[data-validation-summary]');

        if (summary) {
            summary.textContent = '';
        }
    }

    function apply(form, result) {
        clear(form);

        var errors = result.errors || {};
        var summaryMessages = [];

        Object.keys(errors).forEach(function (field) {
            var messages = errors[field] || [];

            var input = form.querySelector('[name="' + field + '"]');
            var span = form.querySelector('[data-validation-for="' + field + '"]');

            if (input && span) {
                input.classList.add('is-invalid');
                span.textContent = messages[0] || '';
            } else {
                summaryMessages = summaryMessages.concat(messages);
            }
        });

        // A failure that belongs to no single field (for example "this email is taken")
        // goes into the summary box at the top of the form.
        if (summaryMessages.length === 0 && Object.keys(errors).length === 0 && result.message) {
            summaryMessages.push(result.message);
        }

        var summary = form.querySelector('[data-validation-summary]');

        if (summary) {
            summary.textContent = summaryMessages.join(' ');
        }
    }

    window.Forms = {
        clear: clear,
        apply: apply
    };
})();
