// Copyright © 2012 by blueshell Software Engineering Harry von Borstel (http://www.blueshell.com)
// This work is licensed under COPL (see http://www.codeproject.com/info/cpol10.aspx)
// 
// 
namespace TypesaveViewModel.WpfBinding
{
	public class PlainBinding<TView> : Binding<TView, TView>
	{
		public PlainBinding()
			: base(binding => binding.Connector.Value, (v, binding) => binding.Connector.Value = v)
		{
		}
	}
}