using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Orca_Gamma.Models.DatabaseModels {
	public class Catagory {
		
		[Key]
		public int Id {
			get; set;
		}

		[Required]
		[Display(Name = "Category")]
		public String Name {
			get; set;
		}
	}
}