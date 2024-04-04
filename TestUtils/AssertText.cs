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
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace TestUtils
{
	public static class AssertText
	{
		public static void BytesAreEqual(byte[] exp, byte[] act,  string expSource=null, string actSource = null)
		{
			int lineCount = 1;
			int charCount = 1;
			var expLength = exp == null ? 0 : exp.Length;
			var actLength = act == null ? 0 : act.Length;
			var minLength = Math.Min(expLength, actLength);
			expSource = EnrichSource(expSource);
			actSource = EnrichSource(actSource);
			for (int i = 0; i < minLength; i++)
			{
				if (exp[i] != act[i])
				{
					int from = i - 10;
					if (from < 0) from = 0;
					throw new AssertFailedException(
						$"AssertFile.BytesAreEqual failed at byte {i}, line {lineCount}, char {charCount}.\r\n Expected (from byte {from}{expSource}):\r\n{RenderBytes(exp, from, i)}\r\n Actual (from byte {from}{actSource}):\r\n{RenderBytes(act, from, i)}");
				}
				if (act[i] == (byte)'\n')
				{
					lineCount++;
					charCount = 1;
				}
				else
					charCount++;
			}
			if (expLength != actLength || ((exp == null) != (act == null)))
			{
				int from = minLength - 10;
				if (from < 0) from = 0;
				throw new AssertFailedException(
					   string.Format(
						   "AssertFile.AreEqual failed.\r\n Expected array length: {0} (from byte {4}):\r\n{2}\r\n Actual array length: {1} (from byte {4}):\r\n{3}",
						   expLength,
						   actLength, RenderBytes(exp, from, minLength), RenderBytes(act, from, minLength), from));
			}

			static string EnrichSource(string source)
			{
				return source == null
					? null
					: $" of \"{source}\"";
			}
		}

		/// <summary>
		/// Checks whether two files are equal.
		/// </summary>
		/// <param name="expectedFile">The file that is expected.</param>
		/// <param name="actualFile">The file that is checked.</param>
		public static void FilesAreEqual(string expectedFile, string actualFile, 
			byte[] bytesToReplace = null, byte[] replacementBytes = null)
		{
			if (bytesToReplace == null && replacementBytes != null)
			{
				throw new ArgumentNullException(nameof(bytesToReplace));
			}
			if (bytesToReplace != null && replacementBytes == null)
			{
				throw new ArgumentNullException(nameof(replacementBytes));
			}
			var exp = File.ReadAllBytes(expectedFile);
			if (bytesToReplace!=null)
				exp = Replace(exp);
			var act = File.ReadAllBytes(actualFile);
			BytesAreEqual(exp, act, expectedFile, actualFile);
			Console.WriteLine("Checked \"{0}\"", actualFile);

			return;

			byte[] Replace(byte[] original)
			{
				var i = original.AsSpan().IndexOf(bytesToReplace);
				if (i < 0)
				{
					return original;
				}
				var r = original[..i]
					.Concat(replacementBytes)
					.Concat(original[(i+replacementBytes.Length)..])
					.ToArray();
				return Replace(r);
			}
		}

		/// <summary>
		/// Verifies that all files found in <paramref name="actualFolder"/> are found equally in <paramref name="expectedFolder"/>.
		/// </summary>
		/// <param name="expectedFolder">The folder that holds the files expected.</param>
		/// <param name="actualFolder">The folder that is checked.</param>
		public static void FoldersAreEqual(string expectedFolder, string actualFolder)
		{
			FoldersAreEqual(new DirectoryInfo(expectedFolder), new DirectoryInfo(actualFolder));
		}

		/// <summary>
		/// Verifies that all files found in <paramref name="actualFolder"/> are found equally in <paramref name="expectedFolder"/>.
		/// </summary>
		/// <param name="expectedFolder">The folder that holds the files expected.</param>
		/// <param name="actualFolder">The folder that is checked.</param>
		public static void FoldersAreEqual(DirectoryInfo expectedFolder, DirectoryInfo actualFolder)
		{
			foreach (var fileSystemInfo in expectedFolder.EnumerateFileSystemInfos())
			{
				if (fileSystemInfo.Attributes.HasFlag(FileAttributes.Directory))
				{
					var actSub = actualFolder.GetDirectories(fileSystemInfo.Name);
					Assert.AreEqual(1, actSub.Length, string.Format("The folder {0}\\{1} is missing.", actualFolder.FullName, fileSystemInfo.Name));
					FoldersAreEqual((DirectoryInfo)fileSystemInfo, actSub[0]);
				}
				else
				{
					FilesAreEqual(fileSystemInfo.FullName, Path.Combine(actualFolder.FullName, fileSystemInfo.Name));
				}
			}
		}

		/// <summary>
		/// Verifies that two strings are equal.
		/// </summary>
		/// <param name="expected">The string that is expected.</param>
		/// <param name="actual">The string that is checked.</param>
		public static void StringsAreEqual(string expected, string actual)
		{
			var encoding =
				((expected == null || expected.All(c => c < '\x80'))
				 && (actual == null || actual.All(c => c < '\x80')))
					? Encoding.ASCII
					: Encoding.Unicode;
			var exp = expected == null ? null : encoding.GetBytes(expected);
			var act = actual == null ? null : encoding.GetBytes(actual);
			BytesAreEqual(exp, act);
		}

		private static string RenderBytes(byte[] data, int offset, int marker)
		{
			if (data == null)
				return "«null»";
			var sOut = offset > 0 ? "..." : "";
			var max = Math.Min(offset + 40, data.Length);
			for (var i = offset; i < max; i++)
			{
				sOut += $"{(i == marker ? '►' : ' ')} {(data[i] >= 32 ? (char)data[i] : '?')}";
			}
			sOut += "\r\n" + (offset > 0 ? "..." : "");
			for (var i = offset; i < max; i++)
			{
				sOut += $"{(i == marker ? '►' : ' ')}{data[i]:x2}";
			}
			return sOut;
		}
	}
}
