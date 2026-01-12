// File download helper
function downloadFile(bytes, fileName) {
    const blob = new Blob([bytes], { type: 'application/octet-stream' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
}

// Logout helper - uses fetch to properly handle cookies
window.logoutWithFetch = async function() {
    try {
        const response = await fetch('/api/auth/logout', {
            method: 'POST',
            credentials: 'same-origin' // Important: Include cookies
        });
        
        const data = await response.json();
        
        // ✅ FIX: Clear all auth-related storage
        // Clear localStorage
        localStorage.clear();
        
        // Clear sessionStorage
        sessionStorage.clear();
        
        // ✅ FIX: Manually clear all cookies (in case server-side deletion fails)
        const cookies = document.cookie.split(";");
        for (let i = 0; i < cookies.length; i++) {
            const cookie = cookies[i];
            const eqPos = cookie.indexOf("=");
            const name = eqPos > -1 ? cookie.substr(0, eqPos).trim() : cookie.trim();
            
            // Delete cookie for all possible paths
            document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 GMT;path=/";
            document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 GMT;path=/;domain=" + window.location.hostname;
        }
        
        console.log('[Logout] Cleared all cookies and storage');
        
        // ✅ If server returns forceReload flag, do a hard refresh
        if (data.forceReload) {
            // Use location.replace to prevent back button issues
            window.location.replace('/login');
        }
    } catch (error) {
        console.error('[Logout] Error:', error);
        // Even if logout API fails, still clear everything and redirect
        localStorage.clear();
        sessionStorage.clear();
        window.location.replace('/login');
    }
};
