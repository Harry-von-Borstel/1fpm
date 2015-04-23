/*
    1fpm -- One File Per Message - File oriented mail system
    Copyright (C) 2015  blueshell Software Engineering Harry von Borstel 
    (http://www.blueshell.com)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Controls
{
    public class BreadCrumb :
        ListBox
    {
        public BreadCrumb()
        {

        }

        private Type adaptor;

        public Type Adaptor
        {
            get { return adaptor; }
            set { adaptor = value; }
        }

        private ISimpleNode topFolderSimpleNode;
        private ISimpleNode currentFolderSimpleNode;
        


        public object TopFolder
        {
            get { return (object)GetValue(TopFolderProperty); }
            set { SetValue(TopFolderProperty, value); }
        }

        public static readonly DependencyProperty TopFolderProperty =
            DependencyProperty.Register("TopFolder", typeof(object), typeof(BreadCrumb), new PropertyMetadata(null, new PropertyChangedCallback(OnRangeChanged)));




        public object CurrentFolder
        {
            get { return (object)GetValue(CurrentFolderProperty); }
            set { SetValue(CurrentFolderProperty, value); }
        }

        public static readonly DependencyProperty CurrentFolderProperty =
            DependencyProperty.Register("CurrentFolder", typeof(object), typeof(BreadCrumb), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnRangeChanged)));

        static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dd = (BreadCrumb)d;
            var newSn = e.NewValue as ISimpleNode;
            if (dd.Adaptor != null && e.NewValue != null)
            {
                if (newSn == null)
                {
                    newSn = (ISimpleNode)dd.Adaptor.GetConstructor(new[] { typeof(object) })
                        .Invoke(new[] { e.NewValue });
                }
            }
            if (e.Property == BreadCrumb.TopFolderProperty)
                dd.topFolderSimpleNode = newSn;
            else
                dd.currentFolderSimpleNode = newSn;
            if (dd.TopFolder == null || dd.CurrentFolder == null)
                return;
            ISimpleNode folder = dd.currentFolderSimpleNode;
            dd.Items.Clear();
            do
            {
                dd.Items.Insert(0, folder);
                if (folder.IsEqual( dd.TopFolder))
                    break;
                folder = folder.Parent;
            } while (folder != null);
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            var si = this.SelectedItem as ISimpleNode;
            if (si!=null) this.CurrentFolder = ToPublicType( si);
            base.OnSelectionChanged(e);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        private static object ToPublicType(ISimpleNode sn)
        {
            var adaptor = sn as Adaptor;
            if (adaptor != null)
                return adaptor.Adaptee;
            else
                return sn;
        }

        public static void mi_Click(object sender, RoutedEventArgs e)
        {
            var mi = (MenuItem)sender;
            var bc = (BreadCrumb)(((object[])(mi.Tag))[0]);
            var sf = (ISimpleNode)(((object[])(mi.Tag))[1]);
            bc.CurrentFolder = ToPublicType(sf);
        }

        internal static void lbi_Selected(object sender, RoutedEventArgs e)
        {
            var mi = (ListBoxItem)sender;
            var bc = (BreadCrumb)mi.Tag;
            var sf = (ISimpleNode)mi.Content;
            bc.CurrentFolder = ToPublicType(sf);
        }


        public static void RegisterMenuButton(object sender)
        {
            var dob = (DependencyObject)sender;
            do
            {
                dob = VisualTreeHelper.GetParent(dob);
            } while (dob != null && !(dob is ListBoxItem));
            var lbi = dob as ListBoxItem;
            var so = lbi.Content as ISimpleNode;
            if (so == null) return;
            var button = (MenuItem)sender;
            if (button.Items.Count > 0) return;
            button.Items.Clear();
            do
            {
                dob = VisualTreeHelper.GetParent(dob);
            } while (dob != null && !(dob is BreadCrumb));
            foreach (var a in so.Where(so1 => so1 is ISimpleNode))
            {
                var rmi = new MenuItem();
                rmi.Click += BreadCrumb.mi_Click;
                rmi.Tag = new object[] { dob, a };
                rmi.Header = a.ToString();
                button.Items.Add(rmi);
            }

        }

    }
}
