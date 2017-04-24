using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Orca_Gamma.Models;
using Orca_Gamma.Models.DatabaseModels;
using Microsoft.AspNet.Identity;
using PagedList;
using Orca_Gamma.Models.ViewModels;


namespace Orca_Gamma.Controllers
{
    public class ProjectsController : Controller
    {
        private Models.ApplicationDbContext db = new Models.ApplicationDbContext();

        public ApplicationUser getCurrentUser()
        {
            return db.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId());
        }

        public DateTime GetESTime()
        {
            DateTime timeUTC = DateTime.UtcNow;
            TimeZoneInfo estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime estTime = TimeZoneInfo.ConvertTimeFromUtc(timeUTC, estZone);
            return estTime;
        }

        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {

            //Date sort & project name search with pagedList
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            
            var project = from p in db.Project
                          where p.IsDeleted == false
                          select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                project = from p in db.Project
                          where p.IsDeleted == false
                          && (p.Description.Contains(searchString) || p.Name.Contains(searchString))
                          select p;
                //project = project.Where(s => s.Name.Contains(searchString));
            }

            //this is where I take sortOrder value and display
            switch (sortOrder)
            {
                case "project_lead":
                    project = project.OrderBy(p => p.ProjectLead);
                    break;
                case "project_name":
                    project = project.OrderBy(p => p.Name);
                    break;
                case "description":
                    project = project.OrderBy(p => p.Description);
                    break;
                case "date_created":
                    project = project.OrderBy(p => p.DateStarted);
                    break;    
            }


            int pageSize = 15;
            int pageNumber = (page ?? 1);

            return View(project.OrderBy(p => p.DateStarted).ToPagedList(pageNumber, pageSize));
        }


        // GET: Projects/Details
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //get the project they clicked on
            Project project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
			
            //get a list of collaborators on this project
            List<Collaborator> collabList = db.Collaborators.Include(k => k.User).Include(g => g.Project).Where(i => i.ProjectId == id).ToList();
            //convert the list of collaborators to a list of their applicationuser objects
            List<ApplicationUser> collabUserList = new List<ApplicationUser>();
            ApplicationUser temp;
            foreach (Collaborator collab in collabList)
            {
                temp = db.Users.Find(collab.UserId);
                collabUserList.Add(temp);
            };

            var projectWithCollabList = new ProjectViewModel
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                DateStarted = project.DateStarted,
                DateFinished = project.DateFinished,
                CollaboratorList = collabUserList,
                User = project.User
            };

            if (project.IsDeleted)
            {
                return HttpNotFound();
            }
			
            String tempID = getCurrentUser().Id;
            ViewBag.currentID = tempID;
            return View(projectWithCollabList);
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

                DateStarted = GetESTime(),

                //this needs to be changed so they enter a finished date -Geoff
                //**IMORTANT**
                DateFinished = GetESTime(),

                IsDeleted = false,
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

            if (project.IsDeleted)
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
                var project = db.Project.Find(model.Id);
                project.Name = model.Name;
                project.Description = model.Description;

                db.SaveChanges();
                return RedirectToAction("Details", new { id = project.Id });
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

            if (project.IsDeleted)
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
            project.IsDeleted = true;
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


        //GET: Projects/Add Collab
        [Authorize]
        public ActionResult Collab(int? id)
        {
            var projects = db.Project.Find(id);
            var projectLead = projects.User;
            var user = db.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId());

            if (id != null && projectLead == user && !projects.IsDeleted)
            {
                ViewBag.p_id = id;
                var collabList = db.Collaborators.Include(k => k.User).Include(g => g.Project).Where(i => i.ProjectId == id);
                return View(collabList.ToList());
            }
            return RedirectToAction("Index");
        }

        //GET: Collab
        [Authorize]
        public ActionResult CollabCreate(int? projectId)
        {
            var tempProject = db.Project.Find(projectId);

            if (tempProject.IsDeleted)
            {
                return HttpNotFound();
            }

            ViewBag.p_id = tempProject.Id;
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

            if (model.UserId != null)
            {
                var tempUser = db.Users.FirstOrDefault(g => g.UserName == model.UserId);
                if (tempUser == null)
                {
                    ViewBag.ERR = "User \"" + model.UserId + "\" does not exist."; 
                    return View();
                }
                //Ensure a collab can't be added twice.
                //Get list of collabs linked to this project
                var collabList = db.Collaborators.Include(k => k.User).Include(g => g.Project).Where(i => i.ProjectId == model.ProjectId);
                var collabUserNameList = new List<String>();
                //iterate through and see if you get a match
                foreach(Collaborator collab in collabList)
                {
                    var temp = db.Users.Find(collab.UserId);
                    collabUserNameList.Add(temp.UserName);
                }
                if (tempUser.Id.Equals(tempProject.ProjectLead))
                {
                    return RedirectToAction("Details", new { id = model.ProjectId });
                }
                foreach(String username in collabUserNameList)
                {
                    if (username.Equals(tempUser.UserName))
                    {
                        return RedirectToAction("Details", new { id = model.ProjectId });
                    }
                }
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


        //GET: Projects/Remove Collab
        [Authorize]
        public ActionResult CollabDelete(String userId, int projectId)
        {
            var project = db.Project.Find(projectId);
            if (project.IsDeleted)
            {
                return HttpNotFound();
            }

            var tempCollab = db.Collaborators.FirstOrDefault(p => p.ProjectId == projectId && p.UserId.Equals(userId));
            if (tempCollab != null)
            {
                return View(tempCollab);
            }
            return RedirectToAction("Index");
        }

        //POST: Projects/Remove Collab
        [HttpPost, ActionName("CollabDelete")]
        [Authorize]
        public ActionResult CollabDeleteConfirmed(String userId, int projectId)
        {
            var tempCollab = db.Collaborators.FirstOrDefault(p => p.ProjectId == projectId && p.UserId.Equals(userId));
            db.Collaborators.Remove(tempCollab);
            db.SaveChanges();
            return RedirectToAction("Collab", new { id = projectId });
        }
        
        //GET: Transfer Project Lead
        [Authorize]
        public ActionResult LeadTransfer(int? projectId)
        {
            var projects = db.Project.Find(projectId);
            var projectLead = projects.User;
            var user = db.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId());

            if (projectId != null && projectLead == user && !projects.IsDeleted)
            {
                var trueid = projectId;
                Project project = db.Project.Find(projectId);
                IEnumerable<Collaborator> collabList = db.Collaborators.Where(t => t.ProjectId == projectId);
                var idList = new List<String>();
                foreach (Collaborator collab in collabList)
                {
                    idList.Add(collab.UserId);
                }
                var unameList = new List<String>();
                foreach (String id in idList)
                {
                    var toAdd = db.Users.Find(id);
                    unameList.Add(toAdd.UserName);
                }
                ViewBag.Project = projectId;
                return View(new LeadTransferViewModel { project = (int)trueid, collabList = unameList });
            }
            return RedirectToAction("Index");
        }


        //POST: Projects/Lead Transfer
        [HttpPost, ActionName("LeadTransfer")]
        [Authorize]
        public ActionResult LeadTransfer(LeadTransferViewModel model)
        {

            if (model.selectedCollab != null)
            {
                //Make old leader into a collaborator
                Project project = db.Project.Find(model.project);
                String oldLeadId = project.ProjectLead;
                var oldLeadUser = db.Users.SingleOrDefault(g => g.Id.Equals(oldLeadId));
                var tempCollab = new Collaborator
                {
                    User = oldLeadUser,
                    Project = project,
                    ProjectId = project.Id,
                    UserId = oldLeadUser.Id
                };
                db.Collaborators.Add(tempCollab);

                //Make selected username's id the leader of that project.
                String newLeadUserName = model.selectedCollab;
                var newLeadUser = db.Users.SingleOrDefault(k => k.UserName.Equals(newLeadUserName));
                project.User = newLeadUser;
                project.ProjectLead = newLeadUser.Id;

                //Delete selected leader's collaboration entry.
                var toDelete = db.Collaborators.FirstOrDefault(p => p.UserId.Equals(newLeadUser.Id) && p.ProjectId == project.Id);
                db.Collaborators.Remove(toDelete);

                db.SaveChanges();
                return RedirectToAction("Details", new { id = project.Id });
            }
            return RedirectToAction("Index");
        }
    }
}
