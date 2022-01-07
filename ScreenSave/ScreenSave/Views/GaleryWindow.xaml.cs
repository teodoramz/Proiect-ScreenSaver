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
using Image = System.Drawing.Image;

namespace ClientApp.Views
{
    /// <summary>
    /// Interaction logic for GaleryWindow.xaml
    /// </summary>
    public partial class GaleryWindow : Window
    {
        private string currUserID;
        private List<string> imageNames;
        private List<BitmapImage> images;
        private List<DateTime> dates;
        public GaleryWindow(string userID)
        {
            InitializeComponent();

            string iconUri = "..\\..\\..\\Resources\\ViewsResources\\logo.png";
            this.Icon = BitmapFrame.Create(new Uri(iconUri, UriKind.Relative));

            //clipboard.setimage(image imagine);
            imageNames = new List<string>();
            images = new List<BitmapImage>();
            dates = new List<DateTime>();

            currUserID = userID;
            yourGroupsComboBox.Items.Insert(0, "Your groups");
            loadGroups();
            //to be added
            yourGroupsComboBox.SelectedIndex = 0;
           
        }

        public void loadGroups()
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
        
        private BitmapImage ConvertStringToBitmapImage(string data)
        {
            byte[] bytesData = Convert.FromBase64String(data);
            var image = new BitmapImage();
            using (var mem = new MemoryStream(bytesData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
            
        }
        private void yourGroupsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //galeryListBox.Items.Clear();
            string groupName = yourGroupsComboBox.SelectedItem.ToString();
            if(groupName == "Your groups")
            {
                return;
            }
            ClientToServer _client = ClientToServer.getInstance;
            if (_client.isConnected())
            {
                _client.clearAll();
                _client.LoadImgRequestFunct(groupName);

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
                BitmapImage bitmap;
                if (response == "LoadImgSucces!")
                {
                    if (msg[2] != "NoData")
                    {
                        imageNames.Clear();
                        images.Clear();
                        dates.Clear();
                        galeryListBox.Items.Clear();

                        for (int i = 2; i < Convert.ToInt32(noMsg) + 1; i++)
                        {
                            //image name
                            imageNames.Add(msg[i]);

                            //img bytes to bitmapImage
                            bitmap = ConvertStringToBitmapImage(msg[i + 1]);
                            images.Add(bitmap);
                            
                            System.Windows.Controls.Image imageGalery = new System.Windows.Controls.Image();
                            imageGalery.Source = bitmap;
                            imageGalery.Width = 390;
                            imageGalery.Height = 200;
                            imageGalery.HorizontalAlignment = HorizontalAlignment.Center;
                            imageGalery.VerticalAlignment = VerticalAlignment.Center;
                            //image on click event
                            imageGalery.MouseLeftButtonDown += ImageClicked;
                            
                            //add image to galery
                            galeryListBox.Items.Add(imageGalery);
                            
                            dates.Add(DateTime.Parse(msg[i + 2]));
                            i = i + 2;

                           
                        }
                    }
                    else
                    {
                        //no images, do not modify galery or lists
                        MessageBox.Show("No screenshots in this group!");
                        return;
                    }

                }
                else
                {
                    MessageBox.Show("Can't load images in list box!");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Server down! Please try again later!");
                return;
            }

        }

        private void ImageClicked(object sender, MouseButtonEventArgs e)
        {
            int position = 0;
            System.Windows.Controls.Image img = sender as System.Windows.Controls.Image;
            for (int i = 0; i < images.Count; i++)
            {
                if( images[i] == img.Source)
                {
                    position = i;
                    break;
                }
            }
            //currUserID, this.imageNames, this.images, this.dates, position
            ViewImageWindow viw = new ViewImageWindow(currUserID, this.imageNames, this.images, this.dates, position, this);
            viw.Show();
            this.Hide();

            //var test = new test();
            //t.Show();


        }

        private void backToDashboardButton_Click(object sender, RoutedEventArgs e)
        {
            var dw = new DashboardWindow(currUserID);
            dw.Show();
            this.Hide();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void closeButton_Click(object sender, RoutedEventArgs e)
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
    }
}
