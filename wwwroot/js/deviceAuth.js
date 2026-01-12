// Device Auth Manager - Handles device validation and force logout
window.DeviceAuthManager = {
    connection: null,
    isConnected: false,
    reconnectAttempts: 0,
    maxReconnectAttempts: 5,
    validationIntervalId: null,

    // Initialize SignalR connection
    async init() {
        console.log('[DeviceAuth] Initializing...');

        try {
            // Create SignalR connection
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl("/hubs/auth")
                .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
                .configureLogging(signalR.LogLevel.Information)
                .build();

            // Register event handlers
            this.connection.on("ForceLogout", (data) => {
                console.warn('[DeviceAuth] 🔥 Force logout received:', data);
                this.handleForceLogout(data);
            });

            this.connection.on("RegistrationConfirmed", (data) => {
                console.log('[DeviceAuth] ✅ Registration confirmed:', data);
            });

            this.connection.onreconnecting((error) => {
                console.warn('[DeviceAuth] ⚠️ Reconnecting...', error);
            });

            this.connection.onreconnected((connectionId) => {
                console.log('[DeviceAuth] ✅ Reconnected. ConnectionId:', connectionId);
                this.reconnectAttempts = 0;
                this.isConnected = true;
            });

            this.connection.onclose((error) => {
                console.error('[DeviceAuth] ❌ Connection closed:', error);
                this.isConnected = false;
            });

            // Start connection
            await this.connection.start();
            this.isConnected = true;
            console.log('[DeviceAuth] ✅ Connected to AuthHub');

            // Register for notifications
            await this.connection.invoke("Register");

            // Start periodic validation (every 30 seconds)
            this.startPeriodicValidation();

        } catch (error) {
            console.error('[DeviceAuth] ❌ Failed to connect:', error);
            this.scheduleReconnect();
        }
    },

    // Handle force logout notification
    handleForceLogout(data) {
        const { reason, message, timestamp } = data;

        console.warn('[DeviceAuth] 🔥 Force logout triggered');
        console.warn(`  Reason: ${reason}`);
        console.warn(`  Message: ${message}`);
        console.warn(`  Time: ${timestamp}`);

        // Show alert to user
        alert(`⚠️ THÔNG BÁO ĐĂNG XUẤT\n\n${message}\n\nBạn sẽ bị đăng xuất ngay lập tức.`);

        // Clear local storage
        if (window.localStorage) {
            localStorage.removeItem('authToken');
            localStorage.removeItem('loginMethod');
        }

        if (window.sessionStorage) {
            sessionStorage.removeItem('authToken');
        }

        // Redirect to login
        window.location.href = '/login?reason=' + encodeURIComponent(reason);
    },

    // Periodic device access validation
    startPeriodicValidation() {
        // Validate immediately
        this.validateDeviceAccess();

        // Then validate every 30 seconds
        this.validationIntervalId = setInterval(() => {
            this.validateDeviceAccess();
        }, 30000); // 30 seconds

        console.log('[DeviceAuth] ⏱️ Periodic validation started (30s interval)');
    },

    // Validate device access via API
    async validateDeviceAccess() {
        try {
            const response = await fetch('/api/devicevalidation/validate-device-access', {
                method: 'GET',
                headers: {
                    'Authorization': 'Bearer ' + this.getAuthToken(),
                    'Content-Type': 'application/json'
                }
            });

            if (response.status === 401) {
                console.error('[DeviceAuth] ❌ Unauthorized - Token expired or invalid');
                this.handleInvalidAccess('TokenExpired', 'Token đã hết hạn hoặc không hợp lệ');
                return;
            }

            if (!response.ok) {
                console.error('[DeviceAuth] ❌ Validation failed:', response.status);
                return;
            }

            const data = await response.json();
            
            if (!data.isValid) {
                console.warn('[DeviceAuth] ⚠️ Device access invalid:', data);
                this.handleInvalidAccess(data.reason, data.message);
            } else {
                console.log('[DeviceAuth] ✅ Device access valid');
            }

        } catch (error) {
            console.error('[DeviceAuth] ❌ Validation error:', error);
        }
    },

    // Handle invalid access (logout)
    handleInvalidAccess(reason, message) {
        console.warn('[DeviceAuth] 🔥 Invalid access detected:', reason);

        // Stop validation timer
        if (this.validationIntervalId) {
            clearInterval(this.validationIntervalId);
            this.validationIntervalId = null;
        }

        // Show alert
        alert(`⚠️ QUYỀN TRUY CẬP BỊ THU HỒI\n\n${message}\n\nBạn sẽ bị đăng xuất.`);

        // Clear tokens
        if (window.localStorage) {
            localStorage.removeItem('authToken');
            localStorage.removeItem('loginMethod');
        }

        if (window.sessionStorage) {
            sessionStorage.removeItem('authToken');
        }

        // Disconnect SignalR
        if (this.connection) {
            this.connection.stop();
        }

        // Redirect to login
        window.location.href = '/login?reason=' + encodeURIComponent(reason);
    },

    // Get auth token from storage
    getAuthToken() {
        return sessionStorage.getItem('authToken') || 
               localStorage.getItem('authToken') || 
               '';
    },

    // Schedule reconnect attempt
    scheduleReconnect() {
        if (this.reconnectAttempts >= this.maxReconnectAttempts) {
            console.error('[DeviceAuth] ❌ Max reconnect attempts reached');
            return;
        }

        this.reconnectAttempts++;
        const delay = Math.min(1000 * Math.pow(2, this.reconnectAttempts), 30000);
        
        console.log(`[DeviceAuth] ⏱️ Reconnecting in ${delay}ms (attempt ${this.reconnectAttempts})`);
        
        setTimeout(() => {
            this.init();
        }, delay);
    },

    // Stop validation and disconnect
    stop() {
        console.log('[DeviceAuth] Stopping...');

        if (this.validationIntervalId) {
            clearInterval(this.validationIntervalId);
            this.validationIntervalId = null;
        }

        if (this.connection) {
            this.connection.stop();
            this.connection = null;
        }

        this.isConnected = false;
    }
};

// Auto-initialize on page load (only if user is authenticated)
document.addEventListener('DOMContentLoaded', () => {
    const token = sessionStorage.getItem('authToken') || localStorage.getItem('authToken');
    
    if (token && token.length > 0) {
        console.log('[DeviceAuth] Token found, initializing...');
        setTimeout(() => {
            window.DeviceAuthManager.init();
        }, 1000); // Delay 1s to ensure SignalR library is loaded
    } else {
        console.log('[DeviceAuth] No token found, skipping initialization');
    }
});
