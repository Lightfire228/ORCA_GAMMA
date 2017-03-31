using PagedList;
using System;
using System.Linq;
using System.Web.Mvc;
using Orca_Gamma.Models;
using Orca_Gamma.Models.DatabaseModels;
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

        public ApplicationUser getCurrentUser()
        {
            return _dbContext.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId());
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

            //var countReplies = "SELECT COUNT(dbo.ThreadMessagePosts.Id) FROM dbo.ThreadMessagePosts, dbo.ForumThreads WHERE dbo.ThreadMessagePosts.PartOf=dbo.ForumThreads.Id";
            //var totalReplies = _dbContext.Database.SqlQuery<int>(countReplies).Single();
            //ViewBag.CountReplies = totalReplies;

            //var getLastPost = "SELECT TOP 1 ThreadMessagePosts.Date FROM dbo.ThreadMessagePosts,dbo.ForumThreads Where dbo.ThreadMessagePosts.PartOf=dbo.ForumThreads.Id ORDER BY ThreadMessagePosts.Date DESC";
            //var showLastPost = _dbContext.Database.SqlQuery<int>(getLastPost).Single();
            //ViewBag.LastPost = showLastPost;

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(threads.ToPagedList(pageNumber, pageSize));
        }

        [Authorize]
        //GET: Forums/Create
        public ActionResult Create()
        {
            return View();
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

            _dbContext.ForumThreads.Add(post);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        //GET: Forums/Reply
        public ActionResult Reply(int? id)
        {
            ThreadMessagePost post = _dbContext.ThreadMessagePosts.Find(id);
            ViewBag.Subject = post.Thread.Subject;

            return View();
        }

        //POST: Forums/Reply
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reply(int? id, ThreadMessagePost model)
        {
            ApplicationUser user = getCurrentUser();
            ThreadMessagePost post = _dbContext.ThreadMessagePosts.Find(id);

            var thread = new ThreadMessagePost
            {
                User = user,
                Thread = post.Thread,
                Body = model.Body,
                Date = DateTime.Now
             };

             _dbContext.ThreadMessagePosts.Add(thread);
             _dbContext.SaveChanges();
             return RedirectToAction("Index");

        }

        // GET: /Forums/Details/
        public ViewResult Details(int? id, int? page)
        {
            var thread = from s in _dbContext.ThreadMessagePosts.Where(s => s.Thread.Id == id)
                       select s;

            ForumThread post = _dbContext.ForumThreads.Find(id);
            ViewBag.Subject = post.Subject;
            ViewBag.FirstPost = post.FirstPost;
            ViewBag.Date = post.Date;
            ViewBag.UserName = post.User.UserName;
            ViewBag.DateJoined = post.User.DateJoined;
            ViewBag.Post = post.Id;

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(thread.ToList().ToPagedList(pageNumber, pageSize));
        }


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