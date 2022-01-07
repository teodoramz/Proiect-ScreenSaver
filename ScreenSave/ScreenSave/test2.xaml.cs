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

namespace ClientApp
{
    /// <summary>
    /// Interaction logic for test2.xaml
    /// </summary>
    public partial class test2 : Window
    {
        public test2()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClientToServer _client = ClientToServer.getInstance;
            _client.SecondFunc();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var unu = new test();
            unu.Show();
            this.Hide();
        }
    }
}
