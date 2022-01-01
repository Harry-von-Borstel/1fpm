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
using System.Net.Mime;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using XFiles;

namespace blueshell.rfc822
{
	public class MessagePart
	{
		protected MessagePart()
		{
			this.HeaderFields = new HeaderFields();
		}


		public HeaderFields HeaderFields { get; set; }

		public bool IsMultiPart
		{
			get
			{
				return this.HeaderFields.ContentTypeEffective.MediaType.StartsWith("multipart/", StringComparison.OrdinalIgnoreCase);
			}
		}

		private readonly IList<MessagePart> innerParts = new List<MessagePart>();

		public IList<MessagePart> InnerParts
		{
			get { return innerParts; }
		}

		public Encoding Charset
		{
			get
			{
				var cs = HeaderFields.ContentTypeEffective.CharSet;
				return cs == null
					? Encoding.ASCII
					: Encoding.GetEncoding(cs);
			}
		}

		/// <summary>
		/// The filename of the unpacked MessagePart.
		/// </summary>
		/// <remarks>
		/// If MultiPart, this would be a folder. This filename might be relative to the current directory.
		/// </remarks>
		public string Filename { get; private set; }

		private XFile file;
		public XFile File { get { return file ?? (file = new XFile(Filename)); } }

		/// <summary>
		/// The text of the message part
		/// </summary>
		/// <remarks>For multipart message parts, this is null.</remarks>
		public string Text { get; set; }

		public enum ReadResult
		{
			Failed,
			FileFinished,
			MultipartFinished,
			MultipartOngoing,
		}

		/// <summary>
		/// Reads the MessagePart from a stream
		/// </summary>
		/// <param name="emlReader">The stream to read from. If this is `null`, an empty MessagePart will be returned.</param>
		/// <param name="lineCounter">The line counter for the input stream.</param>
		/// <param name="parentContentType"></param>
		/// <param name="parent">The parent MessagePart. Null for top level MessagePart.</param>
		/// <param name="parentFolder">The "unpack" folder for the message. Null on nested MessageParts.</param>
		/// <returns></returns>
		internal ReadResult Read(StreamReader emlReader, ref int lineCounter, ContentType parentContentType, MessagePart parent = null, string parentFolder = null
			)
		{
			bool headerReady = false;
			bool sendTextToEpilogue = false;
			FileStream fileOutput = null;
			var line = "";
			byte[] bCrLf = new[] { (byte)'\r', (byte)'\n' };

			try
			{
				if (emlReader == null)
                {
					SetTempStorage();
					return ReadResult.FileFinished;
                }
				while (!emlReader.EndOfStream)
				{
					if (!headerReady)
					{
						line = emlReader.ReadFullFieldLine(ref lineCounter);
						if (line.Length == 0)
                        {
                            headerReady = true;
                            SetTempStorage();
                        }
                        else
							AddHeaderField(line);
					}
					else
					{
						line = emlReader.ReadMessageLine(ref lineCounter);
						if (IsBoundary(line, parentContentType, ""))
						{
							return ReadResult.MultipartOngoing;
						}
						else if (IsBoundary(line, parentContentType, "--"))
						{
							return ReadResult.MultipartFinished;
						}
						else if (IsBoundary(line, HeaderFields.ContentType, ""))
						{
							ReadResult rr;
							do
							{
								var mpi = new MessagePart();
								this.InnerParts.Add(mpi);
								rr = mpi.Read(emlReader, ref lineCounter, HeaderFields.ContentType, this);
								if (rr == ReadResult.Failed)
									return ReadResult.Failed;
							} while (rr == ReadResult.MultipartOngoing);
							//Text += string.Format("<<This multipart has {0} parts>>", this.InnerParts.Count);
							//doLineBreak = false;
							sendTextToEpilogue = true;
						}
						else if (IsBoundary(line, parentContentType, "--"))
						{
							return ReadResult.MultipartFinished;
						}
						else
						{
							byte[] data;
							bool leadingLineBreak = false, trailingLineBreak=false;
							if (sendTextToEpilogue)
							{
								data = Decode(line);
								trailingLineBreak = FetchPendingLineBreak();
								this.Epilogue += new string(Encoding.ASCII.GetChars(data)) + (trailingLineBreak ? "\r\n" : "");
							}
							else
							{
								leadingLineBreak = FetchPendingLineBreak();
								data = Decode(line);
								var outText = (leadingLineBreak ? "\r\n" : "") + new string(Encoding.ASCII.GetChars(data));
								if (IsMultiPart)
								{
									this.Preamble += outText;
								}
								else
								{
									Text += outText;
								}
							}
							if (fileOutput != null)
							{
								if (leadingLineBreak)
									fileOutput.Write(bCrLf, 0, bCrLf.Length);
								fileOutput.Write(data, 0, data.Length);
								if (trailingLineBreak)
									fileOutput.Write(bCrLf, 0, bCrLf.Length);
							}
						}
					}
				}
			}

			catch (Exception e)
			{
				throw new ApplicationException(
					string.Format(@"Error parsing message part at line {0}: ""{1}""", lineCounter, line),
					e);
			}
			finally
			{
				if (fileOutput != null)
					fileOutput.Close();
			}
			return ReadResult.FileFinished;

            void SetTempStorage()
            {
                var folder = parentFolder ?? parent.Filename;
                this.Filename = Path.Combine(
                    folder,
                    HeaderFields.ContentDispositionFileName
                        ?? GetDefaultFilename(parent) + GetDefaultExtension()
                    );
                Directory.CreateDirectory(folder);
                if (!IsMultiPart)
                {
                    fileOutput = new FileStream(this.Filename, FileMode.Create);
                }

                return;
            }
        }

		/// <summary>
		/// Writes the message part to a stream
		/// </summary>
		internal ReadResult Write(StreamWriter emlWriter, ref int lineCounter, ContentType parentContentType, MessagePart parent = null
			)
		{
			FileStream fileInput = null;
			string line = "";
			try
			{
				emlWriter.WriteMessageLine(this.HeaderFields.ToString());
				if (this.IsMultiPart)
				{
					if (this.Preamble != null)
					{
						emlWriter.WriteMessageLine(this.Preamble);	// Preample
						lineCounter++;
					}
					foreach (var part in this.InnerParts)
					{
						emlWriter.Write("--");
						emlWriter.WriteMessageLine(this.HeaderFields.ContentType.Boundary);
						lineCounter++;
						part.Write(emlWriter, ref lineCounter, this.HeaderFields.ContentType, this);
						lineCounter++;
					}
					emlWriter.Write("--");
					emlWriter.Write(this.HeaderFields.ContentType.Boundary);
					emlWriter.WriteMessage("--");
					if (this.Epilogue != null)
					{
						emlWriter.WriteMessageLine("");
						lineCounter++;
						emlWriter.WriteMessage(this.Epilogue);
						lineCounter += this.Epilogue.Count(c => c == '\n');
					}
					else
					{
						emlWriter.WriteMessageLine("");
						lineCounter++;
					}
				}
				else
				{
					fileInput = new FileStream(this.Filename, FileMode.Open);
					do
					{
						var data = new byte[1026];
						int numBytesRead = fileInput.Read(data, 0, data.Length);
						if (numBytesRead == 0)
							break;
						line = Encode(data, numBytesRead, this.HeaderFields.ContentTransferEncoding, this.Charset);
						emlWriter.WriteMessage(line);
						lineCounter += line.Count(c => c == '\n');
					} while (true);
					if (this.HeaderFields.ContentTransferEncoding!= TransferEncoding.Base64)
						emlWriter.WriteMessageLine("");	// Always end with CRLF
				}
			}
			catch (Exception e)
			{
				throw new ApplicationException(
					string.Format(@"Error rendering message part at line {0}: ""{1}""", lineCounter, line),
					e);
			}
			finally
			{
				if (fileInput != null)
					fileInput.Close();
			}
			return ReadResult.FileFinished;
		}

		private string GetDefaultFilename(MessagePart parent)
		{
			if (this.HeaderFields.ContainsKey("Content-ID"))
			{
				var hcid = this.HeaderFields["Content-ID"]; // http://tools.ietf.org/html/rfc2045#section-7
				if (hcid != null)
				{
					var r = new Regex(Re.MSG_ID);
					var m = r.Match(hcid.ToString());
					return m.Groups[Re.Name.ADDR_SPEC].Value;
				}
			}

			return (parent != null)
				? parent.InnerParts.Count.ToString()
				: "1"
			;
		}

		/// <summary>
		/// Gets the default extension for the ContentType
		/// </summary>
		/// <returns>The default extension for the ContentType including the leading dot.</returns>
		private string GetDefaultExtension()
		{
			var key =
				Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + this.HeaderFields.ContentTypeEffective.MediaType, false);
			var value =
				(key != null)
					? key.GetValue("Extension")
					: null;
			return (value != null)
				? value.ToString()
				: string.Empty;
		}

		private static bool IsBoundary(string line, ContentType ct, string trailer)
		{
			return ct != null && line.TrimEnd(new[] { ' ', '\t' }) == "--" + ct.Boundary + trailer;
		}

		private void AddHeaderField(string line)
		{
			var r = new Regex(Re.FIELD);
			var m = r.Match(line);
			var fieldName = m.Groups[Re.Name.FIELD_NAME].Value;
			var fieldBody = m.Groups[Re.Name.FIELD_BODY].Value;
			HeaderFields.Add(fieldName, fieldBody);
		}
		private bool doLineBreak = false;

		private byte[] Decode(string line) { return Decode(line, this.HeaderFields.ContentTransferEncoding, this.Charset, out doLineBreak); }

		/// <summary>
		/// Decodes an unfolded message line
		/// </summary>
		/// <param name="line">The unfolded message line (without trailing CRLF).</param>
		/// <param name="contentTransferEncoding">The encoding of the line.</param>
		/// <param name="doLineBreak">Outputs whether a line break should precede the next decoded line.</param>
		/// <returns>The decoded line.</returns>
		internal static byte[] Decode(string line, TransferEncoding contentTransferEncoding, Encoding charset, out bool doLineBreak)
		{
			switch (contentTransferEncoding)
			{
				case TransferEncoding.Base64:
					{
						doLineBreak = false;
						byte[] data = Convert.FromBase64String(line);
						return data;
					}
				case TransferEncoding.QuotedPrintable:
					// see http://tools.ietf.org/html/rfc2045#section-6.7
					line = line.TrimEnd(new[] { ' ', '\t' });
					string res = "";
					for (int i = 0; i < line.Length; i++)
					{
						Char c = line[i];
						int hex;
						if (c == '='
							&& i < line.Length - 2
							&& int.TryParse(
								line.Substring(i + 1, 2),
								System.Globalization.NumberStyles.AllowHexSpecifier,
								null,
								out hex))
						{
							res += (char)hex;
							i += 2;
						}
						else
							res += c;
					}
					if (line.LastOrDefault() == '=')
					{
						doLineBreak = false;
						res= res.Substring(0, res.Length - 1);
					}
					else
					{
						doLineBreak = true;
					}
					return charset.GetBytes(res);
#if EIGHTBIT
				case TransferEncoding.EightBit:
#endif
				case TransferEncoding.SevenBit:
				case TransferEncoding.Unknown:
				default:
					{
						doLineBreak = true;
						return charset.GetBytes(line);
					}
			}
		}

		private bool FetchPendingLineBreak()
		{
			var res = doLineBreak;
			doLineBreak = false;
			return res;
		}

		/// <summary>
		/// Encode data
		/// </summary>
		/// <param name="data">The data to encode.</param>
		/// <param name="count">The number of bytes to be used from <paramref name="data"/>.</param>
		/// <param name="contentTransferEncoding">The encoding to be used.</param>
		/// <returns>The encoded data.</returns>
		internal static string Encode(byte[] data, int count, TransferEncoding contentTransferEncoding, Encoding charset)
		{
			switch (contentTransferEncoding)
			{
				case TransferEncoding.Base64:
					return Convert.ToBase64String(data, 0, count, Base64FormattingOptions.InsertLineBreaks)+"\r\n";

				case TransferEncoding.QuotedPrintable:
					// see http://tools.ietf.org/html/rfc2045#section-6.7
					var sbLine = new StringBuilder();
					int charPositionAtLine = 0;
					int positionTrailingWhitespace = count;
					if (" \t".Contains((char)data[count - 1]))
						positionTrailingWhitespace--;
					for (int i = 0; i < count; i++)
					{
						byte b = data[i];
						if (b==13 && i+1<count && data[i+1]==10)
						{
							// Preserve CRLF
							i++;
							sbLine.Append("\r\n");
							charPositionAtLine = 0;
							continue;
						}
						bool literal =
							(
								(b >= 33 && b <= 60)
								|| (b >= 62 && b <= 126)
								|| (
									(i != positionTrailingWhitespace)
									&& (b == 32 || b == 9)
								   )
							);
						var proposedCharPositionAtLine =
							charPositionAtLine +
							(literal ? 1 : 3);
						if (proposedCharPositionAtLine > 75)
						{
							// Insert Soft Line Break
							sbLine.Append("=\r\n");
							charPositionAtLine = 0;
						}
						if (literal)
						{
							sbLine.Append((char)b);
							charPositionAtLine++;
						}
						else
						{
							sbLine.Append(string.Format("={0:X2}", b));
							charPositionAtLine += 3;
						}
					}
					return sbLine.ToString();

#if EIGHTBIT
				case TransferEncoding.EightBit:
#endif
				case TransferEncoding.SevenBit:
				case TransferEncoding.Unknown:
				default:
					return charset.GetString(data, 0, count);
			}
		}

		/// <summary>
		/// The preamble of a multipart
		/// </summary>
		/// <value>The string that makes up the preamble or null, if no preample is present.</value>
		/// <remarks>The preamble is always automatically followed by a CRLF, if present. This CRLF is not part of the preamble and not contained in this property. <see cref="http://tools.ietf.org/html/rfc2046#section-5.1.1"/></remarks>
		public string Preamble { get; set; }

		/// <summary>
		/// The epilogue of a multipart
		/// </summary>
		/// <value>The string that makes up the epilogue or null, if no epilogue is present.</value>
		/// <remarks>The epilogue is always automatically preceded by a CRLF, if present. This CRLF is not part of the epilogue and not contained in this property. Thus, if Epilogue==null, a multipart ends with a close-delimiter whereas when Epilogue=="", a multipart ends with a close-delimiter followed by a CRLF. <see cref="http://tools.ietf.org/html/rfc2046#section-5.1.1"/></remarks>
		public string Epilogue { get; set; }

        /// <summary>Gets the MessagePart that contains the given file.</summary>
        /// <param name="file">The file in the unpack folder that should be added.</param>
        /// <returns>
        ///   The MessagePart that is represented by the file.
        /// </returns>
        /// <remarks>If the file is outside this MessagePart's unpack folder, null is returned.
        /// If the MessagePart doesn't exist, it will be created.</remarks>
        internal MessagePart GetPart(XFile file)
		{
			if (file == this.File)
				return this;
			else if (this.IsMultiPart && this.File.CanContain(file))
			{
				foreach (var ip in this.InnerParts)
				{
					if (ip.File.CanContain(file))
						return ip.GetPart(file);
				}
				// None found, just create it.
				var mp = new MessagePart();
				this.InnerParts.Add(mp);
				mp.Filename = file.FullName;
				mp.HeaderFields.ContentDispositionFileName = file.Name;
				return mp;
			}
			return null;
		}
	}
}
