using System;
namespace blueshell.rfc822
{
	public interface IHeaderFieldBody
	{
		/// <summary>
		/// Returns all lines of the header field
		/// </summary>
		/// <param name="fieldName">Name of the header field</param>
		string ToString(string fieldName);
	}
}
