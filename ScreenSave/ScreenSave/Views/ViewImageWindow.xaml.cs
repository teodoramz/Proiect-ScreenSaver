using Microsoft.Win32;
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
    /// Interaction logic for ViewImageWindow.xaml
    /// </summary>
    public partial class ViewImageWindow : Window
    {
        private string currUserID;
        private List<string> imageNames;
        private List<BitmapImage> images;
        private List<DateTime> dates;
        private int position;
        private GaleryWindow gw;
        
        public ViewImageWindow(string userID, List<string> imageNames, 
                                List<BitmapImage> images, List<DateTime> dates, int pos, GaleryWindow gw)
        {
            InitializeComponent();

            string iconUri = "..\\..\\..\\Resources\\ViewsResources\\logo.png";
            this.Icon = BitmapFrame.Create(new Uri(iconUri, UriKind.Relative));

            currUserID = userID;
            this.imageNames = imageNames;
            this.images = images;
            this.dates = dates;
            position = pos;
            this.gw = gw;

            Clipboard.SetImage(images[pos]);
            //var clipboad = new Clipboard();
            //clipboard.setimage(images[pos]);

            galeryImageZoom.Source = this.images[pos];
            if (pos == 0)
            {
                prevButton.IsEnabled = false;
            }
            if (pos == this.images.Count - 1)
            {
                nxtButton.IsEnabled = false;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Hide();
                gw.Show();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void prevButton_Click(object sender, RoutedEventArgs e)
        {
            if(position - 1 == 0)
            {
                prevButton.IsEnabled = false;
            }
            if(position - 1 == images.Count - 2)
            {
                nxtButton.IsEnabled = true;
            }
            position -= 1;
            galeryImageZoom.Source = images[position];
        }

        private void nxtButton_Click(object sender, RoutedEventArgs e)
        {
            if (position + 1 == 1)
            {
                prevButton.IsEnabled = true;
            }
            if (position + 1 == images.Count - 1)
            {
                nxtButton.IsEnabled = false;
            }
            position += 1;
            galeryImageZoom.Source = images[position];
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            gw.Show();
        }

        private void copyClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetImage(images[position]);
            MessageBox.Show("Copied!");
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            
            SaveFileDialog sf = new SaveFileDialog();
            sf.FileName = imageNames[position]; // Default file name
            sf.DefaultExt = ".png"; // Default file extension
            sf.Filter = "PNG(*.PNG)|*.png";
            if (sf.ShowDialog() == true)
            {
                Save(images[position], sf.FileName);
                MessageBox.Show("Saved!");
            }
        }

        public void Save(BitmapImage image, string filePath)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                encoder.Save(fileStream);
            }
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
