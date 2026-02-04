using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OOP_Project
{
    public partial class MainWindow : Window
    {
        ObservableCollection<GameViewModel> Library { get; set; } = new ObservableCollection<GameViewModel>();

        public MainWindow()
        {
            InitializeComponent();
            // Bind the ListBox to the Library collection
            lstBxGame.ItemsSource = Library;
            gameGrid.ItemsSource = Library;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch { }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if(WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else WindowState = WindowState.Maximized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
            //Application.Current.Shutdown();
        }

        private void AddGame_Click(object sender, RoutedEventArgs e)
        {
            var searchWindow = new SearchWindow();
            searchWindow.GameSelected += AddGameToLibrary;
            searchWindow.ShowDialog();
        }

        private void AddGameToLibrary(Game game)
        {
            var gameVM = new GameViewModel(game);
            Library.Add(gameVM);

            // Select the newly added game
            SelectGame(gameVM);
        }

        private void DelGame_Click(object sender, RoutedEventArgs e)
        {
            // Get selected game
            if (lstBxGame.SelectedItem is GameViewModel selectedGame)
            {
                Library.Remove(selectedGame);
            }
            else
            {
                MessageBox.Show("Please select a game to delete.", "No Selection",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void lstBxGame_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstBxGame.SelectedItem is GameViewModel selectedGame)
            {
                SelectGame(selectedGame);
            }
        }

        private void GameCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is GameViewModel game)
            {
                // 1. Check if this game is already the selected one
                // (Assuming you have a variable or property tracking the selected game)
                if (game.IsSelected)
                {
                    // If it's already selected, open the details window
                    ShowGameDetails(game.Game);
                }
                else
                {
                    // 2. If it's not selected, select it now
                    SelectGame(game);
                }
            }
        }

        private void SelectGame(GameViewModel game)
        {
            // Deselect all games
            foreach (var g in Library)
            {
                g.IsSelected = false;
            }

            // Select the clicked game
            game.IsSelected = true;

            // Update ListBox selection
            lstBxGame.SelectedItem = game;

            // Scroll to the selected item in the ListBox
            lstBxGame.ScrollIntoView(game);
        }

        private void lstBxGame_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstBxGame.SelectedItem is GameViewModel gameVM)
            {
                ShowGameDetails(gameVM.Game);
            }
        }

        private void GameCard_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                if (sender is Border border && border.Tag is GameViewModel gameVM)
                {
                    ShowGameDetails(gameVM.Game);
                }
            }
        }

        private void ShowGameDetails(Game game)
        {
            GameDetailsWindow gameDetailWindow = new GameDetailsWindow(game);
            gameDetailWindow.ShowDialog();
        }
    }
}
