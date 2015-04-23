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
using System.IO;
using System.Linq;
using System.Text;

namespace XFiles
{
	public class XFile
	{
		private string fullName;
		public XFile(string fileName)
		{
			this.fullName = Path.GetFullPath(fileName);
		}

		public override bool Equals(object obj)
		{
			var oXFile = obj as XFile;
			return oXFile==null
				? false
				: this.fullName==oXFile.fullName;
		}

		public override int GetHashCode()
		{
			return this.fullName.GetHashCode();
		}

		public string FullName { get { return fullName; } }

		public XFolder Folder
		{
			get
			{
				return new XFolder(Path.GetDirectoryName(fullName));
			}
		}

		/// <summary>
		/// If the given child file might be contained in this file.
		/// </summary>
		/// <remarks>
		/// <paramref name="childFile"/> is not required to really exist in order to return true.
		/// </remarks>
		/// <param name="childFile"></param>
		/// <returns></returns>
		public bool CanContain(XFile childFile)
		{
			return childFile.FullName.StartsWith(this.FullName);
		}

		public string Name { get { return Path.GetFileName(fullName); } }
	}
}
