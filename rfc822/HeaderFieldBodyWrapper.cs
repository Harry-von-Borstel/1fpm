using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blueshell.rfc822
{
	public class HeaderFieldBodyWrapper<T>: IHeaderFieldBody
	{
		public T Inner { get; private set; }
		
		public HeaderFieldBodyWrapper(T inner)
		{
			this.Inner = inner;
		}

		public string ToString(string fieldName)
		{
			return (string.Format("{0}:\t{1}\r\n", fieldName, this.Inner)).Fold();
		}
	}
}
