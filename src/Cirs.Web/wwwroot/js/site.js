// Comfort features only. Every real rule is checked again on the server by UserValidator.
(function () {
    'use strict';

    // Show or hide a password.
    document.querySelectorAll('[data-toggle-password]').forEach(function (button) {
        var input = document.getElementById(button.dataset.target);
        if (!input) { return; }

        button.addEventListener('click', function () {
            var reveal = input.type === 'password';
            input.type = reveal ? 'text' : 'password';
            button.classList.toggle('is-on', reveal);
            button.setAttribute('aria-pressed', reveal ? 'true' : 'false');
            button.setAttribute('aria-label', reveal ? 'Hide password' : 'Show password');
        });
    });

    // Live strength bar and rule checklist under the sign up password.
    var meter = document.querySelector('[data-strength]');
    var rules = document.querySelector('[data-pw-rules]');
    var passwordInput = meter ? document.getElementById(meter.dataset.target) : null;

    if (passwordInput) {
        var labels = ['Not entered yet', 'Weak', 'Fair', 'Good', 'Strong'];
        var label = meter.querySelector('[data-strength-label]');

        var update = function () {
            var value = passwordInput.value;
            var checks = {
                length: value.length >= 8 && value.length <= 64,
                upper: /[A-Z]/.test(value),
                lower: /[a-z]/.test(value),
                digit: /[0-9]/.test(value)
            };

            var score = 0;
            if (value.length > 0) {
                score = Object.keys(checks).filter(function (key) { return checks[key]; }).length;
                if (score === 4 && value.length < 12 && !/[^A-Za-z0-9]/.test(value)) { score = 3; }
                score = Math.max(score, 1);
            }

            meter.className = 'strength s-' + score;
            label.textContent = labels[score];

            if (rules) {
                rules.querySelectorAll('li').forEach(function (item) {
                    var met = checks[item.dataset.rule];
                    item.classList.toggle('met', met);
                    item.querySelector('.mark').textContent = met ? '✓' : '○';
                });
            }
        };

        passwordInput.addEventListener('input', update);
        update();
    }

    // Stop double submits and show that something is happening.
    document.querySelectorAll('[data-auth-form]').forEach(function (form) {
        var button = form.querySelector('[data-submit]');
        if (!button) { return; }

        form.addEventListener('submit', function () {
            window.setTimeout(function () {
                button.classList.add('is-loading');
                button.setAttribute('aria-busy', 'true');
            }, 0);
        });

        // If the user comes back with the browser Back button, reset the button.
        window.addEventListener('pageshow', function () {
            button.classList.remove('is-loading');
            button.removeAttribute('aria-busy');
        });

        // Put the cursor where the user needs to type next.
        var target = form.querySelector('input.is-invalid');
        if (!target) {
            var inputs = form.querySelectorAll('input:not([type="hidden"]):not([type="checkbox"])');
            for (var i = 0; i < inputs.length; i++) {
                if (!inputs[i].value) { target = inputs[i]; break; }
            }
            if (!target && inputs.length) { target = inputs[0]; }
        }
        if (target) { target.focus(); }
    });
}());
