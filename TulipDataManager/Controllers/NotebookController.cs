using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TulipDataManager.Library.DataAccess;
using TulipDataManager.Library.Models;

namespace TulipDataManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotebookController : ControllerBase
    {
        private readonly INotebookData _notebookData;

        public NotebookController(INotebookData notebookData)
        {
            _notebookData = notebookData;
        }


        [HttpPost]
        public int Post(NotebookModel notebook)
        {
            return _notebookData.InsertNotebook(notebook);
        }

        [HttpGet]
        public List<NotebookModel> Get()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return _notebookData.GetAllNotebooks(userId);
        }


        [HttpPost]
        [Route("Note")]
        public void Post(NoteModel note)
        {
            _notebookData.InsertNote(note);
        }
        [HttpGet]
        [Route("Note")]
        public List<NoteModel> GetNotes()
        {
            return _notebookData.GetAllNotes();
        }
    }
}
