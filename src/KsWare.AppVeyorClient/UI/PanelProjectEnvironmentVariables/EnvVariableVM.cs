using JetBrains.Annotations;
using KsWare.AppVeyor.Api.Contracts;
using KsWare.Presentation.Core.Providers;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.AppVeyorClient.UI.PanelProjectEnvironmentVariables {

	public sealed class EnvVariableVM : DataVM<NameValueSecurePair> {

		/// <inheritdoc />
		public EnvVariableVM() {
			RegisterChildren(() => this);
			Fields[nameof(Name)].ValueChangedEvent.add = (s, e) => {if(Data!=null) Data.Name = (string)e.NewValue;};
			Fields[nameof(Value)].ValueChangedEvent.add = (s, e) => {if(Data!=null) Data.Value.Value = (string)e.NewValue;};
			Fields[nameof(IsEncrypted)].ValueChangedEvent.add = (s, e) => {if(Data!=null) Data.Value.IsEncrypted = (bool)e.NewValue;};
		}

		/// <inheritdoc />
		protected override void OnDataChanged(DataChangedEventArgs e) {
			base.OnDataChanged(e);
			var d = (NameValueSecurePair)e.NewData;
			if (d == null) {
				Name = null;
				IsEncrypted = false;
				Value = null;
				return;
			}
			Name = d.Name;
			IsEncrypted = d.Value.IsEncrypted;
			Value = d.Value.Value;
		}

		public string Name { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }
		public string Value { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }
		public bool IsEncrypted { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to Delete
		/// </summary>
		/// <seealso cref="DoDelete"/>
		public ActionVM DeleteAction { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Method for <see cref="DeleteAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoDelete() {
			((IListVM)Parent).Remove(this);
		}
	}

}
