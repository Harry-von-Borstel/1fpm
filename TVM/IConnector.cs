// Copyright © 2012 by blueshell Software Engineering Harry von Borstel (http://www.blueshell.com)
// This work is licensed under COPL (see http://www.codeproject.com/info/cpol10.aspx)
// 
// 
using System;
using System.Collections.ObjectModel;

namespace TypesaveViewModel
{
	public interface IConnector
	{
		ReadOnlyCollection<TypesafeBindingBase> Bindings { get; }
		Action OnValueChanged { get; set; }
		bool CanGetValue { get; }
		bool CanSetValue { get; }
		ErrorInfo ErrorInfo { get; }
		string Name { get; set; }
		ConnectorCollection ConnectorCollection { get; set; }
		void UpdateView();
		void UpdateModel();
	}
}