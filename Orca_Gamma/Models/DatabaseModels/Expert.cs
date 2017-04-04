using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Orca_Gamma.Models.DatabaseModels {
	public class Expert {

		[Key]
		[ForeignKey("User")]
		public String Id {
			get; set;
		}

		[ForeignKey("Catagory")]
		public int CatagoryId {
			get; set;
		}

		public virtual ApplicationUser User {
			get; set;
		}

		public virtual Catagory Catagory {
			get; set;
		}
	}
}