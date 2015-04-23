// Copyright © 2012 by blueshell Software Engineering Harry von Borstel (http://www.blueshell.com)
// This work is licensed under COPL (see http://www.codeproject.com/info/cpol10.aspx)
// 
// 
using System;
using System.Linq;

namespace TypesaveViewModel
{
	public class ErrorInfo
	{
		public event EventHandler<BindingErrorEventArgs> ErrorChanged;

		private readonly string errorText;

		public ErrorInfo(string errorText)
		{
			this.errorText = errorText;
		}

		public ErrorInfo()
		{
			
		}

		public virtual bool HasError
		{
			get
			{
				return errorText==null;
			}
		}

		public virtual System.Collections.Generic.IEnumerable<ErrorInfo> InnerErrors
		{
			get
			{
				return new ErrorInfo[0];
			}
		}

		public virtual void OnError(BindingErrorEventArgs eventArgs)
		{
			if (this.ErrorChanged != null)
				ErrorChanged(this, eventArgs );
		}

		public override string ToString()
		{
			return errorText;
		}
	}
}
