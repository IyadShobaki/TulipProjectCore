using System.Collections.Generic;
using TulipDataManager.Library.Models;

namespace TulipDataManager.Library.DataAccess
{
    public interface INotebookData
    {
        List<NotebookModel> GetAllNotebooks(string Id);
        List<NoteModel> GetAllNotes();
        void InsertNote(NoteModel note);
        int InsertNotebook(NotebookModel notebook);
    }
}