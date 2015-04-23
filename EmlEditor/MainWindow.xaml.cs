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
using Controls;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Controls.WindowsPresentationFoundation;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TypesaveViewModel.WpfBinding;

namespace EmlEditor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			this.textBoxSubject.BindText(this.viewModel.SubjectConnector);
            this.breadCrumb.Adaptor = typeof(ShellFolderAdaptor);
            this.breadCrumb.Bind<ShellFolder>(this.viewModel.TopFolderConnector, BreadCrumb.TopFolderProperty);
            this.breadCrumb.Bind<ShellFolder>(this.viewModel.CurrentFolderConnector, BreadCrumb.CurrentFolderProperty);
            this.explorerBrowser.Bind<ShellFolder>(this.viewModel.CurrentFolderConnector, ExplorerBrowser.NavigationTargetProperty, bindingMode: BindingMode.TwoWay);
        }

		private void buttonSave_Click(object sender, RoutedEventArgs e)
		{
			this.viewModel.Save();
		}

        private void buttonOpen_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
                {
                    var ofd = new OpenFileDialog();
                    if (ofd.ShowDialog(this) == true)
                    //var ofd = new CommonOpenFileDialog();
                    //if (ofd.ShowDialog(this) == CommonFileDialogResult.Ok)
                    {
                        var fn = ofd.FileName;
                        this.viewModel.ReadFile(fn);
                        var fn2 = this.viewModel.MessageConnector.Value.Filename;
                        ShellObject so;
                        if (Directory.Exists(fn2))
                            so = ShellFolder.FromParsingName(fn2);
                        else
                        {
                            ShellFile sf = ShellFile.FromFilePath(fn2);
                            so = sf.Parent;
                        }
                        this.viewModel.TopFolderConnector.Value = (ShellFolder)so;
                        this.viewModel.CurrentFolderConnector.Value = (ShellFolder)so;
                    }
                }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.explorerBrowser.ExplorerBrowserControl.NavigationComplete += ExplorerBrowserControl_NavigationComplete;
        }
        void ExplorerBrowserControl_NavigationComplete(object sender, Microsoft.WindowsAPICodePack.Controls.NavigationCompleteEventArgs e)
        {
            if (this.explorerBrowser.NavigationTarget != e.NewLocation)
                this.explorerBrowser.NavigationTarget = e.NewLocation;
        }
    }
}
