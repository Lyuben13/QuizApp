(function () {
    function showLoader() {
        const el = document.getElementById("page-loader");
        if (el) el.classList.remove("d-none");
    }
    window.addEventListener("beforeunload", showLoader);
    document.addEventListener("DOMContentLoaded", function () {
        document.querySelectorAll('a[href], button[type="submit"], input[type="submit"]').forEach(el => {
            el.addEventListener("click", (e) => {
                // игнорирай Ctrl/Meta за отваряне в нов таб
                if (e.ctrlKey || e.metaKey || e.shiftKey) return;
                showLoader();
            });
        });
    });
})();
