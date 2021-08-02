using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using ICSharpCode.AvalonEdit;
// using KsWare.AppVeyorClient.UI.Common;
using KsWare.Presentation.ViewFramework.AttachedBehavior;

namespace KsWare.AppVeyorClient.Shared.AvalonEditExtension {

	/// <summary>
	/// Connects <see cref="TextEditor"/> to <see cref="TextEditorControllerVM"/>
	/// </summary>
	/// <seealso cref="TextEditor" />
	public class TextEditorBehavior : BehaviorBase<TextEditor> {

		public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached("Text", typeof(string), typeof(TextEditorBehavior), new FrameworkPropertyMetadata(null,TextBindingPropertyChanged));

		private static void TextBindingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var ab=Attach<TextEditorBehavior>(d);
			Debug.WriteLine($"TextEditorBehavior.Text from source");
			ab.AssociatedObject.Text = (string) e.NewValue;
		}

		/// <summary>
		/// Sets the text.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <param name="value">The value.</param>
		public static void SetText(DependencyObject element, string value) { element.SetValue(TextProperty, value); }

		/// <summary>
		/// Gets the text.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns>System.String.</returns>
		public static string GetText(DependencyObject element) { return (string) element.GetValue(TextProperty); }

		protected TextEditorBehavior(TextEditor dependencyObject) : base(dependencyObject) { }

		protected override void OnAttached() {
			base.OnAttached();

			#region Text

			//BUG Binding does not work as expected, on texteditor text changed

			var binding = BindingOperations.GetBinding(AssociatedObject, TextProperty);

			AssociatedObject.LostFocus += AtAssociatedObjectOnLostFocus;
			AssociatedObject.TextChanged += AtAssociatedObjectOnTextChanged;

			switch (binding?.UpdateSourceTrigger) {
//				case UpdateSourceTrigger.Default:
//				case UpdateSourceTrigger.LostFocus: {
//					switch (binding?.Mode) {
//						case BindingMode.Default:
//						case BindingMode.TwoWay:
//						case BindingMode.OneWayToSource: 
//							AssociatedObject.LostFocus += AtAssociatedObjectOnLostFocus;
//							break;
//					}
//					break;
//				}
//				case UpdateSourceTrigger.PropertyChanged: {
//					switch (binding?.Mode) {
//						case BindingMode.Default:
//						case BindingMode.OneWayToSource:
//						case BindingMode.TwoWay:
//							AssociatedObject.TextChanged += AtAssociatedObjectOnTextChanged;
//							break;
//					}
//					break;
//				}
			}

			#endregion
		}

		#region Text

		private void AtAssociatedObjectOnLostFocus(object sender, RoutedEventArgs e) {
			AssociatedObject.Dispatcher.BeginInvoke(new Action(() => {
				Debug.WriteLine($"TextEditorBehavior.Text to source");
				SetText(AssociatedObject, AssociatedObject.Text);
			}));
		}

		private void AtAssociatedObjectOnTextChanged(object s, EventArgs e) {
			AssociatedObject.Dispatcher.BeginInvoke(new Action(() => {
				Debug.WriteLine($"TextEditorBehavior.Text to source");
				SetText(AssociatedObject, AssociatedObject.Text);
			}));
		}

		#endregion
	}
}
