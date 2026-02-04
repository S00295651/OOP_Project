using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using System.Text.Json;


namespace OOP_Project
{
    public partial class SearchWindow : Window
    {
        private const string API_KEY = "7dcd080536e44d449ba1da3ba9122131";

        public event Action<Game> GameSelected;

        public SearchWindow()
        {
            InitializeComponent();
            SearchBox.Focus(); // Auto-focus search box when window opens
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
            Close();
        }

        // Handle Enter key press in search box
        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Search_Click(sender, null);
            }
        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                MessageBox.Show("Please enter a game name to search.", "Empty Search",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Show loading indicator
            LoadingPanel.Visibility = Visibility.Visible;
            NoResultsPanel.Visibility = Visibility.Collapsed;
            ResultsList.ItemsSource = null;

            try
            {
                var games = await SearchGames(SearchBox.Text);

                // Hide loading indicator
                LoadingPanel.Visibility = Visibility.Collapsed;

                if (games.Count > 0)
                {
                    ResultsList.ItemsSource = games;
                }
                else
                {
                    NoResultsPanel.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                LoadingPanel.Visibility = Visibility.Collapsed;
                MessageBox.Show($"Error searching for games: {ex.Message}", "Search Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<List<Game>> SearchGames(string query)
        {
            var url = $"https://api.rawg.io/api/games?key={API_KEY}&search={query}";

            using (var client = new HttpClient())
            {
                var json = await client.GetStringAsync(url);

                var root = JsonSerializer.Deserialize<RawgRoot>(json);
                var list = new List<Game>();

                foreach (var raw in root.results)
                {
                    list.Add(new Game
                    {
                        Id = raw.id,
                        Name = raw.name,
                        Released = raw.released,
                        ImageUrl = raw.background_image
                    });
                }

                return list;
            }
        }

        private void GameCard_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is Game game)
            {
                GameSelected?.Invoke(game);
                Close();
            }
        }
    }
}
