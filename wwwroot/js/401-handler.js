// Global 401 Unauthorized Handler
// Intercepts 401 responses and shows user-friendly popup alerts

(function() {
    'use strict';

    console.log('[401Handler] Initializing global 401 handler...');

    // Store original fetch
    const originalFetch = window.fetch;

    // Override fetch to intercept 401 responses
    window.fetch = async function(...args) {
        try {
            const response = await originalFetch.apply(this, args);
            
            // Check for 401 Unauthorized
            if (response.status === 401) {
                await handle401Response(response.clone());
            }
            
            return response;
        } catch (error) {
            throw error;
        }
    };

    // Handle 401 responses
    async function handle401Response(response) {
        try {
            // Try to parse JSON body
            const contentType = response.headers.get('content-type');
            if (contentType && contentType.includes('application/json')) {
                const data = await response.json();
                
                console.warn('[401Handler] Unauthorized access:', data);
                
                // Show user-friendly popup
                const message = data.message || 'Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.';
                
                // Use SweetAlert if available, otherwise use alert
                if (window.Swal) {
                    await Swal.fire({
                        icon: 'warning',
                        title: 'Truy cập bị từ chối',
                        text: message,
                        confirmButtonText: 'Đăng nhập lại',
                        allowOutsideClick: false
                    });
                } else {
                    alert(message);
                }
                
                // Force logout if required
                if (data.requiresLogout) {
                    console.log('[401Handler] Forcing logout...');
                    
                    // Call logout API
                    try {
                        await originalFetch('/api/auth/logout', {
                            method: 'POST',
                            credentials: 'same-origin'
                        });
                    } catch (err) {
                        console.error('[401Handler] Logout error:', err);
                    }
                    
                    // Store message for login page
                    sessionStorage.setItem('logoutMessage', message);
                    
                    // Redirect to login
                    window.location.href = '/login?reason=unauthorized';
                }
            }
        } catch (error) {
            console.error('[401Handler] Error handling 401 response:', error);
        }
    }

    // Also intercept XMLHttpRequest for older code
    const originalOpen = XMLHttpRequest.prototype.open;
    const originalSend = XMLHttpRequest.prototype.send;

    XMLHttpRequest.prototype.open = function(method, url, ...rest) {
        this._url = url;
        return originalOpen.apply(this, [method, url, ...rest]);
    };

    XMLHttpRequest.prototype.send = function(...args) {
        this.addEventListener('load', function() {
            if (this.status === 401) {
                try {
                    const data = JSON.parse(this.responseText);
                    const message = data.message || 'Phiên đăng nhập đã hết hạn.';
                    
                    if (window.Swal) {
                        Swal.fire({
                            icon: 'warning',
                            title: 'Truy cập bị từ chối',
                            text: message,
                            confirmButtonText: 'OK'
                        });
                    } else {
                        alert(message);
                    }
                    
                    if (data.requiresLogout) {
                        sessionStorage.setItem('logoutMessage', message);
                        setTimeout(() => {
                            window.location.href = '/login?reason=unauthorized';
                        }, 1500);
                    }
                } catch (e) {
                    console.error('[401Handler] Error parsing XHR response:', e);
                }
            }
        });
        
        return originalSend.apply(this, args);
    };

    console.log('[401Handler] ✅ Global 401 handler initialized');
})();
