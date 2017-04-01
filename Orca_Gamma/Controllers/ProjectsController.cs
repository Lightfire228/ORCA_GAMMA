using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Orca_Gamma.Models;
using Orca_Gamma.Models.DatabaseModels;
using Microsoft.AspNet.Identity;

namespace Orca_Gamma.Controllers
{
    public class ProjectsController : Controller
    {
        private Models.ApplicationDbContext db = new Models.ApplicationDbContext();

        //this is legit
        // GET: Projects
        //public ActionResult Index(string searchString)
        //{

        //    var project = from p in db.Project
        //                   select p;

        //    if (!String.IsNullOrEmpty(searchString))
        //    {
        //        project = project.Where(s => s.Name.Contains(searchString));

        //    }

        //    return View(project.ToList());

        //}

        public ActionResult Index( string sortOrder, string searchString )
        {
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            var project = from p in db.Project
                           select p;

            if (!String.IsNullOrEmpty(searchString))
                   {
                       project = project.Where(s => s.Name.Contains(searchString));
                   }

                switch (sortOrder)
            {
                case "Date":
                    project = project.OrderBy(p => p.DateStarted);
                    break;
                case "date_desc":
                    project = project.OrderByDescending(p => p.DateStarted);
                    break;

            }
            return View(project.ToList());
        }



        /*
         * without search (create/edit only)
        // GET: Projects
        public ActionResult Index()
        {
            var project = db.Project.Include(p => p.User);
            return View(project.ToList());

        }

        */

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }


        // GET: Projects/Create
        [Authorize]
        public ActionResult Create()
        {
            //ViewBag.ProjectLead = new SelectList(db.ApplicationUsers, "Id", "FirstName");
            return View();
        }

        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Project model)
        {
            Project project = new Models.DatabaseModels.Project
            {
                User = db.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId()),
                Name = model.Name,
                Description = model.Description,

                //new:
                DateStarted = DateTime.Now,
                DateFinished = DateTime.Now


            };
            db.Project.Add(project);
            db.SaveChanges();

            if (ModelState.IsValid)
            {
                db.Project.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");

        }
   
        // GET: Projects/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Project project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }

            return View(project);
        }

        // POST: Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(ProjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                var project = db.Project.Find(model.ID);
                project.Name = model.Name;
                project.Description = model.Description;


                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }


        // GET: Projects/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Project.Find(id);
            db.Project.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
