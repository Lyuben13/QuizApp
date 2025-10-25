// Enhanced Page Loader with better UX
(function () {
    let loaderTimeout = null;
    let isLoaderVisible = false;

    function showLoader() {
        if (isLoaderVisible) return;
        
        const el = document.getElementById("page-loader");
        if (el) {
            el.classList.remove("d-none");
            isLoaderVisible = true;
            
            // Add fade-in animation
            el.style.opacity = '0';
            el.style.transition = 'opacity 0.3s ease-in-out';
            setTimeout(() => {
                el.style.opacity = '1';
            }, 10);
        }
    }

    function hideLoader() {
        if (!isLoaderVisible) return;
        
        const el = document.getElementById("page-loader");
        if (el) {
            el.style.opacity = '0';
            setTimeout(() => {
                el.classList.add("d-none");
                isLoaderVisible = false;
            }, 300);
        }
    }

    function showLoaderWithDelay(delay = 500) {
        if (loaderTimeout) {
            clearTimeout(loaderTimeout);
        }
        
        loaderTimeout = setTimeout(() => {
            showLoader();
        }, delay);
    }

    function cancelLoader() {
        if (loaderTimeout) {
            clearTimeout(loaderTimeout);
            loaderTimeout = null;
        }
        hideLoader();
    }

    // Show loader on page unload
    window.addEventListener("beforeunload", () => {
        showLoader();
    });

    // Show loader on form submissions and navigation
    document.addEventListener("DOMContentLoaded", function () {
        // Handle form submissions
        document.querySelectorAll('form').forEach(form => {
            form.addEventListener("submit", (e) => {
                // Don't show loader for forms with data-no-loader attribute
                if (form.hasAttribute('data-no-loader')) return;
                
                // Validate form before showing loader
                if (!form.checkValidity()) {
                    e.preventDefault();
                    return;
                }
                
                showLoaderWithDelay(200);
            });
        });

        // Handle navigation links
        document.querySelectorAll('a[href]:not([href^="#"]):not([href^="javascript:"]):not([target="_blank"])').forEach(link => {
            link.addEventListener("click", (e) => {
                // Ignore Ctrl/Meta/Shift for new tab/window
                if (e.ctrlKey || e.metaKey || e.shiftKey) return;
                
                // Don't show loader for links with data-no-loader attribute
                if (link.hasAttribute('data-no-loader')) return;
                
                // Check if it's an external link
                const href = link.getAttribute('href');
                if (href && (href.startsWith('http') || href.startsWith('//'))) {
                    const currentDomain = window.location.hostname;
                    const linkDomain = new URL(href, window.location.href).hostname;
                    if (linkDomain !== currentDomain) return;
                }
                
                showLoaderWithDelay(300);
            });
        });

        // Handle button clicks
        document.querySelectorAll('button[type="submit"], input[type="submit"]').forEach(button => {
            button.addEventListener("click", (e) => {
                // Don't show loader for buttons with data-no-loader attribute
                if (button.hasAttribute('data-no-loader')) return;
                
                const form = button.closest('form');
                if (form && !form.checkValidity()) {
                    e.preventDefault();
                    return;
                }
                
                showLoaderWithDelay(200);
            });
        });

        // Handle page load completion
        window.addEventListener("load", () => {
            cancelLoader();
        });

        // Handle back/forward navigation
        window.addEventListener("pageshow", (event) => {
            if (event.persisted) {
                cancelLoader();
            }
        });
    });

    // Expose functions globally for manual control
    window.quizLoader = {
        show: showLoader,
        hide: hideLoader,
        showWithDelay: showLoaderWithDelay,
        cancel: cancelLoader
    };

    // Auto-hide loader after maximum time (safety net)
    setTimeout(() => {
        cancelLoader();
    }, 10000); // 10 seconds max
})();
