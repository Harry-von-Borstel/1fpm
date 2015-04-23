// Copyright © 2012 by blueshell Software Engineering Harry von Borstel (http://www.blueshell.com)
// This work is licensed under COPL (see http://www.codeproject.com/info/cpol10.aspx)
// 
// 
namespace TypesaveViewModel
{
	/// <summary>
	/// Abstract base class for typesafe bindings
	/// </summary>
	public abstract class TypesafeBindingBase
	{
		/// <summary>
		/// Gets or sets the <see cref="Connector"/> that is used to bind to the model
		/// </summary>
		public TypesaveViewModel.IConnector Connector { get; protected set; }

		/// <summary>
		/// Gets or sets error information for the binding
		/// </summary>
		public ErrorInfo ErrorInfo { get; protected set; }

		/// <summary>
		/// Updates the view depending on the current model
		/// </summary>
		public abstract void UpdateView();

		/// <summary>
		/// Updates the model depending on the current view
		/// </summary>
		public abstract void UpdateModel();

		protected class BindingErrorInfo : ErrorInfo
		{
			protected TypesafeBindingBase Binding { get; set; }

			public BindingErrorInfo(TypesafeBindingBase binding)
			{
				this.Binding = binding;
			}

			public override void OnError(BindingErrorEventArgs eventArgs)
			{
				base.OnError(eventArgs);	// Trigger the event for all its direct subscribers
				this.Binding.Connector.ErrorInfo.OnError(eventArgs);	// Trigger the event for the ErrorChanged subscribers of the connector
			}
		}


	}
}