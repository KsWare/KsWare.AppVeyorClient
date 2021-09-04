using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using KsWare.Presentation.ViewFramework;

namespace KsWare.AppVeyorClient.Shared.CommandSink {

	/// <summary>
	/// Provides a <see cref="CommandBinding"/> for redirecting a <see cref="RoutedUICommand"/> to a command in a view model.
	/// </summary>
	/// <example>
	/// <code language="XAML">
	/// &lt;UserControl DataContext="UserControlViewModel">
	///		&lt;TextBox>
	///			&lt;UIElement.CommandBindings>
	/// 			&lt;CommandBindingMapper Command="ApplicationCommands.Help" CommandBinding="{RootBinding Path=ContextHelpCommand}"/>
	///			&lt;/UIElement.CommandBindings>
	///		&lt;/TextBox>
	/// ...
	/// </code>
	/// <code language="C#">
	///	class UserControlViewModel {
	///		public ICommand ContextHelpCommand {get}
	/// }
	/// </code>
	/// </example>
	/// <seealso cref="UIElement.CommandBindings"/>
	public class CommandBindingMapper : CommandBinding {

		private Binding _commandBinding;
		private readonly BindingProxy _proxy = new BindingProxy();

		/// <inheritdoc />
		public CommandBindingMapper() {
			CanExecute += OnCanExecute;
			Executed += OnExecuted;
		}

		private void OnExecuted(object sender, ExecutedRoutedEventArgs e) {
			if (_proxy.Value is ICommand command) {
				command.Execute(e.Parameter);
			}
		}

		private void OnCanExecute(object sender, CanExecuteRoutedEventArgs e) {
			if (_proxy.Value is ICommand command) {
				command.CanExecute(e.Parameter);
			}
		}

		/// <summary>
		/// Binding to a <see cref="ICommand"/> in a view model.
		/// </summary>
		/// <example>
		/// <code>
		/// &lt;CommandBindingMapper Command="ApplicationCommands.Help" CommandBinding="{RootBinding Path=ContextHelpCommand}"/>
		/// </code>
		/// </example>
		/// <seealso cref="RootBindingExtension"/>
		public Binding CommandBinding {
			get => _commandBinding;
			set {
				_commandBinding = value;
				BindingOperations.SetBinding(_proxy, BindingProxy.ValueProperty, value);
			}
		}
	}

}
