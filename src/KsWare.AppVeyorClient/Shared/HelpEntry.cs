using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace KsWare.AppVeyorClient.Shared {

	public class HelpEntry {

		public static Dictionary<string,HelpEntry> LoadResource() {
			var uri = new Uri("pack://application:,,,/KsWare.AppVeyorClient;component/Ressources/AppVeyorYamlDoc.yaml");
			var info = Application.GetResourceStream(uri);
			using var r = new StreamReader(info.Stream);
			var deserializer = new DeserializerBuilder()
				.WithNamingConvention(UnderscoredNamingConvention.Instance) // "naming convention" means the convention in yaml file!
				.Build();
			var dic = deserializer.Deserialize<Dictionary<string, HelpEntry>>(r);
			foreach (var entry in dic) {
				entry.Value.Path = entry.Key;
				entry.Value.Name = entry.Key.Split('/').Last();
			}

			return dic;
		}

		[YamlIgnore]
		public string Path { get; set; }
		[YamlIgnore]
		public string Name { get; set; }

		public string Description { get; set; }
		public string Type { get; set; }
		public string Example { get; set; }
		public string[] Examples { get; set; }
		public string[] Values { get; set; }
		public string Default { get; set; }
	}

}
