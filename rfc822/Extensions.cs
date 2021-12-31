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
using System.IO;
using System.Linq;
using System.Text;

namespace blueshell.rfc822
{
	public static class Extensions
	{
		public static HeaderFieldBody ToHeaderField(this object arg)
		{
			return 
				arg == null 
					? null 
					: new HeaderFieldBody(arg.ToString());
		}

		/// <summary>
		/// Reads a message line
		/// </summary>
		/// <param name="sr">Input.</param>
		/// <param name="lineCounter">Line counter for diagnostic purposes.</param>
		/// <returns>The message line, excluding the terminating CRLF</returns>
		/// <remarks>
		/// As defined in RFC 2045, chapter 2.10. lines are sequences of octets separated by a CRLF sequences.
		/// </remarks>
		public static string ReadMessageLine(this TextReader sr, ref int lineCounter)
		{
			var sb = new StringBuilder();
			while (sr.Peek() != -1)
			{
				var c = (char)sr.Read();
				if (c == '\r' && sr.Peek() == (int)'\n')
				{
					sr.Read();  // Discard LF
					break;
				}
				sb.Append(c);
			}
			// CRLF found or at end of file
			lineCounter++;
			return sb.ToString();
		}

		/// <summary>
		/// Writes the line to the stream appending a CRLF
		/// </summary>
		/// <param name="sw">The output stream.</param>
		/// <param name="line">The line.</param>
		public static void WriteMessageLine(this TextWriter sw, string line)
		{
			sw.WriteMessage(line);
			sw.Write("\r\n");
		}

		public static void WriteMessage(this TextWriter sw, string line)
		{
			//TODO: Folding
			sw.Write(line);
		}

		/// <summary>
		/// Reads and unfolds a full RFC822 field line
		/// </summary>
		/// <param name="sr">Input.</param>
		/// <param name="lineCounter">Line counter used for diagnostic messages.</param>
		/// <returns>The unfolded full RFC822 field line</returns>
		public static string ReadFullFieldLine(this TextReader sr, ref int lineCounter)
		{
			if (sr.Peek() != -1)
			{
				bool skipNonRfcHeader = (lineCounter == 0);
				string line;
				do
				{
					line = sr.ReadMessageLine(ref lineCounter);
					skipNonRfcHeader = skipNonRfcHeader && IsNonRfcHeader(line);
				} while (skipNonRfcHeader);
				var sb1 = new StringBuilder(line);
				while (new[] { ' ', '\t' }.Contains((char)sr.Peek()))
				{   // folded line
					sr.Read();
					sb1.Append(" ");	// Change TAB to SPACE 
					sb1.Append(sr.ReadMessageLine(ref lineCounter));
				}
				return sb1.ToString();
			}
			return "";
		}

		/// <summary>
		/// Whether the line contains a non-RFC-header
		/// </summary>
		/// <param name="line">The line.</param>
		/// <returns>Whether the line contains a non-RFC-header</returns>
		/// <remarks>Some clients are producing *.eml files containing these headers.</remarks>
		private static bool IsNonRfcHeader(string line)
		{
			return line.StartsWith("From - ");	
		}

		public static string Fold(this string line)
		{
			var rest = line;
			var result = new StringBuilder();
            bool found = true;
            while (rest.Length > 78 && found)
			{
                found = false;
				for (int i = 77; i > 0; i--)
				{
					if (new[] { ' ', '\t' }.Contains(rest[i]))
					{
						result.Append(rest.Substring(0, i));
						result.Append("\r\n");
						rest = rest.Substring(i);
                        found = true;
						break;
					}
				}
			}
			result.Append(rest);
			return result.ToString();
		}

	}
}
