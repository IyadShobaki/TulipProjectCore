using System;
using System.Collections.Generic;
using System.Text;
using TulipDataManager.Library.Internal.DataAccess;
using TulipDataManager.Library.Models;

namespace TulipDataManager.Library.DataAccess
{
    public class NotebookData : INotebookData
    {
        private readonly ISqlDataAccess _sql;

        public NotebookData(ISqlDataAccess sql)
        {
            _sql = sql;
        }

        public int InsertNotebook(NotebookModel notebook)
        {
            int newNotebookId = _sql.CreateNotebook("dbo.spNotebook_Insert", notebook, "TulipData");
            return newNotebookId;
        }

        public List<NotebookModel> GetAllNotebooks(string userId)
        {
            var output = _sql.LoadData<NotebookModel, dynamic>("dbo.spNotebooks_GetAll", new { UserId = userId }, "TulipData");

            return output;
        }

        public List<NoteModel> GetAllNotes()
        {
            var output = _sql.LoadData<NoteModel, dynamic>("dbo.spNotes_GetAll", new { }, "TulipData");

            return output;
        }
        public void InsertNote(NoteModel note)
        {
            _sql.SaveData("dbo.spNote_Insert", note, "TulipData");
        }
    }
}
