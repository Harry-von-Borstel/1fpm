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
using System.IO;
using System.Linq;
using System.Text;
using XFiles;

namespace blueshell.rfc822
{
	public class Message: MessagePart
	{
		internal int LineCounter;

		static Message()
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		}

		public Message()
		{
			this.HeaderFields.Date = DateTime.Now;
		}

		/// <summary>
		/// Reads a message from a file
		/// </summary>
		/// <param name="filename">Path of the *.eml file.</param>
		/// <returns>Successfulness</returns>
		public bool FromFile(string filename)
		{
			LineCounter = 0;
			using (var sr =
				System.IO.File.Exists(filename)
				? new StreamReader(filename, Encoding.GetEncoding(28605))
				: null)
			{
				return Read(sr, ref LineCounter, null, null, filename + ".parts") == ReadResult.FileFinished;
			}
		}

		/// <summary>
		/// Writes the message to a file
		/// </summary>
		/// <param name="filename">Path of the *.eml file.</param>
		public void ToFile(string filename)
		{
			LineCounter = 0;
			Directory.GetParent(filename).Create();
			using (var sr = new StreamWriter(filename,false, Encoding.GetEncoding(28605)))
			{
				Write(sr, ref LineCounter, null);
				return;
			}
		}

		/// <summary>
		/// Adds a file to the Message.
		/// </summary>
		/// <remarks>
		/// The filename must exist in the Message's unpack folder or subfolders.
		/// </remarks>
		/// <param name="filename">Name of the file to add to the message.</param>
		public void AddFile(string filename)
		{
			var file = new XFile(filename);
			if (this.GetPart(file) == null)
				throw new ApplicationException("The file must exist in the Message's unpack folder or subfolders.");
		}
	}
}
