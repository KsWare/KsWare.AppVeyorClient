using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Threading;


namespace KsWare.AppVeyorClient.Shared {

	public static class TextElementExtensions {
		// Object DispatcherObject DependencyObject ContentElement FrameworkContentElement TextElement Inline Run
		// Object DispatcherObject DependencyObject ContentElement FrameworkContentElement TextElement Block Paragraph
		public static T Copy<T>(this T visual) where T:TextElement{
			var text = XamlWriter.Save(visual);
			var s = new MemoryStream(Encoding.Default.GetBytes(text));
			return (T)XamlReader.Load(s);
		}
	}

}
