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

namespace Controls
{
    /// <summary>
    /// Interaction logic for WindowDesignBreadCrumb.xaml
    /// </summary>
    public partial class WindowDesignBreadCrumb : Window
    {
        public WindowDesignBreadCrumb()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Format("bc.TopFolder={0}\nbc.CurrentFolder={1}", this.bc.TopFolder, this.bc.CurrentFolder));
        }
    }
}
