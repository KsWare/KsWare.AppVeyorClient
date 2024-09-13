using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace KsWare.AppVeyorClient.Shared.AvalonEditExtension;

public static class MergingHighlightingLoader {
	
	public static IHighlightingDefinition Load(XmlReader reader, XmlReader colorReader, IHighlightingDefinitionReferenceResolver resolver) {
		var mainDefinition = HighlightingLoader.LoadXshd(reader);
		var colorDefinition = HighlightingLoader.LoadXshd(colorReader);

		var highlighting = HighlightingLoader.Load(mainDefinition, resolver);

		var colors = LoadColorsFromXshd(colorDefinition);
		ApplyColors(highlighting, colors);

		return highlighting;
	}

	private static Dictionary<string, XshdColor> LoadColorsFromXshd(XshdSyntaxDefinition colorDefinition) {
		var colors = new Dictionary<string, XshdColor>();

		foreach (var color in colorDefinition.Elements.OfType<XshdColor>()) {
			colors[color.Name] = color;
		}

		return colors;
	}

	private static void ApplyColors(IHighlightingDefinition highlighting, Dictionary<string, XshdColor> colors) {
		foreach (var color in colors) {
			var colorDefinition = highlighting.GetNamedColor(color.Key);
			if (colorDefinition == null) continue;

			if (color.Value.Foreground != null) {
				var foregroundColor = color.Value.Foreground.GetColor(null);
				if (foregroundColor.HasValue) {
					colorDefinition.Foreground = new SimpleHighlightingBrush(foregroundColor.Value);
				}
			}

			if (color.Value.Background != null) {
				var backgroundColor = color.Value.Background.GetColor(null);
				if (backgroundColor.HasValue) {
					colorDefinition.Background = new SimpleHighlightingBrush(backgroundColor.Value);
				}
			}

			if (color.Value.FontWeight != null) {
				colorDefinition.FontWeight = color.Value.FontWeight.Value;
			}

			if (color.Value.FontStyle != null) {
				colorDefinition.FontStyle = color.Value.FontStyle.Value;
			}
		}
	}
}
