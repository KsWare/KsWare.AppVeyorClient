using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;
using KsWare.AppVeyorClient.Api.Contracts;
using KsWare.Presentation.ViewModelFramework;

namespace KsWare.AppVeyorClient.UI {

	public class ProjectEnvironmentVariablesVM: ObjectVM {

		public ProjectEnvironmentVariablesVM() {
			RegisterChildren(()=>this);
			GetAction.MːDoAction = async () => await OnGet();
			LoadAction.MːDoAction = async () => await OnLoad();
			SaveAction.MːDoAction = async () => await OnSave();
			PostAction.MːDoAction = async () => await OnPost();
		}

		private Client Client => AppVM.Client;

		public  ActionVM GetAction { get; [UsedImplicitly] private set; }
		public  ActionVM LoadAction { get; [UsedImplicitly] private set; }
		public  ActionVM SaveAction { get; [UsedImplicitly] private set; }
		public  ActionVM PostAction { get; [UsedImplicitly] private set; }

		public string PlainText { get => Fields.GetValue<string>(); set => Fields.SetValue(value); }

		private async Task OnGet() {
			var envvars = await Client.Project.GetProjectEnvironmentVariables();
			var sb      = new StringBuilder();
			foreach (var var in envvars) {
				sb.AppendLine($"{var}");
			}
			PlainText = sb.ToString();
		}

		private async Task OnLoad() { throw new NotImplementedException();}

		private async Task OnSave() { }

		private async Task OnPost() {
			if (string.IsNullOrWhiteSpace(PlainText)) { }
			else {
				var lines     = PlainText.Split(new[] {"\r\n", "\n", "\r"}, StringSplitOptions.None);
				var variables = new List<NameValueSecurePair>();
				foreach (var line in lines) {
					if (string.IsNullOrWhiteSpace(line)) continue;
					var tokens      = line.Split(new[] {":"}, 2, StringSplitOptions.None);
					var name        = tokens[0].Trim();
					var value       = tokens[1].Trim();
					var isEncrypted = value.StartsWith("secure!");
					value = isEncrypted ? value.Substring(7).Trim() : value;
					variables.Add(new NameValueSecurePair(name, value, isEncrypted));
				}
				Client.Project.UpdateProjectEnvironmentVariables(variables);
			}
		}

	
	}
}
