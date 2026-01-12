using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace DesktopLoginApp.Helpers
{
    /// <summary>
    /// Helper class to get device information like MAC address and device name
    /// </summary>
    public static class DeviceInfoHelper
    {
        /// <summary>
        /// Get the MAC address of the first active network adapter
        /// Returns format: XX:XX:XX:XX:XX:XX
        /// </summary>
        public static string? GetMacAddress()
        {
            try
            {
                var networkInterface = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(n => n.OperationalStatus == OperationalStatus.Up)
                    .Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    .Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
                    .FirstOrDefault();

                if (networkInterface != null)
                {
                    var mac = networkInterface.GetPhysicalAddress();
                    if (mac != null && mac.GetAddressBytes().Length > 0)
                    {
                        // Format as XX:XX:XX:XX:XX:XX
                        return string.Join(":", mac.GetAddressBytes().Select(b => b.ToString("X2")));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting MAC address: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Get the computer name
        /// </summary>
        public static string GetDeviceName()
        {
            try
            {
                return Environment.MachineName;
            }
            catch
            {
                return "Unknown Device";
            }
        }

        /// <summary>
        /// Get detailed device information for fingerprinting
        /// </summary>
        public static DeviceInfo GetDeviceInfo()
        {
            return new DeviceInfo
            {
                MacAddress = GetMacAddress() ?? "Unknown",
                DeviceName = GetDeviceName(),
                OSVersion = Environment.OSVersion.ToString(),
                UserName = Environment.UserName,
                ProcessorCount = Environment.ProcessorCount
            };
        }
    }

    /// <summary>
    /// Device information model
    /// </summary>
    public class DeviceInfo
    {
        public string MacAddress { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public string OSVersion { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int ProcessorCount { get; set; }

        public override string ToString()
        {
            return $"Device: {DeviceName}, MAC: {MacAddress}, OS: {OSVersion}, User: {UserName}";
        }
    }
}
