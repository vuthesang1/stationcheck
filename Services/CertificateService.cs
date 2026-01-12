using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using StationCheck.Interfaces;
using Microsoft.Extensions.Logging;

namespace StationCheck.Services;

public class CertificateService : ICertificateService
{
    private readonly ILogger<CertificateService> _logger;

    // Client Authentication EKU OID: 1.3.6.1.5.5.7.3.2
    private const string ClientAuthEKU = "1.3.6.1.5.5.7.3.2";

    public CertificateService(ILogger<CertificateService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Extract certificate information from base64-encoded certificate data
    /// </summary>
    public async Task<CertificateInfo> ExtractCertificateInfoAsync(string certificateData)
    {
        try
        {
            return await Task.Run(() =>
            {
                // Decode base64 certificate data
                byte[] certificateBytes = Convert.FromBase64String(certificateData);

                // Create X509Certificate2 object
                using (var certificate = new X509Certificate2(certificateBytes))
                {
                    // Extract thumbprint
                    string thumbprint = certificate.Thumbprint ?? string.Empty;

                    // Extract subject
                    string subject = certificate.SubjectName.Name ?? string.Empty;

                    // Extract issuer
                    string? issuer = certificate.IssuerName.Name;

                    // Extract validity dates
                    DateTime validFrom = certificate.NotBefore;
                    DateTime validTo = certificate.NotAfter;

                    // Extract and parse EKU
                    string? ekuOids = ExtractEKUOids(certificate);
                    bool hasClientAuthEKU = ekuOids?.Contains(ClientAuthEKU) ?? false;

                    _logger.LogInformation(
                        "Certificate extracted: Thumbprint={Thumbprint}, Subject={Subject}, ValidFrom={ValidFrom}, ValidTo={ValidTo}, HasClientAuthEKU={HasClientAuthEKU}",
                        thumbprint, subject, validFrom, validTo, hasClientAuthEKU);

                    return new CertificateInfo
                    {
                        Thumbprint = thumbprint,
                        Subject = subject,
                        Issuer = issuer,
                        ValidFrom = validFrom,
                        ValidTo = validTo,
                        EKUOids = ekuOids,
                        HasClientAuthEKU = hasClientAuthEKU
                    };
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting certificate information");
            throw new InvalidOperationException("Failed to extract certificate information", ex);
        }
    }

    /// <summary>
    /// Extract EKU OIDs from certificate
    /// </summary>
    private string? ExtractEKUOids(X509Certificate2 certificate)
    {
        try
        {
            var ekuExtension = certificate.Extensions.OfType<X509EnhancedKeyUsageExtension>().FirstOrDefault();
            if (ekuExtension == null)
                return null;

            var oids = ekuExtension.EnhancedKeyUsages.Cast<Oid>().Select(o => o.Value);
            return string.Join(",", oids);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error extracting EKU OIDs from certificate");
            return null;
        }
    }

    /// <summary>
    /// Validate if certificate has required EKU (Client Authentication)
    /// </summary>
    public bool ValidateEKU(string ekuOids)
    {
        if (string.IsNullOrWhiteSpace(ekuOids))
            return false;

        return ekuOids.Contains(ClientAuthEKU);
    }

    /// <summary>
    /// Validate certificate thumbprint format (should be 40 hex characters for SHA1)
    /// </summary>
    public bool ValidateThumbprint(string thumbprint)
    {
        if (string.IsNullOrWhiteSpace(thumbprint))
            return false;

        // Thumbprint should be 40 characters (SHA1) or 64 characters (SHA256)
        if (thumbprint.Length != 40 && thumbprint.Length != 64)
            return false;

        // Should be hex characters only
        return thumbprint.All(c => (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'));
    }
}
