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

namespace ClientApp.Views
{
    /// <summary>
    /// Interaction logic for ManageGroups.xaml
    /// </summary>
    public partial class ManageGroups : Window
    {
        private string currUserID;
        public ManageGroups(string userId)
        {
            InitializeComponent();

            string iconUri = "..\\..\\..\\Resources\\ViewsResources\\logo.png";
            this.Icon = BitmapFrame.Create(new Uri(iconUri, UriKind.Relative));

            currUserID = userId;
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
        private void addGroupButton_Click(object sender, RoutedEventArgs e)
        {
            AddGroupWindow agw = new AddGroupWindow(currUserID);
            agw.Show();
            // deocamdata va fi pop-out
        }

        private void deleteGroupButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteGroupWindow dgw = new DeleteGroupWindow(currUserID);
            dgw.Show();
        }

        private void generateCodeButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateCodesWindow gcw = new GenerateCodesWindow(currUserID);
            gcw.Show();
        }

        private void backDashboardButton_Click(object sender, RoutedEventArgs e)
        {
            DashboardWindow dw = new DashboardWindow(currUserID);
            dw.Show();
            this.Hide();
        }
    }
}
