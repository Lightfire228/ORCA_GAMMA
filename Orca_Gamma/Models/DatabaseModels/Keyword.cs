using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Orca_Gamma.Models.DatabaseModels {
	public class Keyword {

		[Key]
		public int Id {
			get; set;
		}

		[Required]
		public String Name {
			get; set;
		}
	}
}