using StationCheck.Models;

namespace StationCheck.Interfaces;

/// <summary>
/// Interface cho Motion Detection service
/// </summary>
public interface IMotionDetectionService
{
    /// <summary>
    /// Xử lý motion event từ camera/NVR
    /// </summary>
    Task<MotionEvent> ProcessMotionEventAsync(string cameraId, string eventType, string? payload = null);
    
    /// <summary>
    /// Kiểm tra và tạo alerts nếu cần
    /// </summary>
    Task CheckAndCreateAlertsAsync();
    
    /// <summary>
    /// Lấy danh sách alerts chưa resolve
    /// </summary>
    Task<List<MotionAlert>> GetUnresolvedAlertsAsync();
    
    /// <summary>
    /// Resolve một alert
    /// </summary>
    Task<bool> ResolveAlertAsync(string alertId, string resolvedBy, string? notes = null);
    
    /// <summary>
    /// Lấy motion events theo camera và khoảng thời gian
    /// </summary>
    Task<List<MotionEvent>> GetMotionEventsAsync(string? cameraId = null, DateTime? fromDate = null, DateTime? toDate = null);
    
    /// <summary>
    /// Lấy alert rule hiện tại cho camera
    /// </summary>
    Task<AlertRule?> GetCurrentAlertRuleAsync(string cameraId, TimeSpan currentTime);
    
    /// <summary>
    /// Lấy thống kê motion events hôm nay
    /// </summary>
    Task<Dictionary<string, int>> GetTodayMotionStatsAsync();
}
