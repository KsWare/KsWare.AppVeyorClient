// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
using System.Windows;
// using System.Windows.Input;
// using System.Windows.Interactivity;

namespace KsWare.AppVeyorClient.UI.Common {

	public class KeyAction : TriggerAction<UIElement> {

		public static readonly DependencyProperty KeyProperty =
			DependencyProperty.Register("Key", typeof(Key), typeof(KeyAction));

		public Key Key { get => (Key) GetValue(KeyProperty); set => SetValue(KeyProperty, value); }

		public static readonly DependencyProperty ModifiersProperty = DependencyProperty.Register("Modifiers", typeof(
			ModifierKeys), typeof(KeyAction), new PropertyMetadata(ModifierKeys.None));

		public ModifierKeys Modifiers {
			get => (ModifierKeys) GetValue(ModifiersProperty);
			set => SetValue(ModifiersProperty, value);
		}

		public static readonly DependencyProperty TargetProperty =
			DependencyProperty.Register("Target", typeof(UIElement), typeof(KeyAction), new UIPropertyMetadata(null));

		public UIElement Target {
			get => (UIElement) GetValue(TargetProperty);
			set => SetValue(TargetProperty, value);
		}

		protected override void Invoke(object parameter) {
			if (Keyboard.IsKeyDown(Key) && Keyboard.Modifiers==Modifiers) {
				Target.Focus();
			}
		}
	}

}
