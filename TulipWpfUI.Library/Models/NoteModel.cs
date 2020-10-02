using System;
using System.Collections.Generic;
using System.Text;

namespace TulipWpfUI.Library.Models
{
    public class NoteModel
    {
        public int Id { get; set; }
        public int NotebookId { get; set; }
        public string Title { get; set; }
        public string FileLocation { get; set; }
        public DateTime CreatedDate { get; set; } 
        public DateTime UpdatedDate { get; set; } 
    }
}
