using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text.RegularExpressions;

namespace blueshell.rfc822
{
	public class HeaderFields : IEnumerable<KeyValuePair<string, IHeaderFieldBody>>
	{
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
			get {
				return GetDateTime("Date");
			}
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
				headerFields["from"] =value.ToHeaderField();
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

		public ContentType ContentType
		{
			get
			{
				IHeaderFieldBody ct;
				return headerFields.TryGetValue("content-type", out ct)
					? ((HeaderFieldBodyWrapper<ContentType>)ct).Inner
					: null;
			}
			private set
			{
				headerFields["Content-Type"] = new HeaderFieldBodyWrapper<ContentType>(value);
			}
		}

		public ContentDisposition ContentDisposition
		{
			get
			{
				IHeaderFieldBody cd;
				return headerFields.TryGetValue("ContentDisposition", out cd)
					? ((HeaderFieldBodyWrapper<ContentDisposition>)cd).Inner
					: null;
			}
			private set
			{
				headerFields["ContentDisposition"] = new HeaderFieldBodyWrapper<ContentDisposition>(value);
			}
		}

		private HeaderFieldBody SetDateTime(string name, DateTime value)
		{
			return (HeaderFieldBody)(headerFields[name] = new HeaderFieldBody(value.ToString("R")));
		}

		private DateTime GetDateTime(string name)
		{
			return DateTime.Parse(headerFields[name].ToString());
		}



		public void Add(string key, string value)
		{
			switch (key.ToLower())
			{
				case "content-type":
					this.ContentType = new ContentType(value.ToString());
					break;
				case "contentdisposition":
					this.ContentDisposition = new ContentDisposition(value.ToString());
					break;
				default:
					headerFields[key] = new HeaderFieldBody( value);
					break;
			}
		}

		public void Add(string line)
		{
			var r = new Regex(Re.FIELD);
			var m = r.Match(line);
			Add(m.Groups[Re.Name.FIELD_NAME].Value,m.Groups[Re.Name.FIELD_BODY].Value);
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

		public override string ToString()
		{
			return this.Aggregate("",(total, next) => total + next.Value.ToString(next.Key));
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
				return this.ContentDisposition==null
					? null
					: this.ContentDisposition.FileName;
			}
			set
			{
				if (this.ContentDisposition == null)
					this.ContentDisposition = new ContentDisposition();
				this.ContentDisposition.FileName=value;
			}
		}
	}
}
