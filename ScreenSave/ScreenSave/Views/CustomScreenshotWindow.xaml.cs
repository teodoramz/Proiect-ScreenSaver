using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for CustomScreenshotWindow.xaml
    /// </summary>
    public partial class CustomScreenshotWindow : Window
    {
        private ScreenshotMainWindow sm;
        private string currUserID;
        public CustomScreenshotWindow(ScreenshotMainWindow sm, string userID)
        {
            InitializeComponent();
            string iconUri = "..\\..\\..\\Resources\\ViewsResources\\logo.png";
            this.Icon = BitmapFrame.Create(new Uri(iconUri, UriKind.Relative));
            this.sm = sm;
            currUserID = userID;
           
        }
        public double GetWindowsScaling()
        {
            double factor = System.Windows.PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice.M11;
            if (AppHelper.ScreenH.MultipleMonitors())
            {
                return 1;
            }
            return factor;
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.E)
            {
                this.Hide();
                Thread.Sleep(1000);
                //fac ss la dimensiunea selectata, returnez imaginea in screenshow main window
                double left = this.Left;
                double top = this.Top;
                double height = (int)this.Height;
                double width = (int)this.Width;

                double scale = GetWindowsScaling();

                width = width * scale;
                height = height * scale;

                

                Bitmap bitmap_screen = new Bitmap((int)width, (int)height);
                Graphics g = Graphics.FromImage(bitmap_screen);
                string uri = Environment.CurrentDirectory + "\\temp2.png";
                if (File.Exists(uri))
                {
                    File.Delete(uri);
                }


                g.CopyFromScreen((int)left, (int)top, 0, 0, bitmap_screen.Size);
                

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

                var smw = new ScreenshotMainWindow(currUserID, bitmap);
                smw.Show();
                smw.setTempImg(uri);

            }
            if(e.Key == Key.Escape)
            {
                sm.Show();
                this.Hide();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScreenshotMainWindow smw = new ScreenshotMainWindow("teodor");
            smw.Show();
            this.Hide();
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
