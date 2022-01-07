using CoreApp;
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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            string iconUri = "..\\..\\..\\Resources\\ViewsResources\\logo.png";
            this.Icon = BitmapFrame.Create(new Uri(iconUri, UriKind.Relative));

            usernameTextBox.Text = "Username";
            passwordTextBox.Password = "Password";
            //to do - keep me logged in!
            string username = "";
            string password = "";

            try
            {
                if (File.Exists("credidentials.txt"))
                {
                    StreamReader sr = new StreamReader("credidentials.txt");
                    username = sr.ReadLine();
                    password = sr.ReadLine();
                    username = AES_Client.Decrypt256(username);
                    password = AES_Service.Decrypt256(password);
                    sr.Close();
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }

            if(!string.IsNullOrEmpty(username) 
                    && !string.IsNullOrEmpty(password))
            {
                loginProcedure(username, password);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void exitButtonClicked(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow rw = new RegisterWindow();
            rw.Show();
            this.Hide();

        }
        public void ClearFields()
        {
            usernameTextBox.Clear();
            passwordTextBox.Clear();
        }
        private void loginProcedure(string username, string password)
        {
            //login procedure
            ClientToServer _client = ClientToServer.getInstance;
            if (_client.isConnected())
            {
                // stergem orice urma de mesaje ramase cumva in interfata de client
                _client.clearAll();
                _client.LoginRequestFunct(username, password);

                while (_client.returnMessage == null)
                {
                    //wait a bit
                }
                if (_client.returnMessage.Count == 0)
                {
                    while (_client.returnMessage.Count == 0)
                    {

                    }
                }
                string noMsg = _client.returnMessage[0];
                if (_client.returnMessage.Count != Convert.ToInt32(noMsg) + 1)
                {
                    while (_client.returnMessage.Count != Convert.ToInt32(noMsg) + 1)
                    {

                    }
                }
                List<string> msg = _client.returnMessage;
                string response = msg[1];
                if (response == "LogInSucces!")
                {
                    if (keepMeLoggedInCheckBox.IsChecked == true)
                    {
                        try
                        {
                            StreamWriter sw = new StreamWriter("credidentials.txt");
                            sw.WriteLine(AES_Client.Encrypt256(username));
                            sw.WriteLine(AES_Service.Encrypt256(password));

                            sw.Close();
                        }
                        catch (Exception ew)
                        {
                            MessageBox.Show("Eroare scriere fisier credidentiale, remember me" + ew.Message);
                            return;
                        }
                    }

                    string userID = msg[2];
                    DashboardWindow rw = new DashboardWindow(userID);
                    rw.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Wrong username or password!");
                    ClearFields();
                    return;
                }
            }
            else
            {
                MessageBox.Show("Server down! Please try again later!");
                ClearFields();
                return;
            }
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameTextBox.Text;
            string password = passwordTextBox.Password;

            if(username == ""||password == "")
            {
                MessageBox.Show("Please fill all the fields!");
                return;
            }

            loginProcedure(username, password);
        }

        private void usernameTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if(usernameTextBox.Text == "Username")
            {
                usernameTextBox.Clear();
            }
        }

        private void usernameTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if(string.IsNullOrEmpty(usernameTextBox.Text))
            {
                usernameTextBox.Text = "Username";
            }
        }

        private void passwordTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (passwordTextBox.Password == "Password")
            {
                passwordTextBox.Clear();
            }
        }

        private void passwordTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(passwordTextBox.Password))
            {
                passwordTextBox.Password = "Password";
            }
        }

        private void keepMeLoggedInCheckBox1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (keepMeLoggedInCheckBox.IsChecked == true)
                keepMeLoggedInCheckBox.IsChecked = false;
            else if(keepMeLoggedInCheckBox.IsChecked == false)
                keepMeLoggedInCheckBox.IsChecked = true;
        }
    }
}

