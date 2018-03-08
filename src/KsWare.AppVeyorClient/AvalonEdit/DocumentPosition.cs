using System;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;

namespace KsWare.AppVeyorClient.UI.Common {

	public class DocumentPosition {

		private readonly TextEditor _editor;

		public DocumentPosition(TextEditor editor, int offset) {
			if (offset > editor.Document.TextLength) throw new ArgumentOutOfRangeException(nameof(offset));
			_editor = editor;
			Offset = offset;
			Refresh();
		}

		public DocumentLine Line { get; private set; }

		public bool IsInIndentRegion { get; private set; }

		/// <summary>
		/// Gets the indent position (1-based).
		/// </summary>
		/// <value>The indent position.</value>
		public int IndentPosition { get; private set; }

		public int IndentIndex => IndentPosition - 1;

		/// <summary>
		/// Gets the character position in line (1-based).
		/// </summary>
		/// <value>The character position.</value>
		public int LineCharPosition { get; private set; }

		/// <summary>
		/// Gets the character index in line (0-based).
		/// </summary>
		/// <value>The character index.</value>
		public int LineCharIndex => LineCharPosition - 1;


		/// <inheritdoc cref="DocumentLine.Offset"/>
		public int LineOffset => Line.Offset;

		/// <summary>
		/// Gets the line number (1-based).
		/// </summary>
		/// <value>The line number.</value>
		public int LineNumber => Line.LineNumber;

		/// <summary>
		/// Gets the line index (0-based).
		/// </summary>
		/// <value>The line index.</value>
		public int LineIndex => LineNumber - 1;

		/// <summary>
		/// Gets the character offset (0-based).
		/// </summary>
		/// <value>The offset.</value>
		public int Offset { get; private set; }

		public void Refresh() {
			if (_editor.Text.Length == 0) {
				Line             = _editor.Document.GetLineByOffset(0);
				LineCharPosition = 1;
				IsInIndentRegion = true;
				IndentPosition   = 1;
				return;
			}
			Line         = _editor.Document.GetLineByOffset(Offset);
			LineCharPosition = Offset - LineOffset + 1; // position is 1-based
			var indentationLength = GetIndentationCharsLength(_editor, Line);
			IsInIndentRegion = LineCharIndex <= indentationLength;
			IndentPosition = IsInIndentRegion ? LineCharIndex / 4 + 1 : /*invalid*/0;
		}

		/// <summary>
		/// Gets the length of the indentation chars.
		/// </summary>
		/// <param name="editor">The editor.</param>
		/// <param name="line">The line.</param>
		/// <returns>System.Int32.</returns>
		private static int GetIndentationCharsLength(TextEditor editor, DocumentLine line) {
			var o = line.Offset;
			while (o < editor.Document.TextLength && editor.Document.GetCharAt(o) == ' ') o++;
			return o - line.Offset;
		}
	}

}