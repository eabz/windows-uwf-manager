using System;
using System.Windows;
using System.Windows.Media;
using UwfManager.Services;

namespace UwfManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly UwfService _uwfService;
        private bool _isCurrentEnabled;

        private System.Windows.Forms.NotifyIcon _notifyIcon;

        public MainWindow()
        {
            InitializeComponent();
            _uwfService = new UwfService();
            InitializeTrayIcon();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        private void InitializeTrayIcon()
        {
            try 
            {
                _notifyIcon = new System.Windows.Forms.NotifyIcon
                {
                    Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location),
                    Visible = true,
                    Text = "UWF Manager"
                };
                _notifyIcon.DoubleClick += (s, e) => 
                {
                    Show();
                    WindowState = WindowState.Normal;
                };
            }
            catch (Exception ex)
            {
                // Non-fatal, just log or ignore if tray icon fails (e.g. icon resource missing)
                MessageBox.Show($"Warning: Could not create tray icon. {ex.Message}", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Optional: Minimize to try instead of close? 
            // For now, let's just cleanup
            _notifyIcon.Dispose();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsAdministrator())
            {
                MessageBox.Show("This application requires Administrator privileges.\nPlease restart as Administrator.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Check if password exists (First Run)
            var auth = new PasswordService();
            if (!auth.IsPasswordSet())
            {
                var setup = new PasswordWindow(isSettingNew: true);
                if (setup.ShowDialog() != true)
                {
                    // If they cancel init setup, close app
                    Close();
                    return;
                }
            }
            
            RefreshStatus();
        }

        private static bool IsAdministrator()
        {
            using var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }

        private void RefreshStatus()
        {
            try
            {
                Log("Checking UWF status...");
                var status = _uwfService.GetStatus(); 
                _isCurrentEnabled = status.IsEnabled;

                Dispatcher.Invoke(() =>
                {
                    if (_isCurrentEnabled)
                    {
                        StatusText.Text = "System Locked (UWF Active)";
                        StatusIndicator.Fill = new SolidColorBrush(Colors.Green);
                        BtnToggle.Content = "Unlock System (Disable UWF)";
                        BtnToggle.Background = new SolidColorBrush(Color.FromRgb(220, 53, 69)); // Red for unlock
                        _notifyIcon.Text = "UWF Manager: Protected";
                    }
                    else
                    {
                        StatusText.Text = "System Unlocked (Editable)";
                        StatusIndicator.Fill = new SolidColorBrush(Colors.Red);
                        BtnToggle.Content = "Lock System (Enable UWF)";
                        BtnToggle.Background = new SolidColorBrush(Color.FromRgb(40, 167, 69)); // Green for lock
                        _notifyIcon.Text = "UWF Manager: Unprotected";
                    }
                    
                    Log($"Status check complete. IsEnabled: {_isCurrentEnabled}");
                });
            }

            catch (Exception ex)
            {
                Log($"Error checking status: {ex.Message}");
            }
        }

        private void BtnToggle_Click(object sender, RoutedEventArgs e)
        {
            // Verify Password before Action
            var auth = new PasswordWindow(isSettingNew: false);
            auth.Owner = this;
            if (auth.ShowDialog() != true)
            {
                Log("Action cancelled: Authentication failed.");
                return;
            }

            try
            {
                if (_isCurrentEnabled)
                {
                    Log("Disabling UWF...");
                    _uwfService.DisableInfo(); // Send command
                }
                else
                {
                    Log("Enabling UWF...");
                    _uwfService.EnableInfo(); // Send command
                }
                
                Log("Command sent. A restart is required to apply changes.");
                BtnRestart.Visibility = Visibility.Visible;
                
                // Optimistically update UI or re-check? 
                // UWF status usually requires reboot to change "Current" status vs "Next Session" status.
                // Our simple check might look at current config.
                // Let's prompt user to restart.
                MessageBox.Show("Configuration updated. Please restart the system to apply changes.", "Restart Required", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Log($"Error toggling UWF: {ex.Message}");
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRestart_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to restart now?", "Confirm Restart", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Log("Restarting system...");
                _uwfService.Restart();
            }
        }

        private void Log(string message)
        {
            Dispatcher.Invoke(() =>
            {
                TxtLog.Text += $"\n[{DateTime.Now:HH:mm:ss}] {message}";
                // Auto scroll
                if (VisualTreeHelper.GetChild(TxtLog.Parent as DependencyObject, 0) is System.Windows.Controls.ScrollViewer scrollViewer)
                {
                    scrollViewer.ScrollToBottom();
                }
            });
        }
    }
}
