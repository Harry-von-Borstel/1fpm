// Copyright © 2012 by blueshell Software Engineering Harry von Borstel (http://www.blueshell.com)
// This work is licensed under COPL (see http://www.codeproject.com/info/cpol10.aspx)
// 
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows;

namespace TypesaveViewModel.WpfBinding
{
	public static class Extensions
	{
		public static void Bind<TView, TModel>(this Connector<TModel> connector, Binding<TView, TModel> newBinding, DependencyObject targetObject, DependencyProperty targetProperty, UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default, BindingMode bindingMode = BindingMode.Default)
		{
			connector.AddBinding(newBinding);
			newBinding.Bind(connector, targetObject, targetProperty, updateSourceTrigger, bindingMode);
		}

		public static void Bind<TView, TModel>(this DependencyObject control, Connector<TModel> connector, DependencyProperty property, Binding<TView, TModel> binding, UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default)
		{
			connector.Bind(binding, control, property, updateSourceTrigger);
		}

		public static void Bind<TViewAndModel>(this DependencyObject control, Connector<TViewAndModel> connector, DependencyProperty property, UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default, Binding<TViewAndModel, TViewAndModel> binding = null, BindingMode bindingMode = BindingMode.Default)
		{
			connector.Bind(binding ?? new PlainBinding<TViewAndModel>(), control, property, updateSourceTrigger, bindingMode);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static void BindIsChecked(this ToggleButton toggleButton, Connector<bool?> connector, UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default)
		{
			connector.Bind(new PlainBinding<bool?>(), toggleButton, ToggleButton.IsCheckedProperty, updateSourceTrigger);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static void BindIsChecked<TModel>(this ToggleButton toggleButton, TModel modelValue, Connector<TModel> connector, UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default)
		{
			connector.Bind(new ToggleButtonBinding<TModel>(modelValue), toggleButton, ToggleButton.IsCheckedProperty, updateSourceTrigger);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static void BindText(this TextBox textBox, Connector<string> connector, UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default)
		{
			textBox.Bind(connector, TextBox.TextProperty, updateSourceTrigger);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static void BindText<TModel>(this TextBox textBox, Connector<TModel> connector, UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default)
		{
			textBox.Bind(connector, TextBox.TextProperty, new ConvertibleBinding<string, TModel>(), updateSourceTrigger);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static void BindText(this TextBlock textBlock, Connector<string> connector, UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default)
		{
			textBlock.Bind(connector, TextBlock.TextProperty, updateSourceTrigger);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static void BindSelectedDate(this DatePicker datePicker, Connector<DateTime?> connector, UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default)
		{
			connector.Bind(new PlainBinding<DateTime?>(), datePicker, DatePicker.SelectedDateProperty, updateSourceTrigger);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static void BindValue<TModel>(this RangeBase rangeControl, Connector<TModel> connector, UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default)
		{
			connector.Bind(new ConvertibleBinding<double, TModel>(), rangeControl, RangeBase.ValueProperty, updateSourceTrigger);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static void BindItemsSource<T>(this ItemsControl itemsControl, ListConnector<T> connector, UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default)
		{
			connector.Bind(new PlainBinding<IEnumerable<T>>(), itemsControl, ItemsControl.ItemsSourceProperty, updateSourceTrigger);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static void BindItemsSource<T>(this ItemsControl itemsControl, Connector<T> connector, UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default)
		{
			connector.Bind(new PlainBinding<T>(), itemsControl, ItemsControl.ItemsSourceProperty, updateSourceTrigger);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static void BindSelectedIndex(this Selector selectorControl, Connector<int> connector, UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default)
		{
			connector.Bind(new PlainBinding<int>(), selectorControl, Selector.SelectedIndexProperty, updateSourceTrigger);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		public static void BindIsEnabled(this UIElement control, Connector<bool> connector, UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default)
		{
			connector.Bind(new PlainBinding<bool>(), control, UIElement.IsEnabledProperty, updateSourceTrigger);
		}

	}
}
