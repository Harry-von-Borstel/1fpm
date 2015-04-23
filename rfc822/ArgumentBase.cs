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
using System.Text.RegularExpressions;

namespace blueshell.rfc822
{
	public class ArgumentBase
	{
		protected string str;
		protected readonly string pattern;

		protected ArgumentBase()
		{

		}

		protected ArgumentBase(string argument, string pattern)
		{
			this.pattern = pattern;
			Set(argument);
		}

		public void Set(string argument)
		{
			if (!TrySet(argument))
				throw new FormatException(string.Format("The argument \"{0}\" doesn't match the pattern \"{1}\".", argument, pattern));
		}

		public virtual bool TrySet(string argument)
		{
			if (argument == null)
				return true;
			var r = new Regex(pattern);
			var m = r.Match(argument);
			if (m.Value.Length != argument.Length)
				return false;
			str = argument;
			return true;
		}

		public override string ToString()
		{
			return str;
		}

	}
}