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
using System.Net.Mime;
using System.Text.RegularExpressions;

namespace blueshell.rfc822
{
	/// <summary>
	/// The fields of a message's or message part's header
	/// </summary>
	/// <remarks>
	/// RFC 822 defines a header as
	/// <code>field       =  field-name ":" [ field-body ] CRLF</code>
	/// A <see cref="HeaderFields"/> object does hold all the fields of a message or a message part.
	/// A field is implemented here either as a string header field or as an object header field.
	/// The body of a string header fields is stored as a string whereas the body of an object header field is stored by the instance of a dedicated class  that represents the header type (like <see cref="ContentDisposition"/>).
	/// </remarks>
	public class HeaderFields : IEnumerable<KeyValuePair<string, IHeaderFieldBody>>
	{
		/// <summary>
		/// Interface for the description of an object header field
		/// </summary>
		private interface IFieldDescriptor
		{
			/// <summary>
			/// The name of the header field, i.e. the RFC 822 "field-name"
			/// </summary>
			string Name { get; set; }

			/// <summary>
			/// A <see cref="Func"/> that returns a field object for a given field body
			/// </summary>
			Func<string, object> Create { get; set; }

			Func<object,string> Render { get; set; }

			/// <summary>
			/// Sets the header field to a given field object
			/// </summary>
			/// <param name="hf">The header fields.</param>
			/// <param name="o">The field object.</param>
			void Setter(HeaderFields hf, object o);
		}

		/// <summary>
		/// Generic descriptor for an object header field
		/// </summary>
		/// <typeparam name="T">The class that represents the header type.</typeparam>
		private struct FieldDescriptor<T> : IFieldDescriptor
		{

			/// <summary>
			/// The name of the header field, i.e. the RFC 822 "field-name"
			/// </summary>
			public string Name { get; set; }

			/// <summary>
			/// A <see cref="Func"/> that returns a field object for a given field body
			/// </summary>
			public Func<string, object> Create { get; set; }

			public Func<object,string> Render { get; set; }

			/// <summary>
			/// Sets the header field to a given field object
			/// </summary>
			/// <param name="hf">The header fields.</param>
			/// <param name="o">The field object.</param>
			public void Setter(HeaderFields hf, object o)
			{
				hf.headerFields[this.Name] = new HeaderFieldObjectWrapper<T>((T)o, this);
			}
		}

		#region Object Field Types

		/// <summary>
		/// Field descriptor for "Content-Type"
		/// </summary>
		private static readonly FieldDescriptor<ContentType> fdContentType = new FieldDescriptor<ContentType>()
		{
			Name = "Content-Type",
			Create = x => new ContentType(x)
		};

		/// <summary>
		/// Field descriptor for "ContentDisposition"
		/// </summary>
		private static readonly FieldDescriptor<ContentDisposition> fdContentDisposition = new FieldDescriptor<ContentDisposition>()
		{
			Name = "ContentDisposition",
			Create = x => new ContentDisposition(x)
		};

		/// <summary>
		/// Field descriptor for "Content-Transfer-Encoding"
		/// </summary>
		/// <remarks>See http://tools.ietf.org/html/rfc2045#section-6" </remarks>
		private static readonly FieldDescriptor<TransferEncoding> fdContentTransferEncoding = new FieldDescriptor<TransferEncoding>()
		{
			Name = "Content-Transfer-Encoding",
			Create = x =>
			{
				switch (x.ToLower())
				{
					case "7bit":
						return TransferEncoding.SevenBit;
#if EIGHTBIT
					case "8bit":
						return TransferEncoding.EightBit;
#endif
					case "base64":
						return TransferEncoding.Base64;
					case "quoted-printable":
						return TransferEncoding.QuotedPrintable;
					default:
						return TransferEncoding.Unknown;
				}
			},
			Render = (o) =>
				{
					switch ((TransferEncoding)o)
					{
						case TransferEncoding.Base64:
							return "base64";
						case TransferEncoding.EightBit:
							return "8bit";
						case TransferEncoding.QuotedPrintable:
							return "quoted-printable";
						case TransferEncoding.SevenBit:
							return "7bit";
						default:
							return o.ToString();
					}
				}
		};

		/// <summary>
		/// Dictionary holding the field descriptors
		/// </summary>
		private Dictionary<string, IFieldDescriptor> fieldDescriptors
			= new Dictionary<string, IFieldDescriptor>(StringComparer.OrdinalIgnoreCase)
			{
				{fdContentType.Name,fdContentType},
				{fdContentDisposition.Name,fdContentDisposition},
				{fdContentTransferEncoding.Name,fdContentTransferEncoding}
			}
			;
		
		#endregion

		private class HeaderFieldObjectWrapper<T> : IHeaderFieldBody
		{
			public IFieldDescriptor fieldDescriptor;
			public T FieldObject { get; private set; }

			public HeaderFieldObjectWrapper(T fieldObject, IFieldDescriptor fd)
			{
				this.FieldObject = fieldObject;
				this.fieldDescriptor = fd;
			}

			public override string ToString()
			{
				return this.fieldDescriptor.Render==null
					? this.FieldObject.ToString()
					: fieldDescriptor.Render(this.FieldObject);
			}

			public string ToString(string fieldName)
			{
				return (string.Format("{0}:\t{1}\r\n", fieldName, this.ToString())).Fold();
			}

		}


		/// <summary>
		/// Generic getter for a header field object
		/// </summary>
		/// <typeparam name="T">The class that represents the header type.</typeparam>
		/// <param name="fd">The field descriptor.</param>
		/// <param name="defaultObject">The object to return, when the field wasn't set.</param>
		/// <returns>The object representing the field body.</returns>
		private T HeaderFieldObjectGetter<T>(IFieldDescriptor fd, T defaultObject = default(T))
		{
			IHeaderFieldBody b;
			return headerFields.TryGetValue(fd.Name, out b)
				? (T)(((HeaderFieldObjectWrapper<T>)b).FieldObject)
				: defaultObject;
		}

		/// <summary>
		/// Dictionary holding all the fields
		/// </summary>
		private readonly Dictionary<string, IHeaderFieldBody> headerFields;

		public HeaderFields()
		{
			headerFields = new Dictionary<string, IHeaderFieldBody>(StringComparer.OrdinalIgnoreCase)
			{
				{"Date", null },
				{"Resent-Date",null },
#region source
	#region trace
				{"Return-Path", null},
				{"Received", null},
	#endregion
#region originator
#region authentic
				{"Sender", null},
				{"From", null},
#endregion
				{"Reply-To", null},
#endregion
 
	#endregion
#region destination
				{"To",null },
				{"Resent-To",null },
				{"Cc",null },
				{"Resent-Cc",null },
				{"Bcc",null },
				{"Resent-Bcc",null },
#endregion
			};
		}

		public DateTime Date
		{
			get { return GetDateTime("Date"); }
			set { SetDateTime("Date", value); }
		}

		public DateTime ResentDate
		{
			get
			{
				return GetDateTime("Resent-Date");
			}
			set { SetDateTime("Resent-Date", value); }
		}

		public RouteAddress ReturnPath
		{
			get
			{
				return new RouteAddress(headerFields["Return-path"].ToString());
			}
			set
			{
				headerFields["Return-path"] = new HeaderFieldBody(value.ToString());
			}
		}

		public CommaList<Mailbox> From
		{
			get
			{
				var hf = headerFields["from"];
				return
					hf == null
					 ? null
					 : new CommaList<Mailbox>(hf.ToString());
			}
			set
			{
				headerFields["from"] = value.ToHeaderField();
			}
		}

		public Mailbox Sender
		{
			get
			{
				return new Mailbox(headerFields["sender"].ToString());
			}
			set
			{
				headerFields["sender"] = value.ToHeaderField();
			}
		}

		public CommaList<Address> To
		{
			get
			{
				return new CommaList<Address>(headerFields["to"].ToString());
			}
			set
			{
				headerFields["to"] = value.ToHeaderField();
			}
		}

		public CommaList<Address> Cc
		{
			get
			{
				return new CommaList<Address>(headerFields["cc"].ToString());
			}
			set
			{
				headerFields["cc"] = value.ToHeaderField();
			}
		}

		public CommaList<Address> Bcc
		{
			get
			{
				return new CommaList<Address>(headerFields["bcc"].ToString());
			}
			set
			{
				headerFields["bcc"] = value.ToHeaderField();
			}
		}

		public string Subject
		{
			get
			{
				return headerFields["subject"].ToString();
			}
			set
			{
				headerFields["subject"] = new HeaderFieldBody(value);
			}
		}

		public ContentType ContentTypeEffective
		{
			get
			{
				return this.ContentType ?? new ContentType("text/plain; charset=US-ASCII");
			}
		}

		/// <summary>
		/// Field object for "Content-Type" field
		/// </summary>
		public ContentType ContentType
		{
			get { return HeaderFieldObjectGetter<ContentType>(fdContentType); }
			set { fdContentType.Setter(this, value); }
		}

		/// <summary>
		/// Field object for "ContentDisposition" field
		/// </summary>
		public ContentDisposition ContentDisposition
		{
			get { return HeaderFieldObjectGetter<ContentDisposition>(fdContentDisposition); }
			set { fdContentDisposition.Setter(this, value); }
		}

		/// <summary>
		/// Field object for "Content-Transfer-Encoding" field
		/// </summary>
		public TransferEncoding ContentTransferEncoding
		{
			get { return HeaderFieldObjectGetter<TransferEncoding>(fdContentTransferEncoding, TransferEncoding.SevenBit); }
			set { fdContentTransferEncoding.Setter(this, value); }
		}

		private HeaderFieldBody SetDateTime(string name, DateTime value)
		{
			return (HeaderFieldBody)(headerFields[name] = new HeaderFieldBody(value.ToString("R")));
		}

		private DateTime GetDateTime(string name)
		{
			return DateTime.Parse(headerFields[name].ToString());
		}


		/// <summary>
		/// Adds a header
		/// </summary>
		/// <param name="key">The "header-name" (case insensitive).</param>
		/// <param name="value">The "header-body".</param>
		public void Add(string key, string value)
		{
			IFieldDescriptor fd;
			if (fieldDescriptors.TryGetValue(key, out fd))
			{
				fd.Setter(this, fd.Create(value));
			}
			else
			{
				headerFields[key] = new HeaderFieldBody(value);
			}
		}

		/// <summary>
		/// Adds a header
		/// </summary>
		/// <param name="line">The logical RFC 822 line representing a header field.</param>
		public void Add(string line)
		{
			var r = new Regex(Re.FIELD);
			var m = r.Match(line);
			Add(m.Groups[Re.Name.FIELD_NAME].Value, m.Groups[Re.Name.FIELD_BODY].Value);
		}

		public bool ContainsKey(string key)
		{
			return headerFields.ContainsKey(key);
		}

		public IHeaderFieldBody this[string key]
		{
			get
			{
				return headerFields[key.ToLower()];
			}
		}

		public IEnumerator<KeyValuePair<string, IHeaderFieldBody>> GetEnumerator()
		{
			foreach (var hf in headerFields)
			{
				if (hf.Value != null)
				{
					yield return hf;
				}
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		/// <summary>
		/// Renders the header.
		/// </summary>
		/// <returns>The completely rendered header.</returns>
		public override string ToString()
		{
			return this.Aggregate("", (total, next) => total + next.Value.ToString(next.Key));
		}

		/// <summary>
		/// Sets or gets the FileName of the <see cref="ContentDisposition"/>.
		/// </summary>
		/// <remarks>
		/// If the <see cref="ContentDisposition"/> is not set, this returns null on getting the property. On setting the property in this case, a new instance of <see cref="ContentDisposition"/> is created and then the <see cref="ContentDisposition.FileName"/> is set to the setting value. Note, that in this case <see cref="ContentDisposition"/>.<see cref="ContentDisposition.Inline"/> is false.
		/// </remarks>
		public string ContentDispositionFileName
		{
			get
			{
				return this.ContentDisposition == null
					? null
					: this.ContentDisposition.FileName;
			}
			set
			{
				if (this.ContentDisposition == null)
					this.ContentDisposition = new ContentDisposition();
				this.ContentDisposition.FileName = value;
			}
		}
	}
}
