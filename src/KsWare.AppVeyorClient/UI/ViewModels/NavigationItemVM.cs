using System.Diagnostics;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using KsWare.Presentation;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.AppVeyorClient.UI.ViewModels {

	[DebuggerDisplay("{DisplayName}")]
	public class NavigationItemVM : ObjectVM {

		public NavigationItemVM() {
			RegisterChildren(()=>this);
		}

		public string DisplayName { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }

		public bool ExistsInDocument { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		public string RegexPattern { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }

		public Regex Regex { get => Fields.GetValue<Regex>(); set => Fields.SetValue(value); }

		public bool HasTemplate { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		public bool IsGroupTitle { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }
		
		public string Description { get; set; }

		[Hierarchy(HierarchyType.Reference)][CanBeNull]
		public NavigationItemVM ParentNav { get; set; }

		public string Key { get; set; }

	}

}
