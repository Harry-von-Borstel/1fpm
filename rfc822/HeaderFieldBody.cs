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
using System.Linq;
using System.Text;

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
            this.content = contentItems;
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
                        (string.Format("{0}:\t{1}\r\n", fieldName, next)).Fold());
        }

    }
}
