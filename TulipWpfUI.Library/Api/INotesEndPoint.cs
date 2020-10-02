using System.Collections.Generic;
using System.Threading.Tasks;
using TulipWpfUI.Library.Models;

namespace TulipWpfUI.Library.Api
{
    public interface INotesEndPoint
    {
        Task<List<NotebookModel>> GetAll();
        Task<List<NoteModel>> GetAllNotes();
        Task<int> PostNotebookInfo(NotebookModel notebook);
        Task PostNoteInfo(NoteModel note);
    }
}