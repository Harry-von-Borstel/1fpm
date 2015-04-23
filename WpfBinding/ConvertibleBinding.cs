// Copyright © 2012 by blueshell Software Engineering Harry von Borstel (http://www.blueshell.com)
// This work is licensed under COPL (see http://www.codeproject.com/info/cpol10.aspx)
// 
// 
using System;

namespace TypesaveViewModel.WpfBinding
{
	public class ConvertibleBinding<TView, TModel> : Binding<TView, TModel>
	{
		public ConvertibleBinding()
			: base(
				binding => (TView)Convert.ChangeType(binding.Connector.Value, typeof(TView)),
				getSetValue()
				)
		{
		}

		private static Action<TView, Binding<TView, TModel>> getSetValue()
		{
			if (typeof(TView) == typeof(string)
			    && typeof(TModel).IsGenericType
			    && typeof(TModel).GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				var underlyingType = typeof(TModel).GetGenericArguments()[0];
				return
					(v, binding) =>
					binding.Connector.Value = string.IsNullOrWhiteSpace(v as string)
					                          	? default(TModel)
					                          	: (TModel)Convert.ChangeType(v, underlyingType);
			}
			else
				return
					(v, binding) => binding.Connector.Value = (TModel)Convert.ChangeType(v, typeof(TModel));
		}
	}
}