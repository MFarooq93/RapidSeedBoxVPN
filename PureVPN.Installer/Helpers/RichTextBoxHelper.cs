using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace PureVPN.Installer.Helpers
{
    public class RichTextBoxHelper : DependencyObject
    {
        public static string GetDocumentPath(DependencyObject obj)
        {
            return (string)obj.GetValue(DocumentPathProperty);
        }

        public static void SetDocumentPath(DependencyObject obj, string value)
        {
            obj.SetValue(DocumentPathProperty, value);
        }

        // Using a DependencyProperty as the backing store for DocumentPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DocumentPathProperty =
            DependencyProperty.RegisterAttached("DocumentPath", typeof(string), typeof(RichTextBoxHelper), new FrameworkPropertyMetadata
            {
                BindsTwoWayByDefault = true,
                PropertyChangedCallback = (obj, e) =>
                {
                    var richTextBox = (RichTextBox)obj;
                    var path = GetDocumentPath(richTextBox);
                    FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                    FlowDocument flowDocument = new FlowDocument();
                    TextRange textRange = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
                    textRange.Load(fileStream, DataFormats.Rtf);
                    richTextBox.Document = flowDocument;
                }
            });
    }
}
