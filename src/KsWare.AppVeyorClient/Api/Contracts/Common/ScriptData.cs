using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsWare.AppVeyorClient.Api.Contracts {

	public class ScriptData {

		public ScriptData() { }

		public ScriptData(string language, string script) {
			Language = language;
			Script = script;
		}

		public string Language { get; set; }

		public string Script { get; set; }

	}

}
