// Nội dung JavaScript dùng chung cho hệ thống (nếu cần bổ sung xác nhận hành động, v.v.)
document.addEventListener('DOMContentLoaded', function () {
    var confirmButtons = document.querySelectorAll('[data-confirm]');
    confirmButtons.forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            if (!confirm(btn.getAttribute('data-confirm'))) {
                e.preventDefault();
            }
        });
    });
});
