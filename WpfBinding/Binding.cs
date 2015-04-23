// Copyright © 2012 by blueshell Software Engineering Harry von Borstel (http://www.blueshell.com)
// This work is licensed under COPL (see http://www.codeproject.com/info/cpol10.aspx)
// 
// 
using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.Generic;

namespace TypesaveViewModel.WpfBinding
{
	/// <summary>
	/// Typesafe binding for WPF
	/// </summary>
	/// <typeparam name="TView">The view's type</typeparam>
	/// <typeparam name="TModel">The model's type</typeparam>
	public class Binding<TView, TModel> : TypesafeBindingBase, ICustomTypeDescriptor
	{
		private DependencyObject targetObject;
		private DependencyProperty targetProperty;

		/// <summary>
		/// Constructs a new instance of <see cref="Binding{TView,TModel}"/>.
		/// </summary>
		/// <param name="getValue">A function that retrieves a value having the target's type from the source; null, if the source doesn't provide a value.</param>
		/// <param name="setValue">A method that writes a value having the target's type to the source; null, if the target doesn't provide a value.</param>
		public Binding(Func<Binding<TView, TModel>, TView> getValue, Action<TView, Binding<TView, TModel>> setValue)
		{
			ErrorInfo = new WpfBindingErrorInfo(this);
			this.getValue = getValue;
			this.setValue = setValue;
		}

		/// <summary>
		/// Function that retrieves a value having the target's type from the source
		/// </summary>
		private readonly Func<Binding<TView, TModel>, TView> getValue;

		/// <summary>
		/// Method that writes a value having the target's type to the source
		/// </summary>
		private readonly Action<TView, Binding<TView, TModel>> setValue;

		/// <summary>
		/// Binds a property of an object in the view to a connector in the model
		/// </summary>
		/// <param name="connector">Connector of the model</param>
		/// <param name="targetObject">Object in the view</param>
		/// <param name="targetProperty">Property of the object in the view</param>
		/// <param name="updateSourceTrigger">Timing of binding</param>
        /// <param name="bindingMode">Directions of binding</param>
		public void Bind(Connector<TModel> connector, DependencyObject targetObject, DependencyProperty targetProperty, UpdateSourceTrigger updateSourceTrigger, BindingMode bindingMode = BindingMode.Default)
		{
			if (this.Connector != null) throw new InvalidOperationException("This binding has already been bound.");
			this.Connector = connector;
			this.targetObject = targetObject;
			this.targetProperty = targetProperty;
			Validation.AddErrorHandler(targetObject, ErrorHandler);
			var binding = new Binding("Value")
			{
				Source = this,
				ValidatesOnExceptions = true,
				NotifyOnValidationError = true,
				UpdateSourceTrigger = updateSourceTrigger,
				Mode = 
                    bindingMode == BindingMode.Default
                        ? (
                            this.CanGetValue
					            ? (this.CanSetValue
						            ? BindingMode.Default
						            : BindingMode.OneWay)
					            : BindingMode.OneWayToSource)
                    : bindingMode
			};
			BindingOperations.SetBinding(targetObject, targetProperty, binding);
		}

		private static void ErrorHandler(object sender, ValidationErrorEventArgs args)
		{
			var sourceObject = sender as DependencyObject;
			if (sourceObject != null)
			{
				var bindingEx = args.Error.BindingInError as BindingExpression;
				if (bindingEx != null)
				{
					var bindingBase = bindingEx.ParentBinding.Source as TypesafeBindingBase;
					if (bindingBase != null)
					{
						bindingBase.ErrorInfo.OnError(new BindingErrorEventArgs
														{
															ErrorMessage = args.Error.ErrorContent.ToString(),
															IsRemoved = args.Action == ValidationErrorEventAction.Removed,
															Binding = bindingBase,
															NativeInfo = args
														});
					}
				}
			}
		}

		/// <summary>
		/// Updates the target and all its dependencies
		/// </summary>
		public override void UpdateView()
		{
			if (this.CanGetValue)
			{
				var bindingExpression = BindingOperations.GetBindingExpression(targetObject, targetProperty);
				if (bindingExpression != null) bindingExpression.UpdateTarget();
			}
		}

		public virtual bool CanSetValue { get { return this.setValue != null && this.Connector.CanGetValue; } }
		public virtual bool CanGetValue { get { return this.getValue != null && this.Connector.CanSetValue; } }
		public new Connector<TModel> Connector
		{
			get { return base.Connector as Connector<TModel>; }
			protected set { base.Connector = value; }
		}

		public override void UpdateModel()
		{
			if (this.CanSetValue)
			{
				var bindingExpression = BindingOperations.GetBindingExpression(targetObject, targetProperty);
				if (bindingExpression != null) bindingExpression.UpdateSource();
			}
		}


		#region Implementation of ICustomTypeDescriptor
		public AttributeCollection GetAttributes()
		{
			return new AttributeCollection();
		}

		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this);
		}

		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this);
		}

		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this);
		}

		public EventDescriptor GetDefaultEvent()
		{
			return null;
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return new BindingPropertyDescriptor(this);
		}

		public object GetEditor(Type editorBaseType)
		{
			return null;
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return EventDescriptorCollection.Empty;
		}

		public EventDescriptorCollection GetEvents()
		{
			return EventDescriptorCollection.Empty;
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			return ((ICustomTypeDescriptor)this).GetProperties();
		}

		public PropertyDescriptorCollection GetProperties()
		{
			return new PropertyDescriptorCollection(new[] { ((ICustomTypeDescriptor)this).GetDefaultProperty() });
		}

		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion

		/// <summary>
		/// Describes the property "Value" of the <see cref="Binding{TView,TModel}"/> that is used by the native binding
		/// </summary>
		private class BindingPropertyDescriptor : PropertyDescriptor
		{
			private readonly Binding<TView, TModel> binding;
			public BindingPropertyDescriptor(Binding<TView, TModel> binding)
				: base("Value", new Attribute[0])
			{
				this.binding = binding;
			}
			public override bool CanResetValue(object component)
			{
				return true;
			}

			public override Type ComponentType
			{
				get { return this.binding.GetType(); }
			}

			/// <summary>
			/// Gets the current value of the property.
			/// </summary>
			/// <returns>
			/// The value of a property for a given component.
			/// </returns>
			/// <param name="component">The component with the property for which to retrieve the value. </param>
			public override object GetValue(object component)
			{
				return this.binding.getValue(this.binding);
			}

			public override bool IsReadOnly
			{
				get { return !this.binding.CanSetValue; }
			}

			public override Type PropertyType
			{
				get { return typeof(TView); }
			}

			public override void ResetValue(object component)
			{
				this.SetValue(component, default(TView));
			}

			public override void SetValue(object component, object value)
			{
				this.binding.setValue((TView)value, this.binding);
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
		}

		protected class WpfBindingErrorInfo : TypesaveViewModel.TypesafeBindingBase.BindingErrorInfo
		{
			public WpfBindingErrorInfo(Binding<TView, TModel> binding)
				: base(binding)
			{

			}

			private Binding<TView, TModel> Binding1 { get { return Binding as Binding<TView, TModel>; } }

			public override bool HasError
			{
				get
				{
					return Validation.GetHasError(Binding1.targetObject);
				}
			}

			public override IEnumerable<ErrorInfo> InnerErrors
			{
				get
				{
					return Validation.GetErrors(Binding1.targetObject).Select(err => new ErrorInfo(err.ErrorContent.ToString()));
				}
			}

			public override string ToString()
			{
				return this.InnerErrors.Aggregate("", (accu, errorInfo) => string.Format("{0}{2}{1}", errorInfo, accu, accu.Length > 0 ? "\n" : ""));
			}
		}
	}
}
