// ***********************************************************************
// Assembly         : KsWare.AppVeyorClient
// Author           : SchreinerK
// Created          : 2018-03-15
//
// Last Modified By : SchreinerK
// Last Modified On : 2018-03-15
// ***********************************************************************
// <copyright file="ColorizeSearchRegex.cs" company="KsWare">
//     Copyright © 2018 by KsWare
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace KsWare.AppVeyorClient.Shared.AvalonEditExtension {

	/// <summary>
	/// Class ColorizeSearchRegex.
	/// </summary>
	/// <seealso cref="ICSharpCode.AvalonEdit.Rendering.DocumentColorizingTransformer" />
	/// <example>
	/// <code language="C#">
	///		textEditor.TextArea.TextView.LineTransformers.Add(new ColorizeSearchRegex());
	/// </code>
	/// </example>
	public class ColorizeSearchRegex : DocumentColorizingTransformer {

		/// <summary>
		/// Gets or sets the regex.
		/// </summary>
		/// <value>The regex.</value>
		public Regex Regex { get; set; }

		/// <summary>
		/// Override this method to colorize an individual document line.
		/// </summary>
		/// <param name="line">The line.</param>
		protected override void ColorizeLine(DocumentLine line) {
			if(Regex==null) return;
			var text = CurrentContext.Document.GetText(line);
			var lineStartOffset = line.Offset;
			var matches = Regex.Matches(text);
			foreach (Match match in matches) {
				base.ChangeLinePart(lineStartOffset + match.Index, // startOffset
					lineStartOffset + match.Index + match.Length,    // endOffset
					(VisualLineElement element) => {
						// This lambda gets called once for every VisualLineElement between the specified offsets.
						var tf = element.TextRunProperties.Typeface;
						// Replace the typeface with a modified version of the same typeface
						element.TextRunProperties.SetTypeface(
							new Typeface(tf.FontFamily, FontStyles.Italic, FontWeights.Bold, tf.Stretch));
					});				
			}
		}
	}
}