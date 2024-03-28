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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace blueshell.rfc822
{
	/// <summary>
	/// The body (i.e. value) of a header field
	/// </summary>
	/// <remarks><see cref="http://tools.ietf.org/html/rfc822#section-3.2"/></remarks>
	public class HeaderFieldBody : IHeaderFieldBody
	{
		private readonly string[] content;
		public HeaderFieldBody(params string[] contentItems)
		{
			var decodedItems = new List<string>();
			if (contentItems != null)
				foreach (var contentItem in contentItems)
				{
					decodedItems.Add(DecodeWords(contentItem));
				}
			this.content = decodedItems.ToArray();
		}

		private string DecodeWords(string contentItem)
		// RFC2047 decoding https://www.ietf.org/rfc/rfc2047.txt
		{
			var result = "";
			var delim = "";
			var lastWordWasEncoded = false;
			foreach (var word in contentItem.Split(new[] { ' ' }))
			{
				foreach (var word2 in word.Split(new[] { '\t' }))
				{
					var e1 = lastWordWasEncoded;
					var w = DecodeWord(word2, ref lastWordWasEncoded);
					var e2 = lastWordWasEncoded;
					var d =
						e1 && e2
							? ""
							: delim;
					result += d + w;
					delim = "\t";
				}
				delim = " ";
			}
			return result;
		}

		internal static string DecodeWord(string word, ref bool wordWasEncoded)
		{
			var lastWordWasEncoded = wordWasEncoded;
			wordWasEncoded = false;
			var encodedWord = word.Trim();
			if (encodedWord.Length > 75)
				return word;
			var parts = encodedWord.Split('?');
			if (parts.Length != 5 || parts[0] != "=" || parts[4] != "=")
				return word;
			switch (parts[2])
			{
				case "Q":
					break;
				case "B":
					// sorry, maybe later...
					return word;

				default:
					return word;
			}

			try
			{
				var encoding = Encoding.GetEncoding(parts[1]);
				var data = parts[3];
				var result = "";
				for (int i = 0; i < data.Length; i++)
				{
					var c = data[i];
					switch (c)
					{
						case '=':
							if (!Regex.IsMatch(data.Substring(i + 1), "^[0-9A-Fa-f]{2}"))
								return word;
							var b = byte.Parse("00" + data.Substring(i + 1, 2), NumberStyles.AllowHexSpecifier);
							result += encoding.GetString(new[] { b });
							i += 2;
							break;

						case '_':
							result += " ";
							break;

						default:
							result += c;
							break;
					}
				}
				wordWasEncoded = true;
				return result;
			}
			catch
			{
				return word;
			}
		}

		/// <summary>
		/// Returns the content string lines of the header field body
		/// </summary>
		/// <returns>The content string lines of the header field. If there is no content, null is returned.</returns>
		public override string ToString()
		{
			return
				content == null || content.Length == 0
					? ""
					: content.Aggregate((total, next) => total + "\r\n" + next);
		}

		/// <summary>
		/// Returns all lines of the header field
		/// </summary>
		/// <param name="fieldName">Name of the header field</param>
		/// <returns>All lines of the header field, consisting of field-name, colon, field-body and trailing CRLF. Folding is applied, thus no line exceeds the 78+2 chars limit. Each occurence of the header field yiels a CRLF-terminated line in RFC 822 format. . If there is no content, null is returned.</returns>
		public string ToString(string fieldName)
		{
			return
				content == null || content.Length == 0
					? null
					: content.Aggregate(
					"",
					(total, next) =>
						total +
						(string.Format("{0}:\t{1}\r\n", fieldName, EncodeAsWords(next))).Fold());
		}


		private static string EncodeAsWords(string s)
		// RFC2047 encoding https://www.ietf.org/rfc/rfc2047.txt
		{
			if (!(HasSpecialChar(s) || NeedsEncodingForFolding(s)))
				return s;
			var encoding = Encoding.GetEncoding("iso-8859-15");
			string word = $"=?{encoding.BodyName}?Q?";
			string result = "";
			const int maxword = 75; // the max size allowed for encoded-word
			const int maxlenchar = 3;   // The max length of an encoded char
			const int lenpostfix = 2; // The length of "?="
			foreach (var c in s)
			{
				if (word.Length >= maxword - maxlenchar - lenpostfix)
				{
					// fold it (i.e. split in order to make it foldable)
					result = $"{result}{word}?= ";
					word = $"=?{encoding.BodyName}?Q?";
				}
				switch (c)
				{
					case ' ':
						word = word + "_";
						break;

					case '=':
					case '?':
					case '\t':
					case '_':
						var bytes = encoding.GetBytes(new char[] { c });
						foreach (var b in bytes)
						{
							word = word + $"={b:X2}";
						}
						break;

					default:
						if (c > '~')
							goto case '=';
						word = word + c;
						break;
				}
			}
			return $"{result}{word}?=";

			static bool HasSpecialChar(string s)
			{
				return s.Any(c => c > '~' || (c < ' ' && c != '\r' && c != '\n' && c != '\t'));
			}

			static bool NeedsEncodingForFolding(string s)
			{
				if (s.Length <= 75)
					return false;
				var a = s.Split(' ', '\t');
				return a.Any(s1 => s1.Length > 75);
			}
		}

	}
}
