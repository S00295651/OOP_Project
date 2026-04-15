using OOP_Project.Services;
using System;
using System.Collections.Generic;
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
                MessageBox.Show("You must be logged in to save your profile", "Not logged in",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

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

            txtUsername.Text = profile.Username;
            txtMemberSince.Text = $"Member since {profile.MemberSince}";
            txtSteamId.Text = profile.SteamId ?? "";
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

            // Login success
            var oldMain = this.Owner;
            var newMain = new MainWindow();
            newMain.Show();
            Close();
            oldMain?.Close();
        }

        private async void btnImportSteam_Click(object sender, RoutedEventArgs e)
        {
            var steamId = txtSteamId.Text.Trim();
            if (string.IsNullOrWhiteSpace(steamId))
            {
                MessageBox.Show("Please enter your Steam ID first", "Steam ID required",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            btnImportSteam.IsEnabled = false;
            btnImportSteam.Content = "Importing...";

            try
            {
                var steamService = new SteamApiService();
                var steamGames = await steamService.GetOwnedGamesAsync(steamId);

                if (steamGames.Count == 0)
                {
                    MessageBox.Show("No games found, make sure your Steam profile is public",
                        "No games found", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Merge existing firebase library
                var existingGames = new List<Game>();
                try { existingGames = await _dataService.LoadLibraryAsync(); } catch { }

                var existingIds = new HashSet<int>(existingGames.ConvertAll(g => g.Id));
                var newGames = steamGames.FindAll(g => !existingIds.Contains(g.Id));
                existingGames.AddRange(newGames);

                await _dataService.SaveLibraryAsync(existingGames);

                // Reload MainWindow library
                if (Owner is MainWindow mainWindow)
                    await mainWindow.ReloadLibraryAsync();

                MessageBox.Show($"Imported {newGames.Count} new games from Steam",
                    "Import successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Import failed: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnImportSteam.IsEnabled = true;
                btnImportSteam.Content = "Import Steam Library";
            }
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