using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
// new...
using AutoMapper;
using HelpTrackingv1.Models;
using System.Security.Claims;
using System.IO;
using CsvHelper;
using System.Collections;

namespace HelpTrackingv1.Controllers
{
    public class Manager
    {
        // Reference to the data context
        private ApplicationDbContext ds = new ApplicationDbContext();

        // AutoMapper components
        MapperConfiguration config;
        public IMapper mapper;

        // Request user property...

        // Backing field for the property
        private RequestUser _user;

        // Getter only, no setter
        public RequestUser User
        {
            get
            {
                // On first use, it will be null, so set its value
                if (_user == null)
                {
                    _user = new RequestUser(HttpContext.Current.User as ClaimsPrincipal);
                }
                return _user;
            }
        }

        // Default constructor...
        public Manager()
        {
            // If necessary, add constructor code here

            // Configure the AutoMapper components
            config = new MapperConfiguration(cfg =>
            {
                // Define the mappings below, for example...
                // cfg.CreateMap<SourceType, DestinationType>();
                // cfg.CreateMap<Employee, EmployeeBase>();

                // Object mapper definitions

                cfg.CreateMap<Models.RegisterViewModel, Models.RegisterViewModelForm>();
                cfg.CreateMap<Student, StudentBase>();
                cfg.CreateMap<StudentBase, Student>();
                cfg.CreateMap<Note, NoteBase>();
                cfg.CreateMap<Student, StudentWithNotes>();
                cfg.CreateMap<Note, NoteWithStudent>();
                cfg.CreateMap<NoteWithStudent, Note>();
            });

            mapper = config.CreateMapper();

            // Turn off the Entity Framework (EF) proxy creation features
            // We do NOT want the EF to track changes - we'll do that ourselves
            ds.Configuration.ProxyCreationEnabled = false;

            // Also, turn off lazy loading...
            // We want to retain control over fetching related objects
            ds.Configuration.LazyLoadingEnabled = false;
        }

        // ############################################################
        // RoleClaim

        public List<string> RoleClaimGetAllStrings()
        {
            return ds.RoleClaims.OrderBy(r => r.Name).Select(r => r.Name).ToList();
        }

        public bool LoadData()
        {
            // User name
            var user = HttpContext.Current.User.Identity.Name;

            // Monitor the progress
            bool done = false;

            // ############################################################
            // Role claims

            if (ds.RoleClaims.Count() == 0)
            {
                // Add role claims here
                ds.RoleClaims.Add(new RoleClaim { Name = "Dev" });
                ds.RoleClaims.Add(new RoleClaim { Name = "Coordinator" });
                ds.RoleClaims.Add(new RoleClaim { Name = "Faculty" });
                ds.RoleClaims.Add(new RoleClaim { Name = "AppAdmin" });
                ds.SaveChanges();
                done = true;
            }

            // ############################################################
            // Add Students

            if(ds.Students.Count() == 0)
            {
                var path = HttpContext.Current.Server.MapPath("~/App_Data/students.csv");

                StreamReader sr = File.OpenText(path);

                var csv = new CsvReader(sr);

                var studentsList = csv.GetRecords<StudentBase>().ToList();

                ds.Students.AddRange(mapper.Map<IEnumerable<Student>>(studentsList)).Distinct();

                ds.SaveChanges();

                sr.Close();

                sr = null;
            }

            return done;
        }

        public bool RemoveData()
        {
            try
            {
                foreach (var e in ds.RoleClaims)
                {
                    ds.Entry(e).State = System.Data.Entity.EntityState.Deleted;
                }
                ds.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RemoveDatabase()
        {
            try
            {
                return ds.Database.Delete();
            }
            catch (Exception)
            {
                return false;
            }
        }


        public IEnumerable<StudentBase> GetAllStudents()
        {
            var fetchedObjects = ds.Students.OrderBy(s => s.StudentId).ThenBy(s => s.FirstName);

            // TODO: Change OrderBy properties as needed 
            return (fetchedObjects == null) ? null : mapper.Map<IEnumerable<StudentBase>>(fetchedObjects).OrderBy(s=>s.Id).ThenBy(s=>s.FirstName);
        }

        public IEnumerable<StudentBase> searchStudents(string studentInfo="")
        {
            var value = studentInfo.ToLower().Trim();
            var fetchedObjects = ds.Students.Where(s => s.StudentId.ToLower().Contains(value) || s.FirstName.ToLower().Contains(value)|| s.LastName.Contains(value) || s.EmailAddress.Contains(value)).Distinct();
            return (fetchedObjects == null) ? null : mapper.Map<IEnumerable<StudentBase>>(fetchedObjects).OrderBy(s => s.Id).ThenBy(s => s.FirstName);
        }


        public StudentBase GetStudentById(int id)
        {
            var fetchedObject = ds.Students.Find(id);

            return (fetchedObject == null) ? null : mapper.Map<StudentBase>(fetchedObject);
        }

        public StudentWithNotes GetStudentWithNotes(int id)
        {
            //var fetchedObjects = from o in ds.Notes
            //                     where o.Author == user &
            //                     (o.Status.StartsWith("2") | o.Status.StartsWith("3") | o.Status.StartsWith("4"))
            //                     orderby o.Status, o.DateUpdated descending
            //                     select o;

            //var studs = ds.Students.ToList();
            //foreach(Student s in studs)
            //{
            //    if(s.Id == id)
            //    {
            //        var item = new StudentWithNotes();
            //        item.FirstName = s.FirstName;
            //        item.LastName = s.LastName;
            //        item.StudentId = s.StudentId;
            //        item.Id = s.Id;
            //        ds.Entry(s).Collection(x => x.Notes).Load();
            //        foreach (Note n in s.Notes)
            //        {
            //            item.Notes.Add(n);
            //        }
            //    }
             //  }
            
            var fetchedObject = ds.Students.Include("Notes").SingleOrDefault(s => s.Id == id);

            return (fetchedObject == null) ? null : mapper.Map<StudentWithNotes>(fetchedObject);
        }

        public NoteWithStudent GetNoteWithStudent(int noteId)
        {
            var fetchedObject = ds.Notes.Include("Student").SingleOrDefault(n=>n.Id == noteId);
            return (fetchedObject == null) ? null : mapper.Map<NoteWithStudent>(fetchedObject);
        }


        public NoteWithStudent AddNote(NoteWithStudentAdd newNote)
        {      
            var student = ds.Students.Find(newNote.IdStudent);
            Note n = new Note();
            n.Content = newNote.Content;
            n.Author = User.Name;
            n.DateCreated = DateTime.Now;
            n.Student = student;
            var addedNote = ds.Notes.Add(n);           
            ds.SaveChanges();
            return (addedNote == null) ? null : mapper.Map<NoteWithStudent>(addedNote);
        }

        public NoteWithStudent AddedNote(NoteAdded newNote)
        {
            var student = ds.Students.Find(newNote.IdStudent);
            Note n = new Note();
            n.Content = newNote.Content;
            n.Author = User.Name;
            n.DateCreated = DateTime.Now;
            n.Student = student;
            var addedNote = ds.Notes.Add(n);
            ds.SaveChanges();
            return (addedNote == null) ? null : mapper.Map<NoteWithStudent>(addedNote);
        }

    }





    // New "RequestUser" class for the authenticated user
    // Includes many convenient members to make it easier to render user account info
    // Study the properties and methods, and think about how you could use it

    // How to use...

    // In the Manager class, declare a new property named User
    //public RequestUser User { get; private set; }

    // Then in the constructor of the Manager class, initialize its value
    //User = new RequestUser(HttpContext.Current.User as ClaimsPrincipal);

    public class RequestUser
    {
        // Constructor, pass in the security principal
        public RequestUser(ClaimsPrincipal user)
        {
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                Principal = user;

                // Extract the role claims
                RoleClaims = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);

                // User name
                Name = user.Identity.Name;

                // Extract the given name(s); if null or empty, then set an initial value
                string gn = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.GivenName).Value;
                if (string.IsNullOrEmpty(gn)) { gn = "(empty given name)"; }
                GivenName = gn;

                // Extract the surname; if null or empty, then set an initial value
                string sn = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Surname).Value;
                if (string.IsNullOrEmpty(sn)) { sn = "(empty surname)"; }
                Surname = sn;

                IsAuthenticated = true;
                // You can change the string value in your app to match your app domain logic
                IsAdmin = user.HasClaim(ClaimTypes.Role, "Admin") ? true : false;
            }
            else
            {
                RoleClaims = new List<string>();
                Name = "anonymous";
                GivenName = "Unauthenticated";
                Surname = "Anonymous";
                IsAuthenticated = false;
                IsAdmin = false;
            }

            // Compose the nicely-formatted full names
            NamesFirstLast = $"{GivenName} {Surname}";
            NamesLastFirst = $"{Surname}, {GivenName}";
        }

        // Public properties
        public ClaimsPrincipal Principal { get; private set; }
        public IEnumerable<string> RoleClaims { get; private set; }

        public string Name { get; set; }

        public string GivenName { get; private set; }
        public string Surname { get; private set; }

        public string NamesFirstLast { get; private set; }
        public string NamesLastFirst { get; private set; }

        public bool IsAuthenticated { get; private set; }

        public bool IsAdmin { get; private set; }

        public bool HasRoleClaim(string value)
        {
            if (!IsAuthenticated) { return false; }
            return Principal.HasClaim(ClaimTypes.Role, value) ? true : false;
        }

        public bool HasClaim(string type, string value)
        {
            if (!IsAuthenticated) { return false; }
            return Principal.HasClaim(type, value) ? true : false;
        }
    }

}