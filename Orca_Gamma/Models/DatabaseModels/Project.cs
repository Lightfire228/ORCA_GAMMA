using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Orca_Gamma.Models.DatabaseModels {
	public class Project {

		[Key]
		public int Id {
			get; set;
		}

		[Required]
		[ForeignKey("User")]
		public String ProjectLead {
			get; set;
		}

		public String Name {
			get; set;
		}

		public String Description {
			get; set;
		}


		public virtual ApplicationUser User {
			get; set;
		}
       
	}
   
    
}