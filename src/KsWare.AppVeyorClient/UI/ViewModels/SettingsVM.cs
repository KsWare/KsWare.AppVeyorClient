using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Threading;
using KsWare.AppVeyorClient.Shared.Json;
using KsWare.Presentation;
using KsWare.Presentation.BusinessFramework;
using KsWare.Presentation.ViewModelFramework;
using Newtonsoft.Json;

namespace KsWare.AppVeyorClient.UI.ViewModels {

	public class SettingsVM : ObjectVM {

		internal static readonly string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"KsWare","AppVeyorClient","settings.json");

		private static readonly string[] SerializablePropertyNames;

		private static JsonSerializerSettings SerializerSettings;

		static SettingsVM() {
			SerializablePropertyNames = typeof(SettingsVM).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
				.Where(IsSerializable).Select(p=>p.Name).ToArray();

			var jsonResolver = new OnlyDeclaredMembersSerializerContractResolver();
			jsonResolver.Include<SettingsVM>();

			SerializerSettings = new JsonSerializerSettings {
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
				ContractResolver      = jsonResolver
			};
		}

		private static bool IsSerializable(PropertyInfo p) { return p.GetCustomAttribute<JsonIgnoreAttribute>() == null; }


		private bool _anyValueChanged;
		private bool _isLoading;
		

		public SettingsVM() {
			RegisterChildren(()=>this);

			SaveToken = true;
			EnableYamlFolding = true;
			EnableYamlSyntaxHighlighting = true;

			PropertyChangedEvent.add = (s, e) => {
				if (SerializablePropertyNames.Contains(e.PropertyName)) {
					if(_isLoading) return;
					_anyValueChanged = true;
					Application.Current.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Action(Save));
				}
			};
		}

		[Browsable(false), JsonIgnore, IgnoreDataMember, Hierarchy(HierarchyType.Ignore)]
		public SettingsVM Data { get => Fields.GetValue<SettingsVM>(); set => Fields.SetValue(value); }	

		[Category("AppVeyor")]
		[DisplayName("Save Token")]
		[Description("If enabled, saves the token into a encrypted store on disk.")]
		public bool SaveToken { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		[Category("YAML")]
		[DisplayName("Enable YAML Folding")]
		public bool EnableYamlFolding { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		[Category("YAML")]
		[DisplayName("Enable YAML Syntax-Highlighting")]
		public bool EnableYamlSyntaxHighlighting { get => Fields.GetValue<bool>(); set => Fields.SetValue(value); }

		public void Save() {
			if(_anyValueChanged==false) return;

			var json = JsonConvert.SerializeObject(this, SerializerSettings);
			Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
			File.WriteAllText(FilePath,json);
			_anyValueChanged = false;
		}

		public void Load() {
			Data = null; 
			if(!File.Exists(FilePath)) return;
			var json = File.ReadAllText(FilePath);
			_isLoading = true;
			JsonConvert.PopulateObject(json,this);
			_isLoading = false;
			Data = this;
		}

		#region Hide ObjectVM properties in Property Editor and in JSON serializer

		[Browsable(false), JsonIgnore, IgnoreDataMember]
		public new bool Focusable { get => base.Focusable; set => base.Focusable = value; }

		[Browsable(false), JsonIgnore, IgnoreDataMember]
		public new bool IsKeyboardFocused { get => base.IsKeyboardFocused; set => base.IsKeyboardFocused = value; }

		[Browsable(false), JsonIgnore, IgnoreDataMember]
		public new bool IsSelected { get => base.IsSelected; set => base.IsSelected = value; }

		[Browsable(false), JsonIgnore, IgnoreDataMember]
		public new IObjectBM MːBusinessObject { get => base.MːBusinessObject; set => base.MːBusinessObject = value; }

		[Browsable(false), JsonIgnore, IgnoreDataMember]
		public new object MːBusinessObjectːData { get => base.MːBusinessObjectːData; set => base.MːBusinessObjectːData = value; }

		[Browsable(false), JsonIgnore, IgnoreDataMember]
		public new object MːData { get => base.MːData; set => base.MːData = value; }

		[Browsable(false), JsonIgnore, IgnoreDataMember]
		public new object MːDataːData { get => base.MːDataːData; set => base.MːDataːData = value; }

		[Browsable(false), JsonIgnore, IgnoreDataMember]
		public new string PropertyLabel { get => base.PropertyLabel; set => base.PropertyLabel = value; }

		[Browsable(false), JsonIgnore, IgnoreDataMember]
		public new ViewModelMetadata Metadata { get => base.Metadata; set => base.Metadata = value; }

		[Browsable(false), JsonIgnore, IgnoreDataMember]
		public new string MemberName { get => base.MemberName; set => base.MemberName = value; }

		[Hierarchy(HierarchyType.Parent)]
		[Browsable(false), JsonIgnore, IgnoreDataMember]
		public new IObjectVM Parent { get => base.Parent; set => base.Parent = value; }

		#endregion
	}
}
