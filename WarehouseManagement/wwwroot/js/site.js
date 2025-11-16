function quickSearch() {
    const searchTerm = document.getElementById('quickSearchInput').value;
    if (searchTerm.length > 2) {
        // يمكن إضافة AJAX هنا للبحث السريع
        window.location.href = `/Materials/Search?SearchTerm=${encodeURIComponent(searchTerm)}`;
    }
}

// تحسين تجربة المستخدم
document.addEventListener('DOMContentLoaded', function () {
    // إخفاء التنبيهات تلقائياً بعد 5 ثوان
    setTimeout(function () {
        const alerts = document.querySelectorAll('.alert');
        alerts.forEach(function (alert) {
            if (alert.classList.contains('alert-success')) {
                alert.style.transition = 'opacity 0.5s';
                alert.style.opacity = '0';
                setTimeout(() => alert.remove(), 500);
            }
        });
    }, 5000);

    // تحسين النماذج
    const forms = document.querySelectorAll('form');
    forms.forEach(function (form) {
        form.addEventListener('submit', function () {
            const submitBtn = form.querySelector('input[type="submit"]');
            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.value = 'جاري المعالجة...';
            }
        });
    });
});
