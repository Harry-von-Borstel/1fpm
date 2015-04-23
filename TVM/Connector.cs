// Copyright © 2012 by blueshell Software Engineering Harry von Borstel (http://www.blueshell.com)
// This work is licensed under COPL (see http://www.codeproject.com/info/cpol10.aspx)
// 
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace TypesaveViewModel
{

	/// <summary>
	/// Base class for all connectors
	/// </summary>
	/// <remarks>
	/// The connector build the connection between target (view) and source (view model)
	/// </remarks>

	public class Connector<TModel> : IConnector
	{
		private readonly List<TypesafeBindingBase> bindings = new List<TypesafeBindingBase>();
		private readonly ReadOnlyCollection<TypesafeBindingBase> roBindings;
		public ReadOnlyCollection<TypesafeBindingBase> Bindings { get { return roBindings; } }

		private readonly Func<Connector<TModel>, TModel> getValue;
		private readonly Action<TModel, Connector<TModel>> setValue;
		public Action OnValueChanged { get; set; }
		private TModel v;

		public Connector()
		{
			this.roBindings = new ReadOnlyCollection<TypesafeBindingBase>(bindings);
			this.errorInfo = new ConnectorErrorInfo(this);
			this.getValue = connector => this.v;
			this.setValue = (val, connector) => this.v = val;

		}

		public Connector(Func<TModel> getValue, Action<TModel> setValue)
			: this(connector => getValue(), (val, connector) => setValue(val))
		{
		}

		public Connector(Func<Connector<TModel>, TModel> getValue, Action<TModel, Connector<TModel>> setValue)
		{
			roBindings = new ReadOnlyCollection<TypesafeBindingBase>(bindings);
			this.getValue = getValue;
			this.setValue = setValue;
		}

		public virtual bool CanGetValue { get { return getValue != null; } }

		public virtual bool CanSetValue { get { return setValue != null; } }

		public virtual TModel Value
		{
			get
			{
				return getValue(this);
			}
			set
			{
				setValue(value, this);
				if (OnValueChanged != null)
					OnValueChanged();
				UpdateView();
			}
		}
		public virtual void UpdateView()
		{
			foreach (var binding in this.bindings)
				binding.UpdateView();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public virtual void UpdateModel()
		{
			foreach (var binding in this.bindings)
			{
				try
				{
					binding.UpdateModel();
				}
				catch { }
			}
		}

		public void AddBinding(TypesafeBindingBase newBinding)
		{
			this.bindings.Add(newBinding);
		}

		private readonly ConnectorErrorInfo errorInfo;
		public ErrorInfo ErrorInfo
		{
			get { return this.errorInfo; }
		}

		private class ConnectorErrorInfo : ErrorInfo
		{
			private readonly Connector<TModel> connector;
			public ConnectorErrorInfo(Connector<TModel> connector)
			{
				this.connector = connector;
			}

			public override IEnumerable<ErrorInfo> InnerErrors
			{
				get
				{
					return connector.bindings
						.Select(binding => binding.ErrorInfo)
						.Where(errorInfo => errorInfo.HasError);
				}
			}

			public override bool HasError
			{
				get { return connector.bindings.Any(binding => binding.ErrorInfo.HasError); }
			}

			public override string ToString()
			{
				string errors = this.InnerErrors.Aggregate("", (accu, errorInfo) => string.Format("{0}{2}{1}", accu, errorInfo, accu.Length > 0 ? "\n" : ""));
				return !string.IsNullOrEmpty(errors) && connector.Name != null
					? string.Format("{0}:\n\t{1}", connector.Name, errors.Replace("\n", "\n\t"))
					: "";
			}

			public override void OnError(BindingErrorEventArgs eventArgs)
			{
				base.OnError(eventArgs);
				if (this.connector.ConnectorCollection != null)
					this.connector.ConnectorCollection.ErrorInfo.OnError(eventArgs);
			}
		}

		public string Name { get; set; }

		public ConnectorCollection ConnectorCollection { get; set; }

	}
}
