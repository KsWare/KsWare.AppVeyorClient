using System.Windows;
using ICSharpCode.AvalonEdit;
using KsWare.Presentation.ViewFramework.AttachedBehavior;

namespace KsWare.AppVeyorClient.Shared.AvalonEditExtension {

	/// <summary>
	/// Connects a <see cref="TextEditor"/> to <see cref="TextEditorControllerVM"/>
	/// </summary>
	/// <seealso cref="KsWare.Presentation.ViewFramework.AttachedBehavior.BehaviorBase{ICSharpCode.AvalonEdit.TextEditor}" />
	public class TextEditorControllerBehavior : BehaviorBase<TextEditor> {

		public static readonly DependencyProperty ControllerProperty = DependencyProperty.RegisterAttached("Controller", typeof(TextEditorControllerVM), typeof(TextEditorControllerBehavior), new FrameworkPropertyMetadata(default(TextEditorControllerVM), new PropertyChangedCallback(PropertyChangedCallback)));

		public static void SetController(DependencyObject element, TextEditorControllerVM value) { element.SetValue(ControllerProperty, value); }

		public static TextEditorControllerVM GetController(DependencyObject element) { return (TextEditorControllerVM) element.GetValue(ControllerProperty); }

		private static void PropertyChangedCallback(DependencyObject d,DependencyPropertyChangedEventArgs e) {
			Attach<TextEditorControllerBehavior>(d);
		}


		protected TextEditorControllerBehavior(TextEditor dependencyObject) : base(dependencyObject) { }

		protected override void OnAttached() {
			base.OnAttached();
			var controller = GetController(AssociatedObject);
			if(controller ==null) return;
			controller.Data = AssociatedObject;
		}
	}

}