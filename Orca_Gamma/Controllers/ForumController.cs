using PagedList;
using System;
using System.Linq;
using System.Web.Mvc;
using Orca_Gamma.Models;
using System.Net;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Orca_Gamma.Controllers
{
    public class ForumController : Controller
    {
        //Get: Post
        private ApplicationDbContext _dbContext;

        public ForumController()
        {
            _dbContext = new ApplicationDbContext();
        }

        // Get: Message
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.nameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            if (searchString !=null)
            {
                page = 1;
            } else
            {
                searchString = currentFilter;
            }
            var threads = from s in _dbContext.ForumThreads
                          select s;
            ViewBag.CurrentFilter = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {

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

        
    }
}