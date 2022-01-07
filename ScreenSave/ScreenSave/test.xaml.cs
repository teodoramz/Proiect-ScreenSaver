using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    /// Interaction logic for test.xaml
    /// </summary>
    public partial class test : Window
    {
        public test()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //do smth
            string ceva = AES_Client.Encrypt256("ceva");

            MessageBox.Show(ceva);
            string altceva = AES_Client.Decrypt256(ceva);
            MessageBox.Show(altceva);
            if(altceva == "ceva")
            {
                MessageBox.Show("Da!");
            }
        }

       
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            bool waiting = true;
            ClientToServer _client = ClientToServer.getInstance;
            if (_client.isConnected())
            {
                _client.FirstFunct();
                while (_client.returnMessage == null)
                {

                }
                if (_client.returnMessage != null && waiting)
                {
                    while (_client.returnMessage.Count == 0)
                    {

                    }

                    string noMsg = _client.returnMessage[0];
                    while (_client.returnMessage.Count != Convert.ToInt32(noMsg) + 1)
                    {

                    }
                    List<string> cv = _client.returnMessage;
                    string mesaj = cv[1];
                    if (mesaj == "Teodor Amzuloiu")
                    {
                        MessageBox.Show("U are awesome!");
                    }
                    _client.returnMessage.Clear();
                    waiting = false;
                }
            }
            else
            {
                MessageBox.Show("Server down!");
                return;
            }

        }
    }
}
