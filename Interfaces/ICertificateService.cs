namespace StationCheck.Interfaces;

public interface ICertificateService
{
    /// <summary>
    /// Extract certificate information from certificate data (base64 or raw bytes)
    /// </summary>
    Task<CertificateInfo> ExtractCertificateInfoAsync(string certificateData);

    /// <summary>
    /// Validate if certificate has required EKU (Enhanced Key Usage)
    /// </summary>
    bool ValidateEKU(string ekuOids);

    /// <summary>
    /// Validate certificate thumbprint format
    /// </summary>
    bool ValidateThumbprint(string thumbprint);
}

public class CertificateInfo
{
    public string Thumbprint { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string? Issuer { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public string? EKUOids { get; set; }
    public bool HasClientAuthEKU { get; set; }
}
