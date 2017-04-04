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
using System.Web.Security;

namespace Orca_Gamma.Controllers
{
    public class PrivateMessagePostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ApplicationUser getCurrentUser()
        {
            return db.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId());
        }

		// GET: PrivateMessagePosts
		[Authorize]
        public ActionResult Index(string sortOrder, string previousFilter, string searchString, string lastSort)
        {
            var theId = User.Identity.GetUserId();
            var privateMessageList = db.PrivateMessages.Include(p => p.User);
            var betweenList = db.PrivateMessagesBetween.Include(k => k.User).Include(g => g.PrivateMessage).Where(r => r.PrivateMessage.IsDeleted == false).Where(i => i.UserId == theId);

            ViewBag.CurrentSort = sortOrder;
            if (lastSort != sortOrder)
            {
                ViewBag.LastSort = sortOrder;
            }
            else
            {
                if(sortOrder == "subject_desc")
                {
                    sortOrder = "subject_asc";
                }
                else if(sortOrder == "creator_desc")
                {
                    sortOrder = "creator_asc";
                }
                else if(sortOrder == "date_desc")
                {
                    sortOrder = "date_asc";
                }
                else if (sortOrder == "subject_asc")
                {
                    sortOrder = "subject_desc";
                }
                else if (sortOrder == "creator_asc")
                {
                    sortOrder = "creator_desc";
                }
                else if (sortOrder == "date_asc")
                {
                    sortOrder = "date_desc";
                }
            }

            ViewBag.SortParm = String.IsNullOrEmpty(sortOrder);

            if (searchString == null)
            {
                searchString = previousFilter;
            }
            ViewBag.CurrentFilter = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                betweenList = betweenList.Where(t => t.PrivateMessage.Subject.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "subject_asc":
                    betweenList = betweenList.OrderBy(s => s.PrivateMessage.Subject);
                    break;
                case "creator_desc":
                    betweenList = betweenList.OrderByDescending(s => s.PrivateMessage.UserId);
                    break;
                case "date_desc":
                    betweenList = betweenList.OrderByDescending(s => s.PrivateMessage.Date);
                    break;
                case "subject_desc":
                    betweenList = betweenList.OrderByDescending(s => s.PrivateMessage.Subject);
                    break;
                case "creator_asc":
                    betweenList = betweenList.OrderBy(s => s.PrivateMessage.UserId);
                    break;
                case "date_asc":
                    betweenList = betweenList.OrderBy(s => s.PrivateMessage.Date);
                    break;
                default:
                    betweenList = betweenList.OrderByDescending(s => s.PrivateMessage.Date);
                    ViewBag.LastSort = "date_desc";
                    break;
            }

            //Get a list of all private messages ids from the between model
            List<PrivateMessage> PostIDList = new List<PrivateMessage>();
            foreach (var Post in betweenList)
            {
                if (Post.User.Id == theId)
                {
                    PostIDList.Add(Post.PrivateMessage);
                }
            }
            
            //var privateMessagePosts = db.PrivateMessagePosts.Include(p => p.PrivateMessage).Include(p => p.User);
            return View(PostIDList.ToList());
        }

		// GET: PrivateMessagePosts/Details/5
		[Authorize]
		public ActionResult Details(int? id) //id of privatemessage
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //PrivateMessagePost privateMessagePost = db.PrivateMessagePosts.Find(id);
            //IEnumerable<PrivateMessagePost> list = from s in db.PrivateMessagePosts where s.Id == id select s;
            var list = db.PrivateMessagePosts.Where(r => r.PrivateMessage.Id == id).Include(p => p.User).Include(r => r.PrivateMessage);

            if (list == null)
            {
                return HttpNotFound();
            }

            PrivateMessage privateMessage = db.PrivateMessages.Find(id);
            ViewBag.SubjectOfMessage = privateMessage.Subject;
            return View(list);
        }

		// GET: PrivateMessagePosts/Create
		[Authorize]
		public ActionResult Create()
        {
            return View();
        }

        // POST: PrivateMessagePosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PrivateMessagePost model)
        {
            ApplicationUser user = getCurrentUser();
            String currentUserId = User.Identity.GetUserId();
            String currentUserUsername = user.UserName;
            
            var initial = new PrivateMessage
            {
                Date = DateTime.Now,
                Subject = model.PrivateMessage.Subject,
                IsDeleted = false,
                User = user
            };

            var post = new PrivateMessagePost
            {
                Body = model.Body,
                User = user,
                PrivateMessage = initial,
                Date = DateTime.Now,
                IsDeleted = false,
                IsImportant = false
            };

            //Recipients-----------------------------------------------------------------------------------------------------------------------------
            //Getting list of recipients through model.createdby and splitting at commas.
            string inputString = model.CreatedBy;
            string[] UserNameList = inputString.Split(',');
            for (int i = 0; i < UserNameList.Length; i++)
            {
                UserNameList[i] = UserNameList[i].Trim();
            }

            //convert this to a list and add the current user if they're not in there.
            List<String> UserNameListConverted = new List<String>(UserNameList);
            if(!UserNameListConverted.Contains(currentUserUsername))
            {
                UserNameListConverted.Add(currentUserUsername);
            }

            //Make a list of their actual ApplicationUser models
            List<ApplicationUser> RecipientList = new List<ApplicationUser>();
            foreach (String UserName in UserNameListConverted)
            {
                ApplicationUser temp = db.Users.SingleOrDefault(g => g.UserName == UserName);
                if(temp != null)
                {
                    RecipientList.Add(temp);
                }
            }

            foreach (var recipient in RecipientList)
            {
                var temp = new PrivateMessageBetween
                {
                    User = recipient,
                    PrivateMessage = initial
                };

                db.PrivateMessagesBetween.Add(temp);
            }
            //Recipients-----------------------------------------------------------------------------------------------------------------------------
            
            db.PrivateMessages.Add(initial);
            db.PrivateMessagePosts.Add(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: PrivateMessagePosts/Delete/5
        [Authorize]
		public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrivateMessage privateMessage = db.PrivateMessages.Find(id);
            if (privateMessage == null)
            {
                return HttpNotFound();
            }
            return View(privateMessage);
        }

        // POST: PrivateMessagePosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
		[Authorize]
		public ActionResult DeleteConfirmed(int id)
        {
            var userId = User.Identity.GetUserId();

            List<PrivateMessageBetween> pm = (from pms in db.PrivateMessagesBetween.ToList()
                                       where pms.UserId == userId && pms.PrivateMessageId == id
                                       select pms).ToList();

            db.PrivateMessagesBetween.Remove(pm[0]);
            db.SaveChanges();
            
            return RedirectToAction("Index");
        }

        // GET: PrivateMessagePosts/Reply
        [Authorize]
        public ActionResult Reply(int? id)
        {
            if (id != null)
            {
                PrivateMessagePost temp = db.PrivateMessagePosts.Find(id);
                var name = temp.User.FirstName;
                ViewBag.PostTitle = temp.PrivateMessage.Subject;
                ViewBag.User = name;
                ViewBag.Message = temp.Body;
                ViewBag.Date = temp.Date;
            }
            return View();
        }

        // POST: PrivateMessagePosts/Reply
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reply(int? id, PrivateMessagePost model)
        {
            ApplicationUser user = getCurrentUser();
            PrivateMessagePost temp = db.PrivateMessagePosts.Find(id);
            var post = new PrivateMessagePost
            {
                Body = model.Body,
                User = user,
                PrivateMessage = temp.PrivateMessage,
                Date = DateTime.Now,
                IsDeleted = false,
                IsImportant = false
            };
            
            db.PrivateMessagePosts.Add(post);
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
