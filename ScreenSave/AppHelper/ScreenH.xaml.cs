using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AppHelper
{
    /// <summary>
    /// Interaction logic for ScreenH.xaml
    /// </summary>
    public partial class ScreenH : Window
    {
        public ScreenH()
        {
            //InitializeComponent();
        }

        public static bool MultipleMonitors()
        {
            if(Screen.AllScreens.Length > 1)
            {
                return true;
            }
            return false;
        }
    }
}
