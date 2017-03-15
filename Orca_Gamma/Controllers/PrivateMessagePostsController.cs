﻿using System;
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
        public ActionResult Index()
        {
            var privateMessagePosts = db.PrivateMessagePosts.Include(p => p.PrivateMessage).Include(p => p.User);
            return View(privateMessagePosts.ToList());
        }

        // GET: PrivateMessagePosts/Details/5
        public ActionResult Details(int? id)
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

        // GET: PrivateMessagePosts/Create
        public ActionResult Create()
        {
            //ViewBag.PartOf = new SelectList(db.PrivateMessages, "Id", "UserId");
            //ViewBag.CreatedBy = new SelectList(db.ApplicationUsers, "Id", "FirstName");
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

            var initial = new PrivateMessage
            {
                UserId = currentUserId,
                Date = DateTime.Now,
                Subject = model.PrivateMessage.Subject,
                IsDeleted = false,
                User = user
            };

            var post = new PrivateMessagePost
            {
                CreatedBy = user.FirstName,
                PartOf = initial.Id,
                Body = model.Body,
                User = user,
                PrivateMessage = initial
            };

            var between = new PrivateMessageBetween
            {
                PrivateMessageId = post.Id,
                UserId = currentUserId,
                User = user,
                PrivateMessage = initial
            };

            if (ModelState.IsValid)
            {
                db.PrivateMessages.Add(initial);
                db.PrivateMessagePosts.Add(post);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View(post);
        }

        // GET: PrivateMessagePosts/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.PartOf = new SelectList(db.PrivateMessages, "Id", "UserId", privateMessagePost.PartOf);
            ViewBag.CreatedBy = new SelectList(db.ApplicationUsers, "Id", "FirstName", privateMessagePost.CreatedBy);
            return View(privateMessagePost);
        }

        // POST: PrivateMessagePosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CreatedBy,PartOf,Body")] PrivateMessagePost privateMessagePost)
        {
            if (ModelState.IsValid)
            {
                db.Entry(privateMessagePost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PartOf = new SelectList(db.PrivateMessages, "Id", "UserId", privateMessagePost.PartOf);
            ViewBag.CreatedBy = new SelectList(db.ApplicationUsers, "Id", "FirstName", privateMessagePost.CreatedBy);
            return View(privateMessagePost);
        }

        // GET: PrivateMessagePosts/Delete/5
        public ActionResult Delete(int? id)
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PrivateMessagePost privateMessagePost = db.PrivateMessagePosts.Find(id);
            db.PrivateMessagePosts.Remove(privateMessagePost);
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
