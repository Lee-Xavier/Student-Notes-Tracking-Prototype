using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HelpTrackingv1.Controllers
{
    public class NoteAdd
    {
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }

        public string Author { get; set; }

        [Display(Name = "Content")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

    }

    public class NoteBase : NoteAdd
    {
        public int Id { get; set; }
    }

    public class NoteWithStudentAdd : NoteAdd
    {

        public NoteWithStudentAdd()
        {
        }


        [Display(Name = "Student Id")]
        public int IdStudent { get; set; }
    }

    public class NoteAdded : NoteAdd
    {

        [Display(Name = "Student Id")]
        public int IdStudent { get; set; }
    }

    public class NoteWithStudent : NoteBase
    {

        public NoteWithStudent()
        {
        }

        public NoteWithStudent(StudentBase student)
        {
            Student = student;
        }
        public StudentBase Student { get; set; }
    }


}