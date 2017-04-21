using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
// new...
using System.ComponentModel.DataAnnotations;

namespace HelpTrackingv1.Models
{
    // Add your design model classes below

    // Follow these rules or conventions:

    // To ease other coding tasks, the name of the 
    //   integer identifier property should be "Id"
    // Collection properties (including navigation properties) 
    //   must be of type ICollection<T>
    // Valid data annotations are pretty much limited to [Required] and [StringLength(n)]
    // Required to-one navigation properties must include the [Required] attribute
    // Do NOT configure scalar properties (e.g. int, double) with the [Required] attribute
    // Initialize DateTime and collection properties in a default constructor


    public class Student
    {
        public Student()
        {
            Notes = new List<Note>();
        }
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string FirstName { get; set; }

        [Required, StringLength(100)]
        public string LastName { get; set; }

        [Required, StringLength(9)]
        public string StudentId { get; set; }

        [Required, StringLength(100)]
        public string EmailAddress { get; set; }

        public ICollection<Note> Notes { get; set; }
    }


    public class Note
    {
        public Note()
        {
            DateCreated = DateTime.Now;
        }
        public int Id { get; set; }

        public DateTime DateCreated { get; set; }

        public string Author { get; set; }

        public string Content { get; set; }

        [Required]
        public Student Student {get;set;}
    }

    public class RoleClaim
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }
    }

}
