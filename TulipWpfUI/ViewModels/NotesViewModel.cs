using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using TulipWpfUI.EventModels;
using TulipWpfUI.Library.Api;
using TulipWpfUI.Library.Models;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace TulipWpfUI.ViewModels
{
    public class NotesViewModel : Screen
    {
        private readonly INotesEndPoint _notesEndPoint;
        private readonly IEventAggregator _events;
        private readonly ILoggedInUserModel _loggedInUserModel;
        public int numberOfNotes = 0;
        public NotesViewModel(INotesEndPoint notesEndPoint,
            IEventAggregator events, ILoggedInUserModel loggedInUserModel)
        {
            _notesEndPoint = notesEndPoint;
            _events = events;
            _loggedInUserModel = loggedInUserModel;

            var fontFamilies = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            FontFamilyComboBox = fontFamilies.ToList();

            List<double> fontSizes = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 28, 48, 72 };
            FontSizeComboBox = fontSizes;

            RichText = new RichTextBox();
        }


        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadNotebooks();
            await LoadNotes();
        }

        private async Task LoadNotebooks()
        {
            var notebooks = await _notesEndPoint.GetAll();
            Notebooks = new BindingList<NotebookModel>(notebooks);
            NotifyOfPropertyChange(() => Notebooks);
        }

        public BindingList<NoteModel> AllNotes { get; set; }
        public async Task LoadNotes()
        {
            var notes = await _notesEndPoint.GetAllNotes();
            AllNotes = new BindingList<NoteModel>(notes);

        }
        private List<double> _fontSizeComboBox;

        public List<double> FontSizeComboBox
        {
            get { return _fontSizeComboBox; }
            set { _fontSizeComboBox = value; }
        }


        private List<FontFamily> _fontFamilyComboBox;

        public List<FontFamily> FontFamilyComboBox
        {
            get { return _fontFamilyComboBox; }
            set { _fontFamilyComboBox = value; }
        }


        private BindingList<NotebookModel> _notebooks;

        public BindingList<NotebookModel> Notebooks
        {
            get { return _notebooks; }
            set
            {
                _notebooks = value;
                NotifyOfPropertyChange(() => Notebooks);
            }
        }

        private NotebookModel _selectedNotebook;

        public NotebookModel SelectedNotebook
        {
            get { return _selectedNotebook; }
            set
            {
                _selectedNotebook = value;
                Notes = new BindingList<NoteModel>(AllNotes.Where(x => x.NotebookId.Equals(SelectedNotebook.Id)).ToList());
                numberOfNotes = Notes.Count() + 1;
                NotifyOfPropertyChange(() => Notes);
                NotifyOfPropertyChange(() => SelectedNotebook);
                NotifyOfPropertyChange(() => CanAddNote);
            }
        }

        private NoteModel _selectedNote;

        public NoteModel SelectedNote
        {
            get { return _selectedNote; }
            set
            {
                _selectedNote = value;
                using (FileStream fileStream = new FileStream(SelectedNote.FileLocation, FileMode.Open))
                {
                    TextRange range = new TextRange(RichText.Document.ContentStart, RichText.Document.ContentEnd);
                    range.Load(fileStream, DataFormats.Rtf);
                    ContentTextBox = range.Text;
                }
                NotifyOfPropertyChange(() => SelectedNote);
                NotifyOfPropertyChange(() => CanSaveButton);
                NotifyOfPropertyChange(() => ContentTextBox);
            }
        }
        public RichTextBox RichText { get; set; } 

        private string _contentTextBox;

        public string ContentTextBox 
        {
            get { return _contentTextBox; }
            set
            {
                _contentTextBox = value;
                NotifyOfPropertyChange(() => ContentTextBox);
            }
        }

        private string _statusTextBlock;

        public string StatusTextBlock
        {
            get { return _statusTextBlock; }
            set
            {
                _statusTextBlock = value;
                NotifyOfPropertyChange(() => StatusTextBlock);
            }
        }

        public bool CanSaveButton
        {
            get
            {
                bool output = false;
                if (SelectedNote != null)
                {
                    output = true;
                }
                return output;
            }
        }
        public void SaveButton()
        {
            using (FileStream fileStream = new FileStream(SelectedNote.FileLocation, FileMode.Create))
            {
                TextRange range = new TextRange(RichText.Document.ContentStart, RichText.Document.ContentEnd);
                range.Text = ContentTextBox;
                range.Save(fileStream, DataFormats.Rtf);
            }
        }

        public bool CanAddNote
        {
            get
            {
                bool output = false;
                if (SelectedNotebook != null)
                {
                    output = true;
                }
                return output;
            }
        }
        public async void AddNote()
        {
            var currentDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            //{D:\C#Projects\TulipProjectCore\TulipWpfUI\bin\Debug\netcoreapp3.1\}
            string projectDirectory = currentDirectory.Parent.Parent.Parent.FullName;
            //D:\\C#Projects\\TulipProjectCore\\TulipWpfUI"
            // Create Images folder if not exists
            string textFolder = (Directory.CreateDirectory(@$"{projectDirectory}\TextFiles\").ToString());

            string fileLocation = $"{textFolder}{_loggedInUserModel.LastName}{numberOfNotes}.rtf";
            _ = new FileStream(fileLocation, FileMode.Create);

            NoteModel note = new NoteModel
            {
                NotebookId = SelectedNotebook.Id,
                Title = $"New Note {numberOfNotes}",
                FileLocation = fileLocation,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };
            await _notesEndPoint.PostNoteInfo(note);

            await _events.PublishOnUIThreadAsync(new NotesEvent());
        }

        private BindingList<NoteModel> _notes;

        public BindingList<NoteModel> Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                NotifyOfPropertyChange(() => Notes);
            }
        }


        public async void CreateNotebook()
        {
            NotebookModel notebook = new NotebookModel
            {
                UserId = _loggedInUserModel.Id,
                Name = "New Notebook"
            };
            await _notesEndPoint.PostNotebookInfo(notebook);
        }
    }
}
