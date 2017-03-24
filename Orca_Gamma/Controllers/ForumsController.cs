using PagedList;
using System;
using System.Linq;
using System.Web.Mvc;
using Orca_Gamma.Models;
using Orca_Gamma.Models.DatabaseModels;
using Orca_Gamma.Models.ViewModels;
using System.Net;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System.Web.Security;
using System.Collections.Generic;

namespace Orca_Gamma.Controllers
{
    public class ForumsController : Controller
    {
 
        private ApplicationDbContext _dbContext;

        public ForumsController()
        {
            _dbContext = new ApplicationDbContext();
        }

        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.nameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            var threads = from s in _dbContext.ForumThreads
                          select s;
            ViewBag.CurrentFilter = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                threads = threads.Where(t => t.Subject.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    threads = threads.OrderByDescending(s => s.Subject);
                    break;
                default:
                    threads = threads.OrderBy(s => s.Subject);
                    break;
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);

            return View(threads.ToPagedList(pageNumber, pageSize));
        }

        public ApplicationUser getCurrentUser()
        {
            return _dbContext.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId());
        }

        public int GetThreadId()
        {
            ForumThread thread = new ForumThread();
            return thread.Id;
        }
        public ForumThread GetCurrentThread()
        {
            return _dbContext.ForumThreads.Find(GetThreadId());
        }
        //GET: Forums/Create
        public ActionResult Create()
        {
            return View();
        }

        public ForumThread GetThread(ThreadMessagePost model)
        {
            ApplicationUser user = getCurrentUser();
            var post = new ForumThread
            {
                User = user, // This is how you do foreign keys - Cass
                Subject = model.Thread.Subject,
                FirstPost = model.Thread.FirstPost,
                Date = DateTime.Now

            };

            return post;
        }

        //POST: Forums/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ThreadMessagePost model)
        {
			ApplicationUser user = getCurrentUser();

            var post = new ForumThread
            {
				User = user, // This is how you do foreign keys - Cass
                Subject = model.Thread.Subject,
                FirstPost = model.Thread.FirstPost,
                Date = DateTime.Now

            };

            var thread = new ThreadMessagePost
            {
                User = user,
                Date = DateTime.Now,
                Body = model.Body,
                Thread = post
            };
                    
            //try
            //{
                //if (!ModelState.IsValid)
                //{
                    _dbContext.ForumThreads.Add(post);
                    _dbContext.ThreadMessagePosts.Add(thread);
                    _dbContext.SaveChanges();
                    return RedirectToAction("Index");
                //}
            //}
            //catch (RetryLimitExceededException /* dex */)
            //{
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            //}

            //return View(post);
        }

        //POST: Forums/Reply
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reply(int? id, ThreadMessagePost model)
        {
            ApplicationUser user = getCurrentUser();
            ThreadMessagePost post3 = _dbContext.ThreadMessagePosts.Find(id);
            ForumThread post2 = _dbContext.ForumThreads.Find(id);
            ThreadMessagePost post = new ThreadMessagePost();
            ForumThread post1 = new ForumThread();
            //var post = new ForumThread
            //{
            //    User = user, // This is how you do foreign keys - Cass
            //    Subject = model.Thread.Subject,
            //    FirstPost = model.Thread.FirstPost,
            //    Date = DateTime.Now

            //};
            //if (post.PartOf == post1.Id)
            //{
                var thread = new ThreadMessagePost
                {
                    User = user,
                    Thread = post2,
                    Body = model.Body,
                    Date = DateTime.Now
                    //Thread = post3.Thread
                };

                //try
                //{
                //if (!ModelState.IsValid)
                //{
                //_dbContext.ForumThreads.Add(post);
                _dbContext.ThreadMessagePosts.Add(thread);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            //}
            //}
            //}
            //catch (RetryLimitExceededException /* dex */)
            //{
            //Log the error (uncomment dex variable name and add a line here to write a log.
            //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            //}

            //return View(post);
        }

        // GET: /Forums/Details/
        public ActionResult Details(int? id)
        {

            //ApplicationUser user = getCurrentUser();
            //var threads = from s in _dbContext.ThreadMessagePosts
             //             select s;
            //var thread = new ForumsViewModel
            //{
            //    User = user,
            //    Date = DateTime.Now,
            //    FirstPost = model.FirstPost,
            //    Body = model.Body
            // };


            //int pageSize = 20;
            //int pageNumber = (page ?? 1);

            //return View(thread);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //ThreadMessagePost post = _dbContext.ThreadMessagePosts.Find(id);
            ForumThread post = _dbContext.ForumThreads.Find(id);
            //var threads = new List<ThreadMessagePost>();
            if (post == null)
            {
                return HttpNotFound();
            }
            //ViewBag.Threads = post;
            //ViewBag.Threads = threads;
            return View(post);
        }

        //GET: Forums/Reply
        public ActionResult Reply(int? id)
        {
            //ThreadMessagePost post = _dbContext.ThreadMessagePosts.Find(id);
            //if (id != null)
            //{
            //    ViewBag.Subject = post.Thread.Subject;
            //    ViewBag.FirstPost = post.Thread.FirstPost;
            //    ViewBag.User = post.User.UserName;
            //}
            return View();
        }

        ////POST: Forums/Reply
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Reply(ThreadMessagePost model)
        //{

        //    ApplicationUser user = getCurrentUser();
        //    ForumThread thread = GetCurrentThread();
        //    var post = new ThreadMessagePost
        //    {
        //        User = user,
        //        Thread = thread,
        //        Body = model.Body,
        //        Date = DateTime.Now
        //    };

        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            _dbContext.ThreadMessagePosts.Add(post);
        //            _dbContext.SaveChanges();
        //            return RedirectToAction("Index");
        //        }
        //    }
        //    catch (RetryLimitExceededException /* dex */)
        //    {
        //        //Log the error (uncomment dex variable name and add a line here to write a log.
        //        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
        //    }



        //    return View(post);
        //}



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}