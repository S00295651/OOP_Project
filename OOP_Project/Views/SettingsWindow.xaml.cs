using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OOP_Project
{
    public partial class SettingsWindow : Window
    {
        private readonly FirebaseDataService _dataService = new FirebaseDataService();

        // Tracks the currently active nav item so we can reset its background
        private Border _activeNav;

        public SettingsWindow()
        {
            InitializeComponent();
            _activeNav = navProfile;
            LoadProfile();
        }

        private async void LoadProfile()
        {
            if (!UserSession.IsLoggedIn) return;

            try
            {
                var profile = await _dataService.LoadProfileAsync();

                txtEditUsername.Text = profile.Username;
                txtFavGenre.Text = profile.FavouriteGenre;
                txtBio.Text = profile.Bio;
                txtUsername.Text = profile.Username;
                txtMemberSince.Text = $"Member since {profile.MemberSince}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not load profile: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async System.Threading.Tasks.Task SaveProfileAsync()
        {
            if (!UserSession.IsLoggedIn)
            {
                MessageBox.Show("You must be logged in to save your profile.", "Not logged in",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Load existing profile first to preserve MemberSince
            string memberSince = DateTime.Now.ToString("MMMM yyyy");
            try
            {
                var existing = await _dataService.LoadProfileAsync();
                if (!string.IsNullOrWhiteSpace(existing.MemberSince))
                    memberSince = existing.MemberSince;
            }
            catch { }

            var profile = new UserProfile
            {
                Username = txtEditUsername.Text.Trim(),
                FavouriteGenre = txtFavGenre.Text.Trim(),
                Bio = txtBio.Text.Trim(),
                MemberSince = memberSince
            };

            await _dataService.SaveProfileAsync(profile);

            // Refresh header labels
            txtUsername.Text = profile.Username;
            txtMemberSince.Text = $"Member since {profile.MemberSince}";
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try { DragMove(); } catch { }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Owner = this;
            loginWindow.ShowDialog();

            if (!UserSession.IsLoggedIn) return;

            // Login succeeded (open a fresh MainWindow close Settings + old MainWindow)
            var oldMain = this.Owner;
            var newMain = new MainWindow();
            newMain.Show();
            Close();
            oldMain?.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            btnSave.IsEnabled = false;
            try
            {
                await SaveProfileAsync();
                MessageBox.Show("Profile saved!", "Saved",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not save profile: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnSave.IsEnabled = true;
            }
        }

        private void NavItem_Click(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is Border clicked)) return;

            // Reset previously active nav highlight
            _activeNav.Background = Brushes.Transparent;

            // Highlight new active nav
            clicked.Background = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString("#2A475E"));
            _activeNav = clicked;

            // Hide all sections, then show the matching one
            sectionProfile.Visibility = Visibility.Collapsed;
            sectionAppearance.Visibility = Visibility.Collapsed;
            sectionLibrary.Visibility = Visibility.Collapsed;
            sectionApi.Visibility = Visibility.Collapsed;
            sectionAbout.Visibility = Visibility.Collapsed;

            string tag = clicked.Tag as string ?? "";

            switch (tag)
            {
                case "Profile": sectionProfile.Visibility = Visibility.Visible; break;
                case "Appearance": sectionAppearance.Visibility = Visibility.Visible; break;
                case "Library": sectionLibrary.Visibility = Visibility.Visible; break;
                case "API": sectionApi.Visibility = Visibility.Visible; break;
                case "About": sectionAbout.Visibility = Visibility.Visible; break;
            }
        }
    }
}