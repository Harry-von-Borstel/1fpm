// Copyright © 2012 by blueshell Software Engineering Harry von Borstel (http://www.blueshell.com)
// This work is licensed under COPL (see http://www.codeproject.com/info/cpol10.aspx)
// 
// 
namespace TypesaveViewModel.WpfBinding
{
	public class ToggleButtonBinding<TModel> : Binding<bool?, TModel>
	{
		public ToggleButtonBinding(TModel modelValue)
			: base(
				binding => binding.Connector.Value.Equals(modelValue),
				(isChecked, binding) => { if (isChecked.HasValue && isChecked.Value) binding.Connector.Value = modelValue; })
		{
		}
	}
}