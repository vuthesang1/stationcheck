// Device Status Monitor - Real-time notifications for device changes
// Connects to SignalR hub and handles force logout when device is removed/disabled

let deviceStatusConnection = null;
let reconnectAttempts = 0;
const MAX_RECONNECT_ATTEMPTS = 5;

// Initialize device status monitoring
window.initDeviceStatusMonitor = async function() {
    try {
        console.log('[DeviceStatusMonitor] Initializing...');
        
        // Create SignalR connection
        deviceStatusConnection = new signalR.HubConnectionBuilder()
            .withUrl('/hubs/device-status')
            .withAutomaticReconnect({
                nextRetryDelayInMilliseconds: retryContext => {
                    if (retryContext.previousRetryCount >= MAX_RECONNECT_ATTEMPTS) {
                        return null; // Stop reconnecting
                    }
                    return Math.min(1000 * Math.pow(2, retryContext.previousRetryCount), 30000);
                }
            })
            .configureLogging(signalR.LogLevel.Information)
            .build();

        // Handle device status change notifications
        deviceStatusConnection.on('DeviceStatusChanged', function(notification) {
            console.warn('[DeviceStatusMonitor] Device status changed:', notification);
            
            // Show beautiful popup alert using SweetAlert2
            if (notification.message) {
                if (window.Swal) {
                    Swal.fire({
                        icon: 'warning',
                        title: 'Thông báo thiết bị',
                        text: notification.message,
                        confirmButtonText: 'OK',
                        allowOutsideClick: false
                    });
                } else {
                    alert(notification.message);
                }
            }
            
            // Force logout if required
            if (notification.requiresLogout) {
                console.log('[DeviceStatusMonitor] Forcing logout due to:', notification.reason);
                forceLogoutAndRedirect(notification.message);
            }
        });

        // Handle reconnection events
        deviceStatusConnection.onreconnecting(error => {
            console.warn('[DeviceStatusMonitor] Connection lost. Reconnecting...', error);
            reconnectAttempts++;
        });

        deviceStatusConnection.onreconnected(connectionId => {
            console.log('[DeviceStatusMonitor] Reconnected. Connection ID:', connectionId);
            reconnectAttempts = 0;
        });

        deviceStatusConnection.onclose(error => {
            console.error('[DeviceStatusMonitor] Connection closed:', error);
            
            if (reconnectAttempts >= MAX_RECONNECT_ATTEMPTS) {
                console.error('[DeviceStatusMonitor] Max reconnection attempts reached. Please refresh the page.');
            }
        });

        // Start the connection
        await deviceStatusConnection.start();
        console.log('[DeviceStatusMonitor] ✅ Connected successfully');
        
        return true;
    } catch (error) {
        console.error('[DeviceStatusMonitor] ❌ Failed to initialize:', error);
        return false;
    }
};

// Disconnect from device status monitoring
window.disconnectDeviceStatusMonitor = async function() {
    if (deviceStatusConnection) {
        try {
            await deviceStatusConnection.stop();
            console.log('[DeviceStatusMonitor] Disconnected');
        } catch (error) {
            console.error('[DeviceStatusMonitor] Error disconnecting:', error);
        }
    }
};

// Force logout and redirect to login
function forceLogoutAndRedirect(message) {
    // Call logout API (don't wait for response)
    fetch('/api/auth/logout', {
        method: 'POST',
        credentials: 'same-origin'
    }).catch(err => console.error('Logout error:', err));
    
    // Store message to display on login page
    if (message) {
        sessionStorage.setItem('logoutMessage', message);
    }
    
    // Redirect to login page with force reload
    window.location.href = '/login?reason=device_removed';
}

// Check if we have a logout message to display
window.getLogoutMessage = function() {
    const message = sessionStorage.getItem('logoutMessage');
    if (message) {
        sessionStorage.removeItem('logoutMessage');
        return message;
    }
    return null;
};
