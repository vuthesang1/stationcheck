using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DesktopLoginApp.Helpers;
using DesktopLoginApp.Services;

namespace DesktopLoginApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Guid? _currentDeviceId;
    private DeviceInfo? _deviceInfo;

    public MainWindow()
    {
        InitializeComponent();
        
        // Set focus to username textbox
        txtUsername.Focus();
        
        // Add Enter key handler for password box
        txtPassword.KeyDown += (s, e) =>
        {
            if (e.Key == Key.Enter)
            {
                BtnLogin_Click(btnLogin, null!);
            }
        };
        
        // Load saved credentials if "Remember Me" was checked
        LoadSavedCredentials();
    }

    private void LoadSavedCredentials()
    {
        try
        {
            var tokenData = TokenManager.LoadToken();
            if (tokenData != null)
            {
                txtUsername.Text = tokenData.Username;
                chkRememberMe.IsChecked = true;
                FileLogger.Log($"Loaded saved credentials for: {tokenData.Username}");
            }
        }
        catch (Exception ex)
        {
            FileLogger.LogError("Failed to load saved credentials", ex);
        }
    }

    private async void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(txtUsername.Text))
        {
            ShowError("Vui lòng nhập tên đăng nhập");
            txtUsername.Focus();
            return;
        }

        if (string.IsNullOrEmpty(txtPassword.Password))
        {
            ShowError("Vui lòng nhập mật khẩu");
            txtPassword.Focus();
            return;
        }

        // Show loading
        SetLoading(true);
        HideError();
        HideDeviceStatus();

        try
        {
            // Get device information
            _deviceInfo = DeviceInfoHelper.GetDeviceInfo();
            
            if (string.IsNullOrEmpty(_deviceInfo.MacAddress) || _deviceInfo.MacAddress == "Unknown")
            {
                ShowError("Không thể lấy thông tin MAC address của thiết bị này.\nVui lòng kiểm tra kết nối mạng.");
                return;
            }

            using var apiClient = new StationCheckApiClient(AppConfig.API_BASE_URL);
            
            // Try to login first
            var loginResponse = await apiClient.LoginAsync(
                txtUsername.Text.Trim(),
                txtPassword.Password,
                _deviceInfo.MacAddress
            );

            if (loginResponse.Success && !string.IsNullOrEmpty(loginResponse.AccessToken))
            {
                // Login successful - Save token
                FileLogger.Log("Login successful, saving token...");
                
                var tokenData = new TokenManager.TokenData
                {
                    AccessToken = loginResponse.AccessToken,
                    RefreshToken = loginResponse.RefreshToken ?? string.Empty,
                    UserId = loginResponse.User?.Id ?? string.Empty,
                    Username = loginResponse.User?.Username ?? txtUsername.Text.Trim(),
                    FullName = loginResponse.User?.FullName ?? string.Empty,
                    Email = loginResponse.User?.Email ?? string.Empty,
                    Role = loginResponse.User?.Role ?? 0,
                    MacAddress = _deviceInfo.MacAddress,
                    SavedAt = DateTime.Now
                };

                // Save token if "Remember Me" is checked
                if (chkRememberMe.IsChecked == true)
                {
                    TokenManager.SaveToken(tokenData);
                    FileLogger.Log("Token saved to secure storage");
                }
                else
                {
                    TokenManager.ClearToken();
                    FileLogger.Log("Token not saved (Remember Me unchecked)");
                }

                ShowSuccess($"Đăng nhập thành công!\nChào mừng {loginResponse.User?.FullName ?? loginResponse.User?.Username ?? "User"}");
                
                await Task.Delay(1500);
                
                // Request desktop login token from API
                FileLogger.Log("Requesting desktop login token for browser...");
                var tokenResponse = await apiClient.RequestDesktopLoginTokenAsync(loginResponse.AccessToken);
                
                if (tokenResponse.Success && !string.IsNullOrEmpty(tokenResponse.Token))
                {
                    FileLogger.Log("Desktop token received, redirecting to browser...");
                    BrowserRedirectHelper.RedirectToBrowser(tokenResponse.Token);
                    
                    // Close desktop app after redirect
                    await Task.Delay(1000);
                    FileLogger.Log("Closing desktop application");
                    Application.Current.Shutdown();
                }
                else
                {
                    ShowError($"Không thể tạo token đăng nhập:\n{tokenResponse.Message}");
                    FileLogger.LogError("Failed to create desktop login token", new Exception(tokenResponse.Message));
                }
            }
            else if (loginResponse.RequiresDeviceRegistration ||
                     loginResponse.Message.Contains("chưa được đăng ký") || 
                     loginResponse.Message.Contains("not found"))
            {
                // Device not registered - auto register
                ShowInfo("Thiết bị chưa đăng ký. Đang tự động đăng ký...");
                await Task.Delay(1000);

                var registerResponse = await apiClient.RegisterDeviceAsync(
                    txtUsername.Text.Trim(),
                    txtPassword.Password,
                    _deviceInfo.MacAddress,
                    _deviceInfo.DeviceName
                );

                if (registerResponse.Success)
                {
                    _currentDeviceId = registerResponse.DeviceId;
                    ShowDeviceStatus(registerResponse.Status, registerResponse.Message);
                }
                else
                {
                    ShowError($"Đăng ký thiết bị thất bại!\n\n{registerResponse.Message}");
                }
            }
            else if (loginResponse.Message.Contains("chưa được phê duyệt") ||
                     loginResponse.Message.Contains("not approved"))
            {
                // Device pending approval
                ShowInfo("Thiết bị đang chờ phê duyệt...");
                ShowDeviceStatus(DeviceStatus.PendingApproval, loginResponse.Message);
            }
            else if (loginResponse.Message.Contains("chưa được phân quyền") ||
                     loginResponse.Message.Contains("not assigned"))
            {
                ShowError($"Thiết bị đã được phê duyệt nhưng bạn chưa được phân quyền.\n\n{loginResponse.Message}");
            }
            else
            {
                ShowError(loginResponse.Message);
            }
        }
        catch (Exception ex)
        {
            ShowError($"Lỗi kết nối:\n{ex.Message}");
        }
        finally
        {
            SetLoading(false);
        }
    }

    private async void BtnRefreshStatus_Click(object sender, RoutedEventArgs e)
    {
        if (!_currentDeviceId.HasValue)
        {
            ShowError("Không có thông tin thiết bị để kiểm tra");
            return;
        }

        SetLoading(true);
        HideError();

        try
        {
            using var apiClient = new StationCheckApiClient(AppConfig.API_BASE_URL);
            var statusResponse = await apiClient.GetDeviceStatusAsync(_currentDeviceId.Value);

            if (statusResponse.Success)
            {
                ShowDeviceStatus(statusResponse.Status, statusResponse.Message);

                // If approved and assigned, prompt to login
                if (statusResponse.Status == DeviceStatus.Approved && statusResponse.IsUserAssigned)
                {
                    var result = MessageBox.Show(
                        "Thiết bị đã được phê duyệt và bạn đã được phân quyền!\n\nBạn có muốn đăng nhập ngay không?",
                        "Thông báo",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information
                    );

                    if (result == MessageBoxResult.Yes)
                    {
                        BtnLogin_Click(btnLogin, null!);
                    }
                }
            }
            else
            {
                ShowError($"Không thể kiểm tra trạng thái:\n{statusResponse.Message}");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Lỗi:\n{ex.Message}");
        }
        finally
        {
            SetLoading(false);
        }
    }

    private void SetLoading(bool isLoading)
    {
        btnLogin.IsEnabled = !isLoading;
        txtUsername.IsEnabled = !isLoading;
        txtPassword.IsEnabled = !isLoading;
        chkRememberMe.IsEnabled = !isLoading;
        btnRefreshStatus.IsEnabled = !isLoading;
        
        pnlLoading.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
    }

    private void ShowError(string message)
    {
        txtStatus.Text = message;
        txtStatus.Foreground = new SolidColorBrush(Colors.Red);
        txtStatus.Visibility = Visibility.Visible;
    }

    private void ShowSuccess(string message)
    {
        txtStatus.Text = message;
        txtStatus.Foreground = new SolidColorBrush(Color.FromRgb(40, 167, 69)); // Green
        txtStatus.Visibility = Visibility.Visible;
    }

    private void ShowInfo(string message)
    {
        txtStatus.Text = message;
        txtStatus.Foreground = new SolidColorBrush(Color.FromRgb(0, 123, 255)); // Blue
        txtStatus.Visibility = Visibility.Visible;
    }

    private void HideError()
    {
        txtStatus.Visibility = Visibility.Collapsed;
    }

    private void ShowDeviceStatus(DeviceStatus status, string message)
    {
        if (_deviceInfo == null) return;

        txtDeviceName.Text = _deviceInfo.DeviceName;
        txtMacAddress.Text = _deviceInfo.MacAddress;
        
        switch (status)
        {
            case DeviceStatus.PendingApproval:
                txtDeviceStatusText.Text = "⏳ Chờ phê duyệt";
                txtDeviceStatusText.Foreground = new SolidColorBrush(Color.FromRgb(255, 193, 7)); // Orange
                ShowInfo(message);
                break;
            case DeviceStatus.Approved:
                txtDeviceStatusText.Text = "✅ Đã phê duyệt";
                txtDeviceStatusText.Foreground = new SolidColorBrush(Color.FromRgb(40, 167, 69)); // Green
                ShowSuccess(message);
                break;
            case DeviceStatus.Rejected:
                txtDeviceStatusText.Text = "❌ Đã từ chối";
                txtDeviceStatusText.Foreground = new SolidColorBrush(Colors.Red);
                ShowError(message);
                break;
        }

        pnlDeviceStatus.Visibility = Visibility.Visible;
    }

    private void HideDeviceStatus()
    {
        pnlDeviceStatus.Visibility = Visibility.Collapsed;
    }
}