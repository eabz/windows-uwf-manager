using System.Windows;
using UwfManager.Services;

namespace UwfManager
{
    public partial class PasswordWindow : Window
    {
        private readonly PasswordService _passwordService;
        private readonly bool _isSettingNew;

        public PasswordWindow(bool isSettingNew = false)
        {
            InitializeComponent();
            _passwordService = new PasswordService();
            _isSettingNew = isSettingNew;

            if (_isSettingNew)
            {
                Title = "First Run Setup";
                LblPrompt.Text = "Set Administrator Password:";
            }
            else
            {
                Title = "Authentication Required";
                LblPrompt.Text = "Enter Administrator Password:";
            }

            TxtPassword.Focus();
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            var input = TxtPassword.Password;

            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Password cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_isSettingNew)
            {
                _passwordService.SetPassword(input);
                MessageBox.Show("Password set successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else
            {
                if (_passwordService.VerifyPassword(input))
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Incorrect password.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Error);
                    TxtPassword.Clear();
                    TxtPassword.Focus();
                }
            }
        }
    }
}
