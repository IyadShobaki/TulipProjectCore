using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TulipWpfUI.Library.Api;
using TulipWpfUI.Library.Models;
using TulipWpfUI.ViewModels;

namespace TulipWpfUI.Views
{
    /// <summary>
    /// Interaction logic for NotesView.xaml
    /// </summary>
    public partial class NotesView : UserControl
    {
     
        public NotesView()
        {
            InitializeComponent();
           
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //int amountOfCharacters = (new TextRange(contentRichTextBox.Document.ContentStart, contentRichTextBox.Document.ContentEnd)).Text.Length;
            int amountOfCharacters = ContentTextBox.Text.Length;
            StatusTextBlock.Text = $"Document length: {amountOfCharacters} characters";
        }
    }
}
