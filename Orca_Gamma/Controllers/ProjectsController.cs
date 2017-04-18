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

        public ApplicationUser getCurrentUser()
        {
            return db.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId());
        }

        public ActionResult Index(string sortOrder, string searchString)
        {

            //new
            var userId = User.Identity.GetUserId();
            var projectsList = db.Project.Include(p => p.User);
            var collabList = db.Collaborators.Include(k => k.User).Include(g => g.Project).Where(i => i.UserId == userId);
            var postList = db.PrivateMessagePosts;


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
            String tempID = getCurrentUser().Id;
            ViewBag.currentID = tempID;
            return View(project);
        }


        // GET: Projects/Create
        [Authorize]
        public ActionResult Create()
        {
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

        //GET: Projects/Add Collab
        [Authorize]
        public ActionResult Collab(int? id)
        {
            if(id != null)
            {
                ViewBag.p_id = id;
                var collabList = db.Collaborators.Include(k => k.User).Include(g => g.Project).Where(i => i.ProjectId == id);
                return View(collabList.ToList());
            }
            return RedirectToAction("Index");
        }

        //GET
        [Authorize]
        public ActionResult CollabCreate(int? projectId)
        {
            var tempProject = db.Project.Find(projectId);

            var tempCollab = new Collaborator
            {
                Project = tempProject
            };
            return View(tempCollab);
        }

        //POST: Projects/Add Collab
        [HttpPost]
        [Authorize]
        public ActionResult CollabCreate(Collaborator model)
        {
            var tempProject = db.Project.Find(model.ProjectId);

            if (model.UserId != "")
            {
                var tempUser = db.Users.SingleOrDefault(g => g.UserName == model.UserId);
                var tempCollab = new Collaborator
                {
                    User = tempUser,
                    Project = tempProject
                };

                db.Collaborators.Add(tempCollab);
                db.SaveChanges();

                return RedirectToAction("Collab", new { id = tempCollab.ProjectId });
            }
            return View();
        }

        //GET
        [Authorize]
        public ActionResult CollabDelete(String userId, int projectId)
        {
            var tempCollab = db.Collaborators.Where(i => i.ProjectId == projectId).Where(k => k.UserId.Equals(userId)).ToList();
            if(tempCollab[0] != null)
            {
                return View(tempCollab[0]);
            }
            return RedirectToAction("Index");
        }

        //POST
        [HttpPost, ActionName("CollabDelete")]
        [Authorize]
        public ActionResult CollabDeleteConfirmed(String userId, int projectId)
        {
            List<Collaborator> toDelete = db.Collaborators.Where(i => i.ProjectId == projectId).Where(k => k.UserId.Equals(userId)).ToList();
            db.Collaborators.Remove(toDelete[0]);
            db.SaveChanges();
            return RedirectToAction("Collab", new { id = projectId });
        }

        ////POST: Projects/Delete Collab
        //[HttpPost, ActionName("Remove Collab")]
        //[ValidateAntiForgeryToken]
        //[Authorize]
        //public Delete(userId, p_id)
        //{
        //    var userId = User.Identity.GetUserId();

        //    //Delete from db the entry that matches

        //    //I casted it as a List basically... I dunno, it's stupid but it works.
        //    List<Collaborator> collab = (from pms in db.Collaborators.ToList()
        //                                 where pms.UserId == userId && pms.PrivateMessageId == id
        //                                 select pms).ToList();

        //    db.PrivateMessagesBetween.Remove(pm[0]);
        //    db.SaveChanges();

        //}



        // GET: Projects/Edit
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

            var projects = db.Project.Find(id);
            var projectLead = projects.User;
            var user = db.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId());

            if (projectLead == user)
            {
                return View(project);

            }
            else
                return RedirectToAction("Index");

        }


        // POST: Projects/Edit
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


        // GET: Projects/Delete
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

            var projects = db.Project.Find(id);
            var projectLead = projects.User;
            var user = db.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId());

            if (projectLead == user)
            {
                return View(project);
            }

            else
                return RedirectToAction("Index");

        }

        // POST: Projects/Delete
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

