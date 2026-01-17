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

        public MainWindow()
        {
            InitializeComponent();
            _uwfService = new UwfService();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshStatus();
        }

        private void RefreshStatus()
        {
            try
            {
                Log("Checking UWF status...");
                var status = _uwfService.GetStatus(); // This performs the check
                _isCurrentEnabled = status.IsEnabled;

                // Update UI on correct thread
                Dispatcher.Invoke(() =>
                {
                    if (_isCurrentEnabled)
                    {
                        StatusText.Text = "System Locked (UWF Active)";
                        StatusIndicator.Fill = new SolidColorBrush(Colors.Green);
                        BtnToggle.Content = "Unlock System (Disable UWF)";
                    }
                    else
                    {
                        StatusText.Text = "System Unlocked (Editable)";
                        StatusIndicator.Fill = new SolidColorBrush(Colors.Red);
                        BtnToggle.Content = "Lock System (Enable UWF)";
                    }
                    
                    Log("Status updated.");
                });
            }
            catch (Exception ex)
            {
                Log($"Error checking status: {ex.Message}");
            }
        }

        private void BtnToggle_Click(object sender, RoutedEventArgs e)
        {
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
