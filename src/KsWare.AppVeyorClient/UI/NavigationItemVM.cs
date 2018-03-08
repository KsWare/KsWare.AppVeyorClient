using System.Text.RegularExpressions;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.AppVeyorClient.UI {

	public class NavigationItemVM : ObjectVM {

		public NavigationItemVM() {
			RegisterChildren(()=>this);
		}

		public string DisplayName { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }

		public bool ExistsInDocument { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		public string RegexPattern { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }

		public Regex Regex { get => Fields.GetValue<Regex>(); set => Fields.SetValue(value); }
	}

}