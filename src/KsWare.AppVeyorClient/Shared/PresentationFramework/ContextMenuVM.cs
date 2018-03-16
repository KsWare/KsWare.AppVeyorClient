using System.Windows.Controls;
using JetBrains.Annotations;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.AppVeyorClient.Shared.PresentationFramework {
	
	public class MenuItemVM : DataVM<MenuItem> {

		public MenuItemVM() {
			RegisterChildren(() => this);
			//Data.Command
			//Data.Icon;
			//Data.Header;
			//Data.IsCheckable;
			//Data.IsChecked;
		}

		public string Caption { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to command
		/// </summary>
		/// <seealso cref="DoCommand"/>
		public ActionVM CommandAction { get; [UsedImplicitly] private set; }

		public bool IsChecked { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		public bool IsCheckable { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		/// <summary>
		/// Method for <see cref="CommandAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoCommand() {
			
		}

		// MenuItem.Items
		public ListVM<MenuItemVM> Items { get; [UsedImplicitly] private set; }

	}

}
