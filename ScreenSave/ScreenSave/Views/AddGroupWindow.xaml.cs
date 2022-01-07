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
    /// Interaction logic for AddGroupWindow.xaml
    /// </summary>
    public partial class AddGroupWindow : Window
    {
        private string currUserID;
        public AddGroupWindow(string userID)
        {
            InitializeComponent();
            string iconUri = "..\\..\\..\\Resources\\ViewsResources\\logo.png";
            this.Icon = BitmapFrame.Create(new Uri(iconUri, UriKind.Relative));
            currUserID = userID;
            groupNameTextBox.Text = "Group name";
            codeGroupTextBox.Text = "Group code";
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void createGroupButton_Click(object sender, RoutedEventArgs e)
        {
            //create group procedure
            string groupName = groupNameTextBox.Text;
            if(groupName == "Group name")
            {
                MessageBox.Show("Give a name to your group first!");
                return;
            }

            ClientToServer _client = ClientToServer.getInstance;
            if (_client.isConnected())
            {
                _client.clearAll();
                _client.CreateGroupRequestFunct(currUserID,groupName);

                while (_client.returnMessage == null)
                {
                    //wait a bit
                }
                if (_client.returnMessage.Count == 0)
                {
                    while (_client.returnMessage.Count == 0)
                    {
                        //wait a bit
                    }
                }
                string noMsg = _client.returnMessage[0];
                if (_client.returnMessage.Count != Convert.ToInt32(noMsg) + 1) // remember this\
                {
                    while (_client.returnMessage.Count != Convert.ToInt32(noMsg) + 1)
                    {

                    }
                }
                List<string> msg = _client.returnMessage;
                string response = msg[1];
                if (response == "CreateGroupSucces!")
                {
                    MessageBox.Show("Group successfully created!");
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Unable to create group!");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Server down! Please try again later!");
                this.Hide();
                return;
            }
        }

        private void joinGroupButton_Click(object sender, RoutedEventArgs e)
        {
            //join group procedure
            string groupCode = codeGroupTextBox.Text;
            if (groupCode == "Group code")
            {
                MessageBox.Show("Please enter the joining code first!");
                return;
            }

            ClientToServer _client = ClientToServer.getInstance;
            if (_client.isConnected())
            {
                _client.clearAll();
                _client.JoinGroupRequestFunct(currUserID, groupCode);

                while (_client.returnMessage == null)
                {
                    //wait a bit
                }
                if (_client.returnMessage.Count == 0)
                {
                    while (_client.returnMessage.Count == 0)
                    {
                        //wait a bit
                    }
                }
                string noMsg = _client.returnMessage[0];
                if (_client.returnMessage.Count != Convert.ToInt32(noMsg) + 1) // remember this\
                {
                    while (_client.returnMessage.Count != Convert.ToInt32(noMsg) + 1)
                    {

                    }
                }
                List<string> msg = _client.returnMessage;
                string response = msg[1];
                if (response == "JoinGroupSucces!")
                {
                    MessageBox.Show("You joined the group successfully!");
                    this.Hide();
                }
                else if(response == "InvalidCode!")
                {
                    MessageBox.Show("Invalid joining code!");
                    return;
                }
                else if(response == "AlreadyJoined!")
                {
                    MessageBox.Show("You are already in this group!");
                    return;
                }
                else
                {
                    MessageBox.Show("Unable to join group!");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Server down! Please try again later!");
                this.Hide();
                return;
            }

        }

        private void groupNameTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if(groupNameTextBox.Text == "Group name")
            {
                groupNameTextBox.Clear();
            }
        }

        private void groupNameTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if(string.IsNullOrEmpty(groupNameTextBox.Text))
            {
                groupNameTextBox.Text = "Group name";
            }
        }

        private void codeGroupTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if(codeGroupTextBox.Text == "Group code")
            {
                codeGroupTextBox.Clear();
                codeGroupTextBox.HorizontalContentAlignment = HorizontalAlignment.Left;
            }
        }

        private void codeGroupTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if(string.IsNullOrEmpty(codeGroupTextBox.Text))
            {
                codeGroupTextBox.Text = "Group code";
                codeGroupTextBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                codeGroupTextBox.TextWrapping = TextWrapping.NoWrap;
            }
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
