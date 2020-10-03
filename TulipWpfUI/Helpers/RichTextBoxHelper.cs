using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace TulipWpfUI.Helpers
{
    public class RichTextBoxHelper : DependencyObject
    {
        public static string GetDocumentXaml(DependencyObject obj)
        {
            return (string)obj.GetValue(DocumentXamlProperty);
        }

        public static void SetDocumentXaml(DependencyObject obj, string value)
        {
            obj.SetValue(DocumentXamlProperty, value);
        }

        public static readonly DependencyProperty DocumentXamlProperty =
            DependencyProperty.RegisterAttached(
                "DocumentXaml",
                typeof(string),
                typeof(RichTextBoxHelper),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true,
                    PropertyChangedCallback = (obj, e) =>
                    {
                        var richTextBox = (RichTextBox)obj;

                        // Parse the XAML to a document (or use XamlReader.Parse())
                        var xaml = GetDocumentXaml(richTextBox);
                        var doc = new FlowDocument();

                        var range = new TextRange(doc.ContentStart, doc.ContentEnd);
                       
                        range.Load(new MemoryStream(Encoding.UTF8.GetBytes(xaml)),
                              DataFormats.Rtf);
                        

                        //Set the document
                        richTextBox.Document = doc;

                        //When the document changes update the source
                        range.Changed += (obj2, e2) =>
                            {
                                if (richTextBox.Document == doc)
                                {
                                    MemoryStream buffer = new MemoryStream();
                                    range.Save(buffer, DataFormats.Rtf);
                                    SetDocumentXaml(richTextBox,
                                        Encoding.UTF8.GetString(buffer.ToArray()));
                                }
                            };
                    }
                });
    }

}




















//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Documents;

//namespace TulipWpfUI.Helpers
//{
//    public class RichTextBoxHelper : DependencyObject
//    {
//        public static string GetDocumentContent(DependencyObject obj)
//        {
//            return (string)obj.GetValue(DocumentContentProperty);
//        }

//        public static void SetDocumentContent(DependencyObject obj, string value)
//        {
//            obj.SetValue(DocumentContentProperty, value);
//        }

//        //Using a DependencyProperty as the backing store for DocumentContent.This enables animation, styling, binding, etc...
//        public static readonly DependencyProperty DocumentContentProperty =
//            DependencyProperty.RegisterAttached("DocumentContent", typeof(string), typeof(RichTextBoxHelper),
//                new FrameworkPropertyMetadata
//                {
//                    BindsTwoWayByDefault = true,
//                    PropertyChangedCallback = (obj, e) =>
//                    {
//                        var richTextBox = (RichTextBox)obj;

//                        //Parse the XAML to a document(or use XamlReader.Parse())
//                        var xaml = GetDocumentContent(richTextBox);
//                        var doc = new FlowDocument();
//                        var range = new TextRange(doc.ContentStart, doc.ContentEnd);

//                        using (var reader = new MemoryStream(Encoding.UTF8.GetBytes(xaml)))
//                        {
//                            reader.Position = 0;
//                            richTextBox.SelectAll();
//                            richTextBox.Selection.Load(reader, DataFormats.Rtf);
//                        }
//                    }
//                });
//    }
//}
