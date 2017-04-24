using Microsoft.AspNet.Identity;
using Orca_Gamma.Models;
using Orca_Gamma.Models.DatabaseModels;
using Orca_Gamma.Models.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

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

        public DateTime GetESTime()
        {
            DateTime timeUTC = DateTime.UtcNow;
            TimeZoneInfo estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime estTime = TimeZoneInfo.ConvertTimeFromUtc(timeUTC, estZone);
            return estTime;
        }


        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            //ViewBag.nameSortParm = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewBag.ForumName = sortOrder == "Name" ? "name_desc" : "Name";
            ViewBag.ForumDate = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            IEnumerable<ForumThread> threads = _dbContext.ForumThreads.ToList();

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
                case "Name":
                    threads = threads.OrderBy(s => s.Subject);
                    break;
                case "date_desc":
                    threads = threads.OrderByDescending(s => s.Date);
                    break;
                case "Date":
                    threads = threads.OrderBy(s => s.Date);
                    break;
                //default:
                //    threads = threads.OrderByDescending(s => s.Date);
                //    break;
            }

            var posts = _dbContext.ThreadMessagePosts.ToList();
            var keys = _dbContext.Keywords.ToList();
            List<ThreadViewModel> models = new List<ThreadViewModel>();

            //ForumThread forumThread = _dbContext.ForumThreads.Find(id);

            var keywords = _dbContext.ThreadKeywords;

            foreach (var thread in threads)
            {
                var currPosts = posts.Where(p => p.Thread == thread);
                var lastPost = currPosts.OrderBy(p => p.Date).LastOrDefault();
                var count = currPosts.OrderBy(p => p.Id);

                var model = new ThreadViewModel
                {
                    Threads = thread,
                    Posts = lastPost,
                    Keys = keys
                };
                ViewBag.CountReplies = count.Count();
                models.Add(model);

            }


            //var countReplies = "SELECT DISTINCT COUNT(dbo.ThreadMessagePosts.Body) FROM dbo.ThreadMessagePosts, dbo.ForumThreads WHERE dbo.ThreadMessagePosts.PartOf=dbo.ForumThreads.Id";
            ////var countReply = from s in _dbContext.ThreadMessagePosts.Where(s => s.PartOf == s.Thread.Id)
            //                 //select s;
            //var totalReplies = _dbContext.Database.SqlQuery<int>(countReplies).Single();
            //ViewBag.CountReplies = totalReplies;

            //var getLastPost = "SELECT DISTINCT TOP 1 ThreadMessagePosts.Date FROM dbo.ThreadMessagePosts,dbo.ForumThreads Where dbo.ThreadMessagePosts.PartOf=dbo.ForumThreads.Id GROUP BY ThreadMessagePosts.Date ORDER BY ThreadMessagePosts.Date DESC";
            //var showLastPost = _dbContext.Database.SqlQuery<DateTime>(getLastPost).Single();
            //ViewBag.LastPost = showLastPost;

            int pageSize = 10;
            int pageNumber = (page ?? 1);

			// equivelant of 
			// Math.Ceiling(threads.Count(), pageSize);
			int maxPages = 1 + ((threads.Count() - 1) / pageSize);

			if (pageNumber > maxPages)
				return View("Error");

			return View(models.ToPagedList(pageNumber, pageSize));
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
        public ActionResult Create(CreatePostViewModel model)
        {
			ApplicationUser user = getCurrentUser();
            DateTime time = GetESTime();

			Boolean error = false;
			if (model.Subject == null) {
				error = true;

				ViewBag.subjectBlank = "Subject cannot be empty";
			}
			if (model.Body == null) {
				error = true;

				ViewBag.bodyBlank = "Body cannot be empty";
			}
			if (error)
				return View(model);

			var post = new ForumThread
            {
                User = user, // This is how you do foreign keys - Cass
                Subject = model.Subject,
                FirstPost = null,
                IsDeleted = false,
                Date = time
                
            };

            var thread = new ThreadMessagePost
            {
                User = user,
                Body = model.Body,
                Date = time,
                Thread = post
            };

            var keyword = new Keyword
            {
                Name = model.Name
            };

            var threadKeyword = new ThreadKeyword
            {
                Keyword = keyword,
                Thread = post
            };
            //while (!ModelState.IsValid)
            //{
                _dbContext.ThreadKeywords.Add(threadKeyword);
                _dbContext.Keywords.Add(keyword);
                _dbContext.ThreadMessagePosts.Add(thread);
                _dbContext.ForumThreads.Add(post);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            //}
            //return View();
        }

        [Authorize]
        //GET: Forums/Reply
        public ActionResult Reply(int? id)
        {
            ThreadMessagePost post = _dbContext.ThreadMessagePosts.Find(id);
            ViewBag.Subject = post.Thread.Subject;
            ViewBag.Body = post.Body;

            return View();
        }

        //POST: Forums/Reply
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reply(int? id, ThreadMessagePost model)
        {
            ApplicationUser user = getCurrentUser();
            ThreadMessagePost post = _dbContext.ThreadMessagePosts.Find(id);
            DateTime time = GetESTime();

            var thread = new ThreadMessagePost
            {
                User = user,
                Thread = post.Thread,
                Body = model.Body,
                Date = time
             };

             _dbContext.ThreadMessagePosts.Add(thread);
             _dbContext.SaveChanges();
             return RedirectToAction("Index");

        }

        // GET: /Forums/Details/
        public ViewResult Details(string sortOrder, string currentFilter, string searchString, int? id, int? page)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ApplicationUser user = getCurrentUser();

            ViewBag.CurrentSort = sortOrder;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            var thread = from s in _dbContext.ThreadMessagePosts.Where(s => s.Thread.Id == id)
                           select s;

            ViewBag.CurrentFilter = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                thread = thread.Where(t => t.Body.Contains(searchString));
            }

            ForumThread post = _dbContext.ForumThreads.Find(id);
            ViewBag.Subject = post.Subject;

            int pageSize = 10;
            int pageNumber = (page ?? 1);

			int maxPages = 1 + ((thread.Count() - 1) / pageSize);

			if (pageNumber > maxPages)
				return View("Error");

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