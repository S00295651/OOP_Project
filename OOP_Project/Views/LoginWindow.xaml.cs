using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OOP_Project
{
    public partial class LoginWindow : Window
    {
        private readonly FirebaseAuthService _auth = new FirebaseAuthService();
        private bool _isRegisterMode = false;

        public LoginWindow()
        {
            InitializeComponent();
            txtEmail.Focus();
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try { DragMove(); } catch { }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ToggleMode_Click(object sender, MouseButtonEventArgs e)
        {
            _isRegisterMode = !_isRegisterMode;
            HideError();

            if (_isRegisterMode)
            {
                txtTitle.Text = "Create Account";
                txtSubtitle.Text = "Start building your game library";
                btnAction.Content = "Register";
                txtToggleLabel.Text = "Already have an account? ";
                txtToggleLink.Text = "Sign In";
            }
            else
            {
                txtTitle.Text = "Sign In";
                txtSubtitle.Text = "Access your game library anywhere";
                btnAction.Content = "Sign In";
                txtToggleLabel.Text = "Don't have an account? ";
                txtToggleLink.Text = "Register";
            }
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btnAction_Click(sender, null);
        }


        private async void btnAction_Click(object sender, RoutedEventArgs e)
        {
            var email = txtEmail.Text.Trim();
            var password = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ShowError("Please enter your email and password.");
                return;
            }

            SetLoading(true);
            HideError();

            try
            {
                if (_isRegisterMode)
                {
                    await _auth.RegisterAsync(email, password);
                    txtLoading.Text = "Creating account...";

                    // Create default profile for the new user
                    var dataService = new FirebaseDataService();
                    await dataService.SaveProfileAsync(new UserProfile
                    {
                        Username = email.Split('@')[0],
                        MemberSince = DateTime.Now.ToString("MMMM yyyy")
                    });
                }
                else
                {
                    await _auth.LoginAsync(email, password);
                }

                // The caller will handle navigation
                Close();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void SetLoading(bool isLoading)
        {
            loadingPanel.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
            btnAction.IsEnabled = !isLoading;
            txtEmail.IsEnabled = !isLoading;
            txtPassword.IsEnabled = !isLoading;

            if (isLoading)
                txtLoading.Text = _isRegisterMode ? "Creating account..." : "Signing in...";
        }

        private void ShowError(string message)
        {
            txtError.Text = message;
            txtError.Visibility = Visibility.Visible;
        }

        private void HideError()
        {
            txtError.Visibility = Visibility.Collapsed;
        }
    }
}
