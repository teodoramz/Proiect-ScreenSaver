using System;
using System.Collections.Generic;
using System.IO;
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

namespace ClientApp.Views
{
    /// <summary>
    /// Interaction logic for DashboardWindow.xaml
    /// </summary>
    public partial class DashboardWindow : Window
    {
        private string currUserId;
        public DashboardWindow(string userId)
        {
            InitializeComponent();
            string iconUri = "..\\..\\..\\Resources\\ViewsResources\\logo.png";
            this.Icon = BitmapFrame.Create(new Uri(iconUri, UriKind.Relative));
            this.currUserId = userId;
        }

        private void take_screenshotButton_Click(object sender, RoutedEventArgs e)
        {
            ScreenshotMainWindow rw = new ScreenshotMainWindow(currUserId);
            rw.Show();
            this.Hide();
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            // se elimina instanta de clientToServer
            File.Delete("credidentials.txt");

            LoginWindow rw = new LoginWindow();
            rw.Show();
            this.Hide();
        }

        private void manageGroupsButton_Click(object sender, RoutedEventArgs e)
        {
            ManageGroups mg = new ManageGroups(currUserId);
            mg.Show();
            this.Hide();
        }

        private void galleryButton_Click(object sender, RoutedEventArgs e)
        {
            GaleryWindow gw = new GaleryWindow(currUserId);
            gw.Show();
            this.Hide();
        }
    }
}
