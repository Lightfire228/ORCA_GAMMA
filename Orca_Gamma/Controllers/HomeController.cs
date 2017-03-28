using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Orca_Gamma.Models;
using Orca_Gamma.Models.DatabaseModels;

namespace Orca_Gamma.Controllers {
	public class HomeController : Controller {

        private Models.ApplicationDbContext db = new Models.ApplicationDbContext();

        public ActionResult Index(string searchString) {
            var expert = from ex in db.Experts
                          select ex;

            if (!String.IsNullOrEmpty(searchString))
            {
                expert = expert.Where(n => n.Catagory.Name.Contains(searchString));
            }
			return View(/*expert.ToList()*/);
		}

		public ActionResult About() {
			ViewBag.Message = "Multidiscipline Active Research Collaborative Organization System, also known as MARCOS, designed by the ORCA_GAMMA Software Development Team.";

			return View();
		}

		public ActionResult Contact() {
			ViewBag.Message = "Contact Us:";

			return View();
		}
	}
}