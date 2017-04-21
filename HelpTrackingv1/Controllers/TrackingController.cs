using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HelpTrackingv1.Controllers
{
    public class TrackingController : Controller
    {
        private Manager m = new Manager();

        // GET: Tracking
        public ActionResult Index()
        {
            m.LoadData();
            var studentList = m.GetAllStudents();
            return View(studentList);
        }

        public ActionResult Search(string studentInfo)
        {
            var result = m.searchStudents(studentInfo);
            return View(result);
        }

        // GET: Tracking/Details/5
        public ActionResult Details(int id)
        {
            var studentObj = m.GetStudentWithNotes(id);
            return View(studentObj);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Details(string Content, int studentIdInt)
        {
            NoteAdded item = new NoteAdded();
            item.Author = User.Identity.Name;
            item.Content = Content;
            item.IdStudent = studentIdInt;
      
            var result = m.AddedNote(item);

            if (result != null)
            {
                return RedirectToAction("details", new { id = item.IdStudent });            
            }
            else
            {
                return RedirectToAction("details", new { id = item.IdStudent });
                //return RedirectToAction("Index");
            }
        }


        // GET: Tracking/Details/5
        public ActionResult ViewNote(int id)
        {
            var noteObj = m.GetNoteWithStudent(id);
            return View(noteObj);
        }

        // GET: Tracking/Create
        public ActionResult Create(int id)
        {
            ViewBag.StudentId = id;
            NoteWithStudentAdd note = new NoteWithStudentAdd();
            //note.IdStudent = id;
            return View(note);
        }

        // POST: Tracking/Create
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(NoteWithStudentAdd newNote, int studentIdInt)
        {
            //var student = m.GetStudentById(studentIdInt);
            newNote.IdStudent = studentIdInt;
            var result = m.AddNote(newNote);

            if(result != null)
            {
                return RedirectToAction("Index");
            }
            else
            {
               return RedirectToAction("details", new { id = newNote.IdStudent});
            }

        }

        // GET: Tracking/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Tracking/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Tracking/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Tracking/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
