using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace OOP_Project
{
    public partial class StatsWindow : Window
    {
        private readonly ObservableCollection<GameViewModel> _library;

        public StatsWindow(ObservableCollection<GameViewModel> library)
        {
            InitializeComponent();
            _library = library;

            LoadStats();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try { DragMove(); } catch { }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LoadStats()
        {
            // TODO: Compute and display real statistics from _library
            // Examples of what will go here:
            //   - txtTotalGames.Text = _library.Count.ToString();
            //   - txtAvgRating.Text  = average personal rating
            //   - txtRatedGames.Text = count of games with PersonalRating > 0
            //   - txtTopGenre.Text   = most frequent genre across library
            //   - Populate lstTopGames with top 5 rated games
            //   - Feed genre/rating data into chart controls
        }
    }
}
