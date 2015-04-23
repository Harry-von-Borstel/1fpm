// Copyright © 2012 by blueshell Software Engineering Harry von Borstel (http://www.blueshell.com)
// This work is licensed under COPL (see http://www.codeproject.com/info/cpol10.aspx)
// 
// 
using System;

namespace TypesaveViewModel
{
	public class BindingErrorEventArgs : EventArgs
	{
		public string ErrorMessage { get; set; }
		public bool IsRemoved { get; set; }
		public TypesafeBindingBase Binding { get; set; }
		public object NativeInfo { get; set; }
	}
}
