using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AppHelper;
using Microsoft.Win32;
using Image = System.Drawing.Image;

namespace ClientApp.Views
{
    /// <summary>
    /// Interaction logic for ScreenshotMainWindow.xaml
    /// </summary>
    public partial class ScreenshotMainWindow : Window
    {
        private string currUserID;
        private string tempImg;
        private static Random random = new Random();
        private BitmapImage currBitmapImage;
        public ScreenshotMainWindow(string userID, BitmapImage bmpImg = null)
        {
            InitializeComponent();

            string iconUri = "..\\..\\..\\Resources\\ViewsResources\\logo.png";
            this.Icon = BitmapFrame.Create(new Uri(iconUri, UriKind.Relative));

            this.currUserID = userID;
            currBitmapImage = bmpImg;
            if(currBitmapImage != null)
            {
                imageBkg.Source = currBitmapImage;
            }
            tempImg = "";
            savePlacesComboBox.Items.Insert(0, "Saving ... ");
            savePlacesComboBox.Items.Add("Save local");
            //to be added
            savePlacesComboBox.SelectedIndex = 0;
            
            loadGroups();
        }
       public void setTempImg(string filename)
        {
            this.tempImg = filename;
        }
        private void exitButtonClicked(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            DashboardWindow rw = new DashboardWindow(currUserID);
            rw.Show();
            this.Hide();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void selected_areaButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteTempImg();
            var csw = new CustomScreenshotWindow(this, currUserID);
            csw.Show();
            this.Hide();
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
                            savePlacesComboBox.Items.Add(msg[i]);
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
        private string ConvertImageToBytes(Image img)
        {
            using(MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return Convert.ToBase64String(ms.ToArray());
            }
        }
        
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            string savePlace = savePlacesComboBox.SelectedItem.ToString();
            if(savePlace == "Saving ... ")
            {
                MessageBox.Show("Please select where to save the screenshot!");
                return;
            }
            if(tempImg == "")
            {
                MessageBox.Show("No image to be saved!");
                    return;
            }
            if(savePlace == "Save local")
            {
                string output = "";
                const String chars = "0123456789-_";
                output += new String(Enumerable.Repeat(chars, 16)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                string date = DateTime.Now.ToShortDateString();
                date=date.Replace('/', '-');
                Image image = Image.FromFile(tempImg);
                SaveFileDialog sf = new SaveFileDialog();
                sf.FileName = "ScreenSaverScreenshot" + date + output + "T"; // Default file name
                sf.DefaultExt = ".png"; // Default file extension
                sf.Filter = "PNG(*.PNG)|*.png";
                if (sf.ShowDialog() == true)
                {

                    image.Save(sf.FileName);
                    imageBkg.Source = null;
                    
                }
                else
                {
                    return;
                }
            }
            else
            {
                //convert Image to string
                Image image = Image.FromFile(tempImg);
                string imageString = ConvertImageToBytes(image);

                //generate image filename
                string output = "";
                const String chars = "0123456789-_";
                output += new String(Enumerable.Repeat(chars, 16)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                string date = DateTime.Now.ToShortDateString();
                date = date.Replace('/', '-');

                string filename = "ScreenSaverScreenshot" + date + output + "T";


                //send photo procedure
                ClientToServer _client = ClientToServer.getInstance;
                if (_client.isConnected())
                {
                    // stergem orice urma de mesaje ramase cumva in interfata de client
                    _client.clearAll();
                    _client.SavePhotoToGroupRequestFunct(filename, imageString, savePlace);
                    //_client.LoginRequestFunct(username, password);

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
                    if (response == "SendPhotoSucces!")
                    {
                        MessageBox.Show("Screenshot saved!");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Can't save the ss!");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Server down! Please try again later!");
                    return;
                }
            }
        }
        
        private BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            {
                BitmapSource i = Imaging.CreateBitmapSourceFromHBitmap(
                               bitmap.GetHbitmap(),
                               IntPtr.Zero,
                               Int32Rect.Empty,
                               BitmapSizeOptions.FromEmptyOptions());
                return (BitmapImage)i;
            }
        }
        public double GetWindowsScaling()
        {
            double factor = System.Windows.PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice.M11;
            if(AppHelper.ScreenH.MultipleMonitors())
            {
                return 1;
            }
            return factor;
        }
        public void DeleteTempImg()
        {
            
            imageBkg.Source = null;
        }
        private void fullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            //monitors scale
            double scale = GetWindowsScaling();

            int screenLeft = (int)SystemParameters.VirtualScreenLeft;
            int screenTop = (int)SystemParameters.VirtualScreenTop;
            int screenWidth = (int)SystemParameters.VirtualScreenWidth;
            int screenHeight = (int)SystemParameters.VirtualScreenHeight;

            double width = screenWidth * scale;
            double height = screenHeight * scale;
            
            this.Hide();
            
            Bitmap bitmap_screen = new Bitmap((int)width, (int)height);
            Graphics g = Graphics.FromImage(bitmap_screen);
            string uri = Environment.CurrentDirectory + "\\temp.png";
            if (File.Exists(uri))
            {
                File.Delete(uri);
            }

           
            g.CopyFromScreen(screenLeft, screenTop, 0, 0, bitmap_screen.Size);
            this.Show();
           //var brush = new ImageBrush();
            //brush.ImageSource = imageSourceForImageControl(bitmap_screen);
            bitmap_screen.Save(uri);
            BitmapImage bitmap = new BitmapImage();
            using (FileStream fs = new FileStream(uri, FileMode.Open, FileAccess.Read))
            {
                
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = fs;
                bitmap.EndInit();
                fs.Close();
            }
                imageBkg.Source = bitmap;
                currBitmapImage = bitmap;
                tempImg = uri;
            //System.Threading.Thread.Sleep(1000);
            //SendKeys.Send("{PRTSC}");
            //System.Drawing.Image myImage = Clipboard.GetImage();
            //selected_areaGrid.Background = myImage;
        }

        private void cpyClipboard_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetImage(currBitmapImage);
            MessageBox.Show("Copied!");
        }
    }
}
