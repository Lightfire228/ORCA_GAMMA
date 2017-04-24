using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Orca_Gamma.Models;
using Orca_Gamma.Models.DatabaseModels;
using Orca_Gamma.Models.ViewModels;

namespace Orca_Gamma.Controllers {
	public class HomeController : Controller {

        private Models.ApplicationDbContext db = new Models.ApplicationDbContext();

        public ActionResult Index(string sortOrder, string previousFilter, string searchString, string lastSort)
        {
            var expertList = (from ex in db.Experts
                             select ex).ToList();
            var model = new List<MainIndexViewModel>();

            foreach(Expert expert in expertList)
            {
                var keywordList = db.KeywordRelations.Where(k => k.ExpertId.Equals(expert.Id)).Select(k => k.Keyword.Name).ToList();
                String commaSeperated = "";
                foreach (String keyword in keywordList)
                {
                    commaSeperated += keyword + ", ";
                }
                if (!commaSeperated.Equals(""))
                {
                    commaSeperated = commaSeperated.TrimEnd(' ');
                    commaSeperated = commaSeperated.TrimEnd(',');
                }

                var temp = new MainIndexViewModel
                {
                    User = db.Users.Find(expert.Id),
                    Catagory = db.Catagories.Find(expert.CatagoryId),
                    Keywords = commaSeperated
                };
                model.Add(temp);
            }

            ViewBag.CurrentSort = sortOrder;
            if (lastSort != sortOrder)
            {
                ViewBag.LastSort = sortOrder;
            }
            else
            {
                if (sortOrder == "username_desc")
                {
                    sortOrder = "username_asc";
                }
                else if (sortOrder == "username_asc")
                {
                    sortOrder = "username_desc";
                }
                else if (sortOrder == "name_asc")
                {
                    sortOrder = "name_desc";
                }
                else if (sortOrder == "name_desc")
                {
                    sortOrder = "name_asc";
                }
                else if (sortOrder == "category_asc")
                {
                    sortOrder = "category_desc";
                }
                else if (sortOrder == "category_desc")
                {
                    sortOrder = "category_asc";
                }
            }

            ViewBag.SortParm = String.IsNullOrEmpty(sortOrder);

            if (searchString == null)
            {
                searchString = previousFilter;
            }
            ViewBag.CurrentFilter = searchString;

            if (!String.IsNullOrEmpty(searchString) && model != null)
            {
                searchString = searchString.ToLower();
                var trueModel = new List<MainIndexViewModel>();
                foreach(MainIndexViewModel item in model)
                {
                    if (item.User.FirstName.ToLower().Contains(searchString)
                        || item.User.LastName.ToLower().Contains(searchString)
                        || item.User.UserName.ToLower().Contains(searchString)
                        || item.Keywords.ToLower().Contains(searchString)
                        || item.Catagory.Name.ToLower().Contains(searchString))
                    {
                        trueModel.Add(item);
                    }
                }
                return View(trueModel);
            }

            List<MainIndexViewModel> SortedList;
            switch (sortOrder)
            {
                case "username_asc":
                    SortedList = model.OrderBy(s => s.User.UserName).ToList();
                    break;
                case "username_desc":
                    SortedList = model.OrderByDescending(s => s.User.UserName).ToList();
                    break;
                case "name_asc":
                    SortedList = model.OrderBy(s => s.User.LastName).ToList();
                    break;
                case "name_desc":
                    SortedList = model.OrderByDescending(s => s.User.LastName).ToList();
                    break;
                case "category_asc":
                    SortedList = model.OrderBy(s => s.Catagory.Name).ToList();
                    break;
                case "category_desc":
                    SortedList = model.OrderByDescending(s => s.Catagory.Name).ToList();
                    break;
                default:
                    SortedList = model.OrderBy(s => s.Catagory.Name).ToList();
                    ViewBag.LastSort = "category_asc";
                    break;
            }
            return View(SortedList.ToList());
        }

		public ActionResult About() {
			ViewBag.Message = "Multidiscipline Active Research Collaborative Organization System, also known as MARCOS, designed by the ORCA_GAMMA Software Development Team.";

			return View();
		}

		public ActionResult Contact() {
			ViewBag.Message = "Contact Us:";

			return View();
		}

        //GET
        public ActionResult UserProfile(String id)
        {
            var profile = db.Users.Find(id);
            var expert = db.Experts.Find(id);
            if (expert != null)
            {
                var catagory = db.Catagories.Find(expert.CatagoryId);
                ViewBag.Catagory = catagory.Name;
            }
            else
            {
                ViewBag.Catagory = "Not an Expert";
            }

            return View(profile);
        }
    }
}