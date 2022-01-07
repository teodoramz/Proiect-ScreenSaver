using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();

            string iconUri = "..\\..\\..\\Resources\\ViewsResources\\logo.png";
            this.Icon = BitmapFrame.Create(new Uri(iconUri, UriKind.Relative));

            ClearFields();
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

        private void alreadyButton_Click(object sender, RoutedEventArgs e)
        {

            LoginWindow lw = new LoginWindow();
            lw.Show();
            this.Hide();
        }

        //check email format
        public bool IsValidMail(string emailaddress)
        {
            try
            {
                MailAddress mail = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public void ClearFields()
        {
            usernameTextBox.Clear();
            emailTextBox.Clear();
            passwordTextBox.Clear();
            confPasswordTextBox.Clear();
        }
        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            var username = usernameTextBox.Text;
            var email = emailTextBox.Text;
            var password = passwordTextBox.Password;
            var confPassword = confPasswordTextBox.Password;

            if (string.IsNullOrWhiteSpace(username)
                        || string.IsNullOrWhiteSpace(email)
                                    || string.IsNullOrWhiteSpace(password)
                                            || string.IsNullOrWhiteSpace(confPassword))
            {
                MessageBox.Show("Please fill all the fields!");
                ClearFields();
                return;
            }

            if(!IsValidMail(email))
            {
                MessageBox.Show("Wrong email format!");
                return;
            }

            if(confPassword!=password)
            {
                MessageBox.Show("The passwords do not match");
                passwordTextBox.Clear();
                confPasswordTextBox.Clear();
                return;
            }

            //register procedure
            ClientToServer _client = ClientToServer.getInstance;
            if (_client.isConnected())
            {
                // stergem orice urma de mesaje ramase cumva in interfata de client
                _client.clearAll();
                _client.RegisterRequestFunct(username,email, password);

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
                if (response == "RegisterSucces!")
                {
                    MessageBox.Show("Registration succesfull!");
                    var lw = new LoginWindow();
                    lw.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Unable to register! " +
                        "\n Please try again later or try it with another credidentials!");
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
    }

}

