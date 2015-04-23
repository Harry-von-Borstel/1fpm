using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
//using System.Windows.Controls.Ribbon;
using System.Windows.Media;

namespace Controls
{
    public partial class Generic
    {
        //private void buttonChildren_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    var dob = (DependencyObject)sender;
        //    do
        //    {
        //        dob = VisualTreeHelper.GetParent(dob);
        //    } while (dob != null && !(dob is ListBoxItem));
        //    var lbi = dob as ListBoxItem;
        //    var so = lbi.Content as ShellFolder;
        //    var button = (Button)sender;
        //    //button.ContextMenu.Items.Clear();
        //    button.ContextMenu = new ContextMenu();
        //    do
        //    {
        //        dob = VisualTreeHelper.GetParent(dob);
        //    } while (dob != null && !(dob is BreadCrumb));
        //    foreach (var a in so.Where(so1 => so1 is ShellFolder))
        //    {
        //        var rmi = new MenuItem();
        //        rmi.Click += BreadCrumb.mi_Click;
        //        rmi.Tag = new object[] { dob, a };
        //        rmi.Header = a.ToString();
        //        button.ContextMenu.Items.Add(rmi);
        //    }
        //    button.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
        //    button.ContextMenu.IsOpen = true;
        //}

        private void buttonChildren_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var dob = (DependencyObject)sender;
            do
            {
                dob = VisualTreeHelper.GetParent(dob);
            } while (dob != null && !(dob is ListBoxItem));
            var lbi = dob as ListBoxItem;
            var so = lbi.Content as ShellFolder;
            if (so == null) return;
            var button = (MenuItem)sender;
            if (button.Items.Count > 0) return;
            button.Items.Clear();
            do
            {
                dob = VisualTreeHelper.GetParent(dob);
            } while (dob != null && !(dob is BreadCrumb));
            foreach (var a in so.Where(so1 => so1 is ShellFolder))
            {
                var rmi = new MenuItem();
                rmi.Click += BreadCrumb.mi_Click;
                rmi.Tag = new object[] { dob, a };
                rmi.Header = a.ToString();
                button.Items.Add(rmi);
            }
            //button.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            //button.ContextMenu.IsOpen = true;
        }

        //private void buttonChildren_DropDownOpened(object sender, EventArgs e)
        //{
        //    var dob = (DependencyObject)sender;
        //    do
        //    {
        //        dob = VisualTreeHelper.GetParent(dob);
        //    } while (dob != null && !(dob is ListBoxItem));
        //    var lbi = dob as ListBoxItem;
        //    var so = lbi.Content as ShellFolder;
        //    var ribbonMenuButton = (RibbonMenuButton)sender;
        //    ribbonMenuButton.Items.Clear();
        //    do
        //    {
        //        dob = VisualTreeHelper.GetParent(dob);
        //    } while (dob != null && !(dob is BreadCrumb));
        //    foreach (var a in so.Where(so1=>so1 is ShellFolder))
        //    {
        //        var rmi = new RibbonMenuItem();
        //        rmi.Click +=BreadCrumb.rmi_Click;
        //        rmi.Tag = new object[] {dob,a};
        //        rmi.Header = a.ToString();
        //        ribbonMenuButton.Items.Add(rmi);
        //    }
        //}

        private void buttonChildren_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void buttonChildren_SubmenuOpened(object sender, RoutedEventArgs e)
        {

        }

        private void buttonChildren_Checked(object sender, RoutedEventArgs e)
        {
            const int INITIALIZED = 1;
            var dob = (DependencyObject)sender;
            do
            {
                dob = VisualTreeHelper.GetParent(dob);
            } while (dob != null && !(dob is ListBoxItem));
            var lbi1 = dob as ListBoxItem;
            if (lbi1.Tag == null)
            {
                lbi1.Tag = INITIALIZED;
                var so = lbi1.Content as ISimpleNode;
                //var button = (ToggleButton)sender;
                ////button.ContextMenu.Items.Clear();
                //button.ContextMenu = new ContextMenu();

                dob = (DependencyObject)sender;
                dob = LogicalTreeHelper.GetParent(dob);  // Grid
                dob = LogicalTreeHelper.GetChildren(dob).Cast<DependencyObject>().ElementAt(1); // Popup
                dob = LogicalTreeHelper.GetChildren(dob).Cast<DependencyObject>().First();    // Grid
                dob = LogicalTreeHelper.GetChildren(dob).Cast<DependencyObject>().First();    // ListBox
                var listBox = (ListBox)dob;

                listBox.Items.Clear();  // Remove dummy item
                dob = (DependencyObject)sender;
                do
                {
                    dob = VisualTreeHelper.GetParent(dob);
                } while (dob != null && !(dob is BreadCrumb));
                foreach (var a in so.Where(so1 => so1 is ISimpleNode))
                {
                    var lbi = new ListBoxItem();
                    lbi.Selected += BreadCrumb.lbi_Selected;
                    lbi.Tag = dob;
                    lbi.Content = a;
                    listBox.Items.Add(lbi);
                }
            }
        }



    }
}
