document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.alert').forEach(function (alert) {
        setTimeout(function () {
            var bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
            if (bsAlert) bsAlert.close();
        }, 5000);
    });

    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.forEach(function (el) {
        new bootstrap.Tooltip(el);
    });

    // Confirmación para acciones de "Quiero ayudar"
    document.querySelectorAll('.confirm-ayudar').forEach(function (form) {
        form.addEventListener('submit', function (e) {
            if (!confirm('¿Confirmas que quieres ofrecerte como donante para esta solicitud?')) {
                e.preventDefault();
            }
        });
    });

    document.querySelectorAll('.auto-filtro').forEach(function (el) {
        el.addEventListener('change', function () {
            el.form.submit();
        });
    });
});
