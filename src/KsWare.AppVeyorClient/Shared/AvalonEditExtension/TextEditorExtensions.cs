using System;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;

namespace KsWare.AppVeyorClient.Shared.AvalonEditExtension {

	public static class TextEditorExtensions {

		private static TextBox txt;

		// return textEditor.Document.GetOffset(line:, column:)

		/// <inheritdoc cref="TextBox.GetLineText"/>
		public static string GetLineText(this TextEditor textEditor, int lineIndex) {
//			txt.GetLineText(lineIndex: 0);
			var line = textEditor.Document.GetLineByNumber(lineIndex+1);
			return textEditor.Document.GetText(line.Offset, line.Length);
		}

		/// <inheritdoc cref="TextDocument.GetText(ISegment)"/>
		//[Obsolete("Use Document.GetText(line)")]
		public static string GetLineText(this TextEditor textEditor, DocumentLine line) => textEditor.Document.GetText(line); 

		/// <inheritdoc cref="TextBox.GetCharacterIndexFromLineIndex"/>
		[Obsolete("Document.GetLineByNumber(lineIndex).Offset")]
		public static int GetCharacterIndexFromLineIndex(this TextEditor textEditor, int lineIndex) {
//			txt.GetCharacterIndexFromLineIndex(lineIndex: 0);
			var line = textEditor.Document.GetLineByNumber(lineIndex+1);
			return line.Offset;
		}

		/// <inheritdoc cref="TextBox.GetLineIndexFromCharacterIndex"/>
		[Obsolete("Document.GetLineByOffset(characterIndex).Offset")]
		public static int GetLineIndexFromCharacterIndex(this TextEditor textEditor, int characterIndex) {
			// txt.GetLineIndexFromCharacterIndex(lineNumber: 0);
			var line = textEditor.Document.GetLineByOffset(characterIndex);
			return line.Offset;
		}

//		/// <inheritdoc cref="TextBox.GetFirstCharIndexFromLine"/>
//		public static int GetFirstCharIndexFromLine(this TextEditor textEditor, int lineNumber) {
//			txt.GetFirstCharIndexFromLine(lineNumber: 0);
//			var line = textEditor.Document.GetLineByNumber(lineNumber);
//			return line.Offset;
//		}
//
//		/// <inheritdoc cref="TextBox.GetLineFromCharIndex"/>
//		public static int GetLineFromCharIndex(this TextEditor textEditor, int index) {
//			txt.GetLineFromCharIndex(index: 0);
//			return textEditor.Document.GetLineByOffset(index).LineNumber; // -1 ?
//		}
//
//		/// <inheritdoc cref="TextBox.GetFirstCharIndexOfCurrentLine"/>
//		public static int GetFirstCharIndexOfCurrentLine(this TextEditor textEditor) {
//			txt.GetFirstCharIndexOfCurrentLine();
//			var lineIndex = textEditor.TextArea.Caret.Location.Line;
//			var line = textEditor.Document.GetLineByNumber(lineIndex);
//			return line.Offset;
//		}

		public static DocumentPosition GetSelectionStartPosition(this TextEditor editor) => new DocumentPosition(editor, editor.SelectionStart);

		public static DocumentPosition GetSelectionEndPosition(this TextEditor editor) => new DocumentPosition(editor, editor.SelectionStart+editor.SelectionLength);
		
		public static DocumentPosition GetCaretPosition(this TextEditor editor) => new DocumentPosition(editor, editor.CaretOffset);


	}
}
