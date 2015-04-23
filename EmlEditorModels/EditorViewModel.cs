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
using blueshell.rfc822;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypesaveViewModel;
using Microsoft.WindowsAPICodePack.Shell;

namespace EmlEditorModels
{
	public class EditorViewModel
	{
		public readonly Connector<Message> MessageConnector = new Connector<Message>();
		public readonly Connector<string> SubjectConnector = new Connector<string>();
        public readonly Connector<ShellFolder> TopFolderConnector = new Connector<ShellFolder>();
        public readonly Connector<ShellFolder> CurrentFolderConnector = new Connector<ShellFolder>();

		private string filename;

		public EditorViewModel()
		{
			SubjectConnector.OnValueChanged = 
				() => 
					MessageConnector.Value.HeaderFields.Subject = SubjectConnector.Value;
			MessageConnector.OnValueChanged =
				() =>
					SubjectConnector.Value = MessageConnector.Value.HeaderFields.Subject;
		}

		public void ReadFile(string filename)
		{
			this.filename = filename;
			var m = new Message();
			m.FromFile(filename);
			this.MessageConnector.Value = m;
		}

		public void Save()
		{
			this.MessageConnector.Value.ToFile(this.filename);
		}
	}
}
