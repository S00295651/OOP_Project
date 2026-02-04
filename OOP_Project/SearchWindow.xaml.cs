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
        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            var games = await SearchGames(SearchBox.Text);
            ResultsList.ItemsSource = games;
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

        private void ResultsList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ResultsList.SelectedItem is Game g)
            {
                GameSelected?.Invoke(g);
                Close();
            }
        }
    }
}
