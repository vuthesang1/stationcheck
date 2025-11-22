// Toast notification helper using Bootstrap Toast
window.showToast = function (message, type = 'success', duration = 3000) {
    // Create toast container if not exists
    let toastContainer = document.getElementById('toast-container');
    if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.id = 'toast-container';
        toastContainer.className = 'toast-container position-fixed top-0 end-0 p-3';
        toastContainer.style.zIndex = '9999';
        document.body.appendChild(toastContainer);
    }

    // Create toast element
    const toastId = 'toast-' + Date.now();
    const bgClass = type === 'success' ? 'bg-success' : 
                   type === 'error' ? 'bg-danger' : 
                   type === 'warning' ? 'bg-warning' : 
                   'bg-info';
    
    const icon = type === 'success' ? 'bi-check-circle-fill' : 
                 type === 'error' ? 'bi-x-circle-fill' : 
                 type === 'warning' ? 'bi-exclamation-triangle-fill' : 
                 'bi-info-circle-fill';

    const toastHtml = `
        <div id="${toastId}" class="toast align-items-center text-white ${bgClass} border-0" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">
                    <i class="bi ${icon} me-2"></i>
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    `;

    toastContainer.insertAdjacentHTML('beforeend', toastHtml);

    const toastElement = document.getElementById(toastId);
    const bsToast = new bootstrap.Toast(toastElement, {
        autohide: true,
        delay: duration
    });

    // Remove toast element after it's hidden
    toastElement.addEventListener('hidden.bs.toast', function () {
        toastElement.remove();
    });

    bsToast.show();
};

// Fullscreen toggle helper for StationMonitor
window.toggleFullscreen = function (isFullscreen) {
    if (isFullscreen) {
        // Hide layout elements
        const sidebar = document.getElementById('accordionSidebar');
        const topbar = document.querySelector('.topbar');
        const pageHeading = document.querySelector('.d-sm-flex.align-items-center');
        
        if (sidebar) sidebar.style.display = 'none';
        if (topbar) topbar.style.display = 'none';
        if (pageHeading) pageHeading.style.display = 'none';
        
        // Make body overflow hidden
        document.body.style.overflow = 'hidden';
    } else {
        // Show layout elements
        const sidebar = document.getElementById('accordionSidebar');
        const topbar = document.querySelector('.topbar');
        const pageHeading = document.querySelector('.d-sm-flex.align-items-center');
        
        if (sidebar) sidebar.style.display = '';
        if (topbar) topbar.style.display = '';
        if (pageHeading) pageHeading.style.display = '';
        
        // Reset body overflow
        document.body.style.overflow = '';
    }
};
