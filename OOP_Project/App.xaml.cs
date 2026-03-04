using System.Windows;

namespace OOP_Project
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Show the login window first
            var login = new LoginWindow();
            login.Show();
        }
    }
}
