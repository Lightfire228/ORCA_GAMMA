using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Orca_Gamma.Models.DatabaseModels {
	public class PrivateMessage {
		
		[Key]
		public int Id {
			get; set;
		}

		[Required]
		[ForeignKey("User")]
		public String UserId {
			get; set;
		}

		public DateTime Date {
			get; set;
		}
        
		public String Subject {
			get; set;
		}

		public Boolean IsDeleted {
			get; set;
		}


		public virtual ApplicationUser User {
			get; set;
		}
	}
}