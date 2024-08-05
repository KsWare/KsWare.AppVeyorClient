namespace KsWare.AppVeyorClient.UI.PanelYamlCodeEditor;

public enum BlockFormat {
	None,
	Folded,		// >	removes single newlines within the string (but adds one at the end, and converts double newlines to singles):
	Literal,	// |	turns every newline within the string into a literal newline, and adds one at the end
	Original
}
