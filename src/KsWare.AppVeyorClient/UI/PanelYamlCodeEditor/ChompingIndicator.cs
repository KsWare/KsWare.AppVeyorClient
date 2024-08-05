namespace KsWare.AppVeyorClient.UI.PanelYamlCodeEditor;

public enum ChompingIndicator {
	None,
	Clip,       //		"clip": keep the line feed, remove the trailing blank lines. (TrimEnd)
	Strip,      // -	"strip": remove the line feed, remove the trailing blank lines.	(Trim)
	Keep,       // +	"keep": keep the line feed, keep trailing blank lines.	
}
