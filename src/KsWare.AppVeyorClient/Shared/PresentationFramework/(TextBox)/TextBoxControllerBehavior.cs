using System.Windows;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit;
using KsWare.AppVeyorClient.Shared.AvalonEditExtension;
using KsWare.AppVeyorClient.UI.Common;
using KsWare.Presentation.ViewFramework.AttachedBehavior;

namespace KsWare.AppVeyorClient.Shared.PresentationFramework {

	/// <summary>
	/// Connects a <see cref="TextBox"/> to <see cref="TextBoxControllerVM"/>
	/// </summary>
	/// <seealso cref="KsWare.Presentation.ViewFramework.AttachedBehavior.BehaviorBase{System.Windows.Controls.TextBox}" />
	public class TextBoxControllerBehavior : BehaviorBase<TextBox> {

		public static readonly DependencyProperty ControllerProperty = DependencyProperty.RegisterAttached("Controller", typeof(TextBoxControllerVM), typeof(TextBoxControllerBehavior), new FrameworkPropertyMetadata(default(TextBoxControllerVM), new PropertyChangedCallback(PropertyChangedCallback)));

		public static void SetController(DependencyObject element, TextBoxControllerVM value) { element.SetValue(ControllerProperty, value); }

		public static TextBoxControllerVM GetController(DependencyObject element) { return (TextBoxControllerVM) element.GetValue(ControllerProperty); }

		private static void PropertyChangedCallback(DependencyObject d,DependencyPropertyChangedEventArgs e) {
			Attach<TextBoxControllerBehavior>(d);
		}


		protected TextBoxControllerBehavior(TextBox dependencyObject) : base(dependencyObject) { }

		protected override void OnAttached() {
			base.OnAttached();
			var controller = GetController(AssociatedObject);
			if(controller ==null) return;
			controller.Data = AssociatedObject;
		}
	}

}