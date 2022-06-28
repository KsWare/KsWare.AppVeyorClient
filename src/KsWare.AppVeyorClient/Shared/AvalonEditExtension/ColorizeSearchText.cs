// ***********************************************************************
// Assembly         : KsWare.AppVeyorClient
// Author           : SchreinerK
// Created          : 2018-03-15
// ***********************************************************************
// <copyright file="ColorizeSearchText.cs" company="KsWare">
//     Copyright © 2018-2022 by KsWare
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace KsWare.AppVeyorClient.Shared.AvalonEditExtension {

	/// <summary>
	/// Class ColorizeSearchText.
	/// </summary>
	/// <seealso cref="ICSharpCode.AvalonEdit.Rendering.DocumentColorizingTransformer" />
	/// <example>
	/// <code language="C#">
	///		textEditor.TextArea.TextView.LineTransformers.Add(new ColorizeSearchText());
	/// </code>
	/// </example>
	public class ColorizeSearchText : DocumentColorizingTransformer {

		private string _skipSearchText;

		/// <summary>
		/// Gets or sets the search text.
		/// </summary>
		/// <value>The search text.</value>
		public string SearchText { get; set; }

//		public bool WholeWords { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether case sensitive.
		/// </summary>
		/// <value><c>true</c> if case sensitive; otherwise, <c>false</c>.</value>
		public bool CaseSensitive { get; set; }

		public bool UseRegex { get; set; }

		public Brush Background { get; set; } = new SolidColorBrush { Color = Colors.Yellow };

		/// <summary>
		/// Override this method to colorize an individual document line.
		/// </summary>
		/// <param name="line">The line.</param>
		protected override void ColorizeLine(DocumentLine line) {
			if (string.IsNullOrEmpty(SearchText)) return;
			int lineStartOffset = line.Offset;
			string text = CurrentContext.Document.GetText(line);

			if (UseRegex) {
				try {
					if(SearchText == _skipSearchText) return;
					var matches = Regex.Matches(text, SearchText, CaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);
					foreach (Match match in matches) {
						Format(lineStartOffset + match.Index, match.Length);
					}
				}
				catch (RegexParseException ex) {
					_skipSearchText = SearchText;
				}
			}
			else {
				int start = 0;
				int index;
				while ((index = text.IndexOf(SearchText, start, Comparison)) >= 0) {
					Format(lineStartOffset + index, SearchText.Length);
					start = index + 1; // search for next occurrence
				}
			}

		}

		private void Format(int offset, int length) {
			ChangeLinePart(offset, offset + length,
				(VisualLineElement element) => {
					// This lambda gets called once for every VisualLineElement between the specified offsets.
					var tf = element.TextRunProperties.Typeface;
					// Replace the typeface with a modified version of the same typeface
					element.TextRunProperties.SetTypeface(new Typeface(tf.FontFamily, FontStyles.Normal, FontWeights.Bold, tf.Stretch));
					element.BackgroundBrush = Background;
				});
		}

		private StringComparison Comparison => CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
	}

}
