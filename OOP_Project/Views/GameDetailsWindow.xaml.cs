using MaterialDesignThemes.Wpf;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace OOP_Project
{
    public partial class GameDetailsWindow : Window
    {
        private readonly RawgApiService _apiService;
        private Game _currentGame;
        private const string RATINGS_FILE = "user_ratings.json";

        public GameDetailsWindow(Game game)
        {
            InitializeComponent();
            _apiService = new RawgApiService();
            _currentGame = game;

            LoadGameDetails();
            LoadUserRating();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch { }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            // Find the animation in resources
            if (this.Resources["CloseWindowAnimation"] is System.Windows.Media.Animation.Storyboard sb)
            {
                sb.Completed += (s, ev) => this.Close();
                sb.Begin();
            }
            else
            {
                // Fallback
                this.Close();
            }
        }

        private async void LoadGameDetails()
        {
            try
            {
                var details = await _apiService.GetGameDetailsAsync(_currentGame.Id);

                // Hide loading panel
                loadingPanel.Visibility = Visibility.Collapsed;

                // Populate UI
                txtGameName.Text = details.Name;
                txtMetacritic.Text = details.Rating.ToString("0.0");
                txtReleased.Text = details.Released ?? "Unknown";
                txtGenres.Text = details.Genres;
                txtPlatforms.Text = details.Platforms;
                txtDevelopers.Text = details.Developers;
                txtPublishers.Text = details.Publishers;
                txtPlaytime.Text = details.Playtime > 0 ? $"{details.Playtime} hours" : "N/A";
                txtAchievements.Text = details.AchievementsCount > 0 ? details.AchievementsCount.ToString() : "N/A";
                txtDescription.Text = details.Description;

                // Metacritic score with color coding
                if (details.Metacritic > 0)
                {
                    txtMetacritic.Text = details.Metacritic.ToString();

                    // Color code based on score
                    if (details.Metacritic >= 75)
                        metacriticBorder.Background = new System.Windows.Media.SolidColorBrush(
                            (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#6C8F22"));
                    else if (details.Metacritic >= 50)
                        metacriticBorder.Background = new System.Windows.Media.SolidColorBrush(
                            (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FC9802"));
                    else
                        metacriticBorder.Background = new System.Windows.Media.SolidColorBrush(
                            (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#C93129"));
                }
                else
                {
                    txtMetacritic.Text = "N/A";
                }

                // Load header image
                if (!string.IsNullOrEmpty(details.BackgroundImage))
                {
                    try
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(details.BackgroundImage);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        headerImage.ImageSource = bitmap;
                    }
                    catch { }
                }

                // Show website link if available
                if (!string.IsNullOrEmpty(details.Website))
                {
                    websiteLink.Visibility = Visibility.Visible;
                    websiteLink.Tag = details.Website;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading game details: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void LoadUserRating()
        {
            try
            {
                if (File.Exists(RATINGS_FILE))
                {
                    var json = File.ReadAllText(RATINGS_FILE);
                    var ratings = JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<int, int>>(json);

                    if (ratings != null && ratings.ContainsKey(_currentGame.Id))
                    {
                        int rating = ratings[_currentGame.Id];
                        ratingBar.Value = rating;
                        _currentGame.PersonalRating = rating;
                        UpdateRatingText(rating);
                    }
                }
            }
            catch { }
        }

        private void RatingBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int rating = (int)Math.Round(ratingBar.Value);
            SaveUserRating(rating);
            UpdateRatingText(rating);
        }

        private void UpdateRatingText(int rating)
        {
            if (rating > 0)
            {
                txtYourRating.Text = $"You rated this game {rating} out of 5 stars";

                // Update emoji based on rating
                string emoji;
                switch(rating)
                {
                    case 5: 
                        emoji = " ⭐⭐⭐⭐⭐";
                        break;
                    case 4: 
                        emoji = " ⭐⭐⭐⭐";
                        break;
                    case 3:
                        emoji = " ⭐⭐⭐";
                        break;
                    case 2:
                        emoji = " ⭐⭐";
                        break;
                    case 1:
                        emoji = " ⭐";
                        break;
                    default:
                        emoji = "";
                        break;

                }

                txtYourRating.Text += emoji;
            }
            else
            {
                txtYourRating.Text = "Not rated yet";
            }
        }

        private void SaveUserRating(int rating)
        {
            try
            {
                System.Collections.Generic.Dictionary<int, int> ratings;

                if (File.Exists(RATINGS_FILE))
                {
                    var json = File.ReadAllText(RATINGS_FILE);
                    ratings = JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<int, int>>(json)
                        ?? new System.Collections.Generic.Dictionary<int, int>();
                }
                else
                {
                    ratings = new System.Collections.Generic.Dictionary<int, int>();
                }

                ratings[_currentGame.Id] = rating;
                _currentGame.PersonalRating = rating;

                var newJson = JsonSerializer.Serialize(ratings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(RATINGS_FILE, newJson);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving rating: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void WebsiteLink_Click(object sender, MouseButtonEventArgs e)
        {
            if (websiteLink.Tag is string url)
            {
                try
                {
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                catch { }
            }
        }
    }
}
