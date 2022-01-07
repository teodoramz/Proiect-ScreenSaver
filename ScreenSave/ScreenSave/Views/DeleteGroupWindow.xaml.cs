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
    /// Interaction logic for DeleteGroupWindow.xaml
    /// </summary>
    public partial class DeleteGroupWindow : Window
    {
        private string currUserID;
        private List<string> listGroups;
        public DeleteGroupWindow(string userID)
        {
            InitializeComponent();
            
            string iconUri = "..\\..\\..\\Resources\\ViewsResources\\logo.png";
            this.Icon = BitmapFrame.Create(new Uri(iconUri, UriKind.Relative));

            currUserID = userID;
            listGroups = new List<string>();
            yourGroupsComboBox.Items.Insert(0, "Your groups");
            //to be added
            loadGroupsOwner();
            loadGroups();
            yourGroupsComboBox.SelectedIndex = 0;
        }
        private void loadGroupsOwner()
        {
            ClientToServer _client = ClientToServer.getInstance;
            if (_client.isConnected())
            {
                // stergem orice urma de mesaje ramase cumva in interfata de client
                _client.clearAll();
                _client.LoadDeleteGroupsRequestFunct(currUserID);

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
                if (_client.returnMessage.Count != Convert.ToInt32(noMsg) + 1)
                {
                    while (_client.returnMessage.Count != Convert.ToInt32(noMsg) + 1)
                    {
                        //wait a bit
                    }
                }
                List<string> msg = _client.returnMessage;
                string response = msg[1];
                if (response == "LoadGroupsSucces!")
                {
                    if (msg[2] != "NoData")
                    {
                        for (int i = 2; i < Convert.ToInt32(noMsg) + 1; i++)
                        {
                            listGroups.Add(msg[i]);
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
        private void loadGroups()
        {
            ClientToServer _client = ClientToServer.getInstance;
            if (_client.isConnected())
            {
                // stergem orice urma de mesaje ramase cumva in interfata de client
                _client.clearAll();
                _client.LoadGroupsRequestFunct(currUserID);

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
                if (_client.returnMessage.Count != Convert.ToInt32(noMsg) + 1)
                {
                    while (_client.returnMessage.Count != Convert.ToInt32(noMsg) + 1)
                    {
                        //wait a bit
                    }
                }
                List<string> msg = _client.returnMessage;
                string response = msg[1];
                if (response == "LoadGroupsSucces!")
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void leaveButton_Click(object sender, RoutedEventArgs e)
        {
            string groupName = yourGroupsComboBox.SelectedItem.ToString();
            string action = "NoAction";
            if(groupName == "Your groups")
            {
                MessageBox.Show("Please chose a group first!");
                return;
            }
            if (listGroups.Contains(groupName))
            {
                if (MessageBox.Show("You are the owner of this group. If you leave, the group will be deleted! Are you sure about that?","Question", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    action = "Action";
                }
                else
                {
                    return;
                }
            }

            //leave procedure
            ClientToServer _client = ClientToServer.getInstance;
            if (_client.isConnected())
            {
                _client.clearAll();
                _client.LeaveGroupRequestFunct(currUserID, groupName, action);

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
                if (response == "LeaveGroupSucces!")
                {
                    MessageBox.Show("Group successfully left!");
                    this.Hide();
                }
                else if(response == "DeleteGroupSucces!")
                {
                    MessageBox.Show("Group successfully left and deleted!");
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Unable to leave group!");
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

        private void closeButoon_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
