using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HelpTrackingv1.Controllers
{
    public class StudentAdd
    {
        [Display(Name = "First Name")]
        [Required, StringLength(100)]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required, StringLength(100)]
        public string LastName { get; set; }

        [Display(Name = "Seneca Id")]
        [Required, StringLength(9)]
        public string StudentId { get; set; }

        [Display(Name = "Seneca Email")]
        [Required, StringLength(100)]
        public string EmailAddress { get; set; }

    }

    public class StudentBase : StudentAdd
    {
        public int Id { get; set; }
    }

    public class StudentWithNotes : StudentBase
    {
        public StudentWithNotes()
        {
            Notes = new List<NoteBase>();
        }
        public IEnumerable<NoteBase> Notes { get; set; }
    }

}