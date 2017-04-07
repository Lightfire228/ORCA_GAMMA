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
using Orca_Gamma.Models.ViewModels;
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
            var postList = db.PrivateMessagePosts;

            ViewBag.CurrentSort = sortOrder;
            if (lastSort != sortOrder)
            {
                ViewBag.LastSort = sortOrder;
            }
            else
            {
                if (sortOrder == "subject_desc")
                {
                    sortOrder = "subject_asc";
                }
                else if (sortOrder == "creator_desc")
                {
                    sortOrder = "creator_asc";
                }
                else if (sortOrder == "date_desc")
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
                else if (sortOrder == "latest_desc")
                {
                    sortOrder = "latest_asc";
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

            //switch (sortOrder)
            //{
            //    case "subject_asc":
            //        betweenList = betweenList.OrderBy(s => s.PrivateMessage.Subject);
            //        break;
            //    case "creator_desc":
            //        betweenList = betweenList.OrderByDescending(s => s.PrivateMessage.UserId);
            //        break;
            //    case "date_desc":
            //        betweenList = betweenList.OrderByDescending(s => s.PrivateMessage.Date);
            //        break;
            //    case "subject_desc":
            //        betweenList = betweenList.OrderByDescending(s => s.PrivateMessage.Subject);
            //        break;
            //    case "creator_asc":
            //        betweenList = betweenList.OrderBy(s => s.PrivateMessage.UserId);
            //        break;
            //    case "date_asc":
            //        betweenList = betweenList.OrderBy(s => s.PrivateMessage.Date);
            //        break;
            //    case "latest_asc":
            //        betweenList = betweenList.OrderBy(s => s.PrivateMessage.Date);
            //        break;
            //    case "latest_desc":
            //        betweenList = betweenList.OrderByDescending(s => s.PrivateMessage.Date);
            //        break;
            //    default:
            //        betweenList = betweenList.OrderByDescending(s => s.PrivateMessage.Date);
            //        ViewBag.LastSort = "date_desc";
            //        break;
            //}

            //Get a list of all private messages ids from the between model
            List<PMIndexViewModel> PostIDList = new List<PMIndexViewModel>();
            IEnumerable<PrivateMessagePost> pmps = db.PrivateMessagePosts.Include(g => g.User).Include(h => h.PrivateMessage).ToList();
            IEnumerable<ApplicationUser> aus = db.Users.ToList();
            foreach (var k in betweenList)
            {
                if (k.User.Id == theId)
                {
                    String lastReply = "Default";
                    DateTime lastReplyTime = DateTime.Now;
                    PrivateMessagePost tryThis = pmps.Last(x => x.PartOf == k.PrivateMessageId);
                    lastReply = tryThis.User.UserName;
                    lastReplyTime = tryThis.Date;

                    var temp = new PMIndexViewModel
                    {
                        Id = k.PrivateMessage.Id,
                        UserId = k.PrivateMessage.User.FirstName,
                        Date = k.PrivateMessage.Date,
                        Subject = k.PrivateMessage.Subject,
                        IsDeleted = k.PrivateMessage.IsDeleted,
                        IsImportant = k.PrivateMessage.IsImportant,
                        User = k.PrivateMessage.User,
                        LastPost = lastReply,
                        LastReplyTime = lastReplyTime
                    };
                    PostIDList.Add(temp);
                }
            }
            //List<Order> SortedList = objListOrder.OrderBy(o=>o.OrderDate).ToList();
            List<PMIndexViewModel> SortedList;
            switch (sortOrder)
            {
                case "subject_asc":
                    SortedList = PostIDList.OrderBy(s => s.Subject).ToList();
                    break;
                case "subject_desc":
                    SortedList = PostIDList.OrderByDescending(s => s.Subject).ToList();
                    break;
                case "creator_asc":
                    SortedList = PostIDList.OrderBy(s => s.UserId).ToList();
                    break;
                case "creator_desc":
                    SortedList = PostIDList.OrderByDescending(s => s.UserId).ToList();
                    break;
                case "date_asc":
                    SortedList = PostIDList.OrderBy(s => s.Date).ToList();
                    break;
                case "date_desc":
                    SortedList = PostIDList.OrderByDescending(s => s.Date).ToList();
                    break;
                case "latest_asc":
                    SortedList = PostIDList.OrderBy(s => s.LastReplyTime).ToList();
                    break;
                case "latest_desc":
                    SortedList = PostIDList.OrderByDescending(s => s.LastReplyTime).ToList();
                    break;
                default:
                    SortedList = PostIDList.OrderByDescending(s => s.LastReplyTime).ToList();
                    ViewBag.LastSort = "latest_desc";
                    break;
            }

            //var privateMessagePosts = db.PrivateMessagePosts.Include(p => p.PrivateMessage).Include(p => p.User);
            return View(SortedList.ToList());
        }

        // GET: PrivateMessagePosts/Details/5
        [Authorize]
        public ActionResult Details(int? id) //id of privatemessage
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
 
            var list = db.PrivateMessagePosts.Where(r => r.PrivateMessage.Id == id).Include(p => p.User).Include(r => r.PrivateMessage);

            if (list == null)
            {
                return HttpNotFound();
            }

            PrivateMessage privateMessage = db.PrivateMessages.Find(id);
            ViewBag.SubjectOfMessage = privateMessage.Subject;
            ViewBag.deleted = "[Deleted]";
            ViewBag.ID = privateMessage.Id;
            ViewBag.UserID = System.Web.HttpContext.Current.User.Identity.GetUserId();
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
            if (!UserNameListConverted.Contains(currentUserUsername))
            {
                UserNameListConverted.Add(currentUserUsername);
            }

            //Make a list of their actual ApplicationUser models
            List<ApplicationUser> RecipientList = new List<ApplicationUser>();
            foreach (String UserName in UserNameListConverted)
            {
                ApplicationUser temp = db.Users.SingleOrDefault(g => g.UserName == UserName);
                if (temp != null)
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

            //I casted it as a List basically... I dunno, it's stupid but it works.
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
                PrivateMessage temp = db.PrivateMessages.Find(id);
                ViewBag.PostTitle = temp.Subject;
                ViewBag.ID = temp.Id;
            }
            return View();
        }

        // POST: PrivateMessagePosts/Reply
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Reply(int? id, PrivateMessagePost model)
        {
            ApplicationUser user = getCurrentUser();
            PrivateMessage temp = db.PrivateMessages.Find(id);
            var post = new PrivateMessagePost
            {
                Body = model.Body,
                User = user,
                PrivateMessage = temp,
                Date = DateTime.Now,
                IsDeleted = false,
                IsImportant = false
            };

            db.PrivateMessagePosts.Add(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult ReplyDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrivateMessagePost privateMessagePost = db.PrivateMessagePosts.Find(id);
            if (privateMessagePost == null)
            {
                return HttpNotFound();
            }
            return View(privateMessagePost);
        }

        // POST: PrivateMessagePosts/Delete/5
        [HttpPost, ActionName("ReplyDelete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult ReplyDeleteConfirmed(int id)
        {
            var userId = User.Identity.GetUserId();

            //I casted it as a List basically... I dunno, it's stupid but it works.
            PrivateMessagePost pm = db.PrivateMessagePosts.Find(id);
            pm.IsDeleted = true;

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
