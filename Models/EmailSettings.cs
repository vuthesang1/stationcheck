namespace StationCheck.Models
{
    public class EmailSettings
    {
        public string ImapServer { get; set; } = string.Empty;
        public int ImapPort { get; set; } = 993;
        public bool UseSsl { get; set; } = true;
        public string EmailAddress { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int CheckIntervalMinutes { get; set; } = 1;
        public bool MarkAsRead { get; set; } = true;
        public bool DeleteAfterProcessing { get; set; } = false;
    }
}
