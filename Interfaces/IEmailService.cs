using StationCheck.Models;

namespace StationCheck.Interfaces
{
    public interface IEmailService
    {
        /// <summary>
        /// Check email server for new emails and process them into MotionEvents and EmailEvents
        /// </summary>
        Task<List<MotionEvent>> CheckAndProcessNewEmailsAsync();
        
        /// <summary>
        /// Parse email content to MotionEvent object
        /// </summary>
        Task<MotionEvent?> ParseEmailToMotionEventAsync(string subject, string body, string from, DateTime receivedAt, string messageId);
        
        /// <summary>
        /// Save MotionEvent to database and update Station.LastMotionDetectedAt
        /// </summary>
        Task SaveMotionEventAsync(MotionEvent motionEvent);
    }
}
