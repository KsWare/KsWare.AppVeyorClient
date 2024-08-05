using System.Diagnostics;

namespace KsWare.AppVeyorClient.UI.PanelYamlCodeEditor;

[DebuggerDisplay("{Key}: {Content}")]
public class SectionTemplateData {
	public string Key { get; set; }
	public string Content { get; set; }
	public string Parent { get; set; }

}
