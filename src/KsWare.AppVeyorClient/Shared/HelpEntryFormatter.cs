using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace KsWare.AppVeyorClient.Shared {

	public class HelpEntryFormatter {

		public static readonly HelpEntryFormatter Instance = new HelpEntryFormatter();

		public FlowDocument CreateFlowDocument(IEnumerable<HelpEntry> helpEntries) {
			var doc = new FlowDocument{FontSize = 14};
			foreach (var helpEntry in helpEntries) {
				Debug.WriteLine($"{helpEntry.Path}: {helpEntry.Description}");

				if (helpEntry != helpEntries.First()) doc.Blocks.Add(HorizontalLine());

				doc.Blocks.Add(Caption(helpEntry.Path));
				doc.Blocks.Add(Description(helpEntry.Description));
				if (!string.IsNullOrWhiteSpace(helpEntry.Example)) {
					doc.Blocks.Add(Example(helpEntry.Example));
				}

				foreach (var example in helpEntry.Examples??Array.Empty<string>()) {
					doc.Blocks.Add(Example(example));
				}
			}

			return doc;
		}

		private Inline ParseInline(string text) {
			//return new Run(text);
			var list = new List<Inline>();
			list.Add(new Run(text));

			// split code parts
			// for (int i = 0; i < list.Count; i++) {
			// 	switch (list[i]) {
			// 		case Run run:
			// 			var t = run.Text;
			// 			var match = Regex.Match(t, @"<c>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
			// 			if (match.Success) {
			// 				run.Text = t.Substring(0, match.Index);
			// 				t = t.Substring(match.Index + match.Length);
			// 				match = Regex.Match(t, @"</c>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
			// 				if (match.Success) {
			// 					list.Insert(++i, new CodeInline(t.Substring(0, match.Index)));
			// 					list.Insert(i + 1, new Run(t.Substring(match.Index + match.Length)));
			// 				}
			// 				else {
			// 					list.Insert(++i, new CodeInline(t));
			// 				}
			// 			}
			// 			break;
			// 	}
			// }

			// format all runs (except code)
			for (int i = 0; i < list.Count; i++) {
				switch (list[i]) {
					case Run run:
						var t = run.Text;
						var match = Regex.Match(t, @"</?[bic]>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
						if (match.Success) {
							run.Text = t.Substring(0, match.Index); // trim existing run

							var newFormat = run.Copy();
							switch (match.Value.ToLowerInvariant()) {
								case "<c>": newFormat = (Run)new CodeInline("").ToInline(); break;
								case "<b>": newFormat.FontWeight = FontWeights.Bold; break;
								case "<i>": newFormat.FontStyle = FontStyles.Italic; break;
								case "</b>": newFormat.FontWeight = FontWeights.Normal; break;
								case "</i>": newFormat.FontStyle = FontStyles.Normal; break;
							}
							t =t.Substring(match.Index + match.Length);
							if (match.Value == "<c>") {
								match = Regex.Match(t, @"</c>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
								if (match.Success) {
									newFormat.Text = t.Substring(0, match.Index);
									t = t.Substring(match.Index + match.Length);
									list.Insert(++i, newFormat);
									newFormat = run.Copy();
								}
								else {
									newFormat.Text = t;
									list.Insert(++i, newFormat);
									continue;
								}
							}
							newFormat.Text = t;
							list.Insert(i+1, newFormat);
						}
						break;
				}
			}

			var span = new Span();
			foreach (var inline in list) {
				switch (inline) {
					case CodeInline code: span.Inlines.Add(code.ToInline()); break;
					default: span.Inlines.Add(inline); break;
				}
			}

			return span;
		}

		private Block Caption(string text) {
			return new Paragraph(new Run(text) {
				FontFamily = new FontFamily("Consolas"), FontWeight = FontWeights.Bold
			}) {
				Margin = new Thickness(0, 0, 0, 3)
			};
		}

		private Block HorizontalLine() {
			Line ruler = new Line {
				X1 = 0, X2 = 10, Y1 = 0, Y2 = 0, Stroke = new SolidColorBrush(Colors.Gray), StrokeThickness = 2,
				Stretch = Stretch.Fill
			};
			return new BlockUIContainer(ruler){Margin = new Thickness(0,10,0,10)};
		}

		private Block Example(string text) {
			var section = new Section();
			section.Blocks.Add(new Paragraph(new Run("Example:")) {
				FontSize = 10,
				Margin = new Thickness(0, 3, 0, 0)
			});
			section.Blocks.Add(new Paragraph(ParseInline(text)) {
				Margin = new Thickness(5, 0, 5, 3)
			});
			return section;
		}

		private Block Description(string text) {
			return new Paragraph(ParseInline(text)) {
				Margin = new Thickness(0, 0, 0, 5)
			};
		}

		private class CodeInline: Inline {
			public string Text { get; }

			public CodeInline(string text) {
				Text = text;
			}

			public Inline ToInline() {
				return new Run(Text) {
					FontFamily = new FontFamily("Consolas"),
					FontSize = 14,
					Background = Brushes.White,
					Tag = "<c>",
				};
			}
		}
	}

}
