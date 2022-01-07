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
    /// Interaction logic for GenerateCodesWindow.xaml
    /// </summary>
    public partial class GenerateCodesWindow : Window
    {
        private string currUserID;
        private static Random random = new Random();
        public GenerateCodesWindow(string userID)
        {
            InitializeComponent();

            string iconUri = "..\\..\\..\\Resources\\ViewsResources\\logo.png";
            this.Icon = BitmapFrame.Create(new Uri(iconUri, UriKind.Relative));

            currUserID = userID;
            codeGeneratedTextBox.IsReadOnly = true;

            yourGroupsComboBox.Items.Insert(0, "Your groups");
            //to be added
            loadGroups();
            yourGroupsComboBox.SelectedIndex = 0;
            expDateLabel.Content="";
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void loadGroups()
        {
            ClientToServer _client = ClientToServer.getInstance;
            if (_client.isConnected())
            {
                _client.clearAll();
                _client.LoadOwnerGroupsRequestFunct(currUserID);

                while (_client.returnMessage == null)
                {
                    //wait a bit
                }
                if (_client.returnMessage.Count == 0)
                {
                    while (_client.returnMessage.Count == 0)
                    {
                        //
                    }
                }
                string noMsg = _client.returnMessage[0];
                if (_client.returnMessage.Count != Convert.ToInt32(noMsg) + 1)
                {
                    while (_client.returnMessage.Count != Convert.ToInt32(noMsg) + 1)
                    {
                        //
                    }
                }
                List<string> msg = _client.returnMessage;
                string response = msg[1];
                if (response == "LoadOwnerGroupsSucces!")
                {
                    if (msg[2] != "NoData")
                    {
                        for (int i = 2; i < Convert.ToInt32(noMsg) + 1; i++)
                        {
                            yourGroupsComboBox.Items.Add(msg[i]);
                        }
                    }

                }
                else
                {
                    MessageBox.Show("Can't load groups in combo box!");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Server down! Please try again later!");
                return;
            }
        }

        private static char cipher(char ch, int key)
        {
            if (!char.IsLetter(ch))
            {

                return ch;
            }

            char d = char.IsUpper(ch) ? 'A' : 'a';
            return (char)((((ch + key) - d) % 26) + d);


        }
        //random code generator
        private string generateCode(string groupName)
        {
            string output = string.Empty;

            foreach (char ch in groupName)
                output += cipher(ch, 17);

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            output += new string(Enumerable.Repeat(chars, 150)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return output;
        }
        private void generateCodeButton_Click(object sender, RoutedEventArgs e)
        {
            string groupName = yourGroupsComboBox.SelectedItem.ToString();
            if(groupName == "Your groups")
            {
                MessageBox.Show("Chose a group first!");
                return;
            }
            string generatedCode = generateCode(groupName);

            //add generated code to db procedure
            ClientToServer _client = ClientToServer.getInstance;
            if (_client.isConnected())
            {
                // stergem orice urma de mesaje ramase cumva in interfata de client
                _client.clearAll();
                _client.GenerateCodeRequestFunct(currUserID, groupName, generatedCode);

                while (_client.returnMessage == null)
                {
                    //wait a bit
                }
                if (_client.returnMessage.Count == 0)
                {
                    while (_client.returnMessage.Count == 0)
                    {

                    }
                }//
                string noMsg = _client.returnMessage[0];
                if (_client.returnMessage.Count != Convert.ToInt32(noMsg) + 1) // remember this\
                {
                    while (_client.returnMessage.Count != Convert.ToInt32(noMsg) + 1)
                    {

                    }
                }
                List<string> msg = _client.returnMessage;
                string response = msg[1];
                if (response == "AddCodeSucces!")
                {
                    DateTime date = DateTime.Now;
                    date.AddDays(7);
                    MessageBox.Show("Code generated!");
                    expDateLabel.Content = "Expiration date: " + date.ToString();
                    codeGeneratedTextBox.TextAlignment = TextAlignment.Left;
                    codeGeneratedTextBox.TextWrapping = TextWrapping.NoWrap;
                    codeGeneratedTextBox.Text = generatedCode;
                    codeGeneratedTextBox.IsReadOnly = false;
                }
                else
                {
                    MessageBox.Show("Unable to add generated code to database!");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Server down! Please try again later!");
                return;
            }
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
