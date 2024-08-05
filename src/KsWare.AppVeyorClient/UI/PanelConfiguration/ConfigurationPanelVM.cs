using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using JetBrains.Annotations;
using KsWare.AppVeyor.Api;
using KsWare.AppVeyorClient.Helpers;
using KsWare.AppVeyorClient.Shared;
using KsWare.AppVeyorClient.Shared.AvalonEditExtension;
using KsWare.AppVeyorClient.UI.App;
using KsWare.AppVeyorClient.UI.Common;
using KsWare.AppVeyorClient.UI.PanelProjectSelector;
using KsWare.AppVeyorClient.UI.PanelSearch;
using KsWare.AppVeyorClient.UI.PanelYamlCodeEditor;
using KsWare.AppVeyorClient.UI.ViewModels;
using KsWare.Presentation;
using KsWare.Presentation.ViewFramework.AttachedBehavior;
using KsWare.Presentation.ViewModelFramework;
using BindingMode = System.Windows.Data.BindingMode;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Path = System.IO.Path;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace KsWare.AppVeyorClient.UI.PanelConfiguration {

	public class ConfigurationPanelVM : YamlCodeEditorVM, IHaveTitle {

		public ConfigurationPanelVM() {

		}

		public override string Title => "Configuration";

	}

	

}
