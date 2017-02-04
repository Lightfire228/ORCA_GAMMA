using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Orca_Gamma.Models.DatabaseModels {
	public class ThreadMessagePost {

		[Key]
		public int Id {
			get; set;
		}

		[Required]
		[ForeignKey("User")]
		public String CreatedBy {
			get; set;
		}

		[Required]
		[ForeignKey("Thread")]
		public int PartOf {
			get; set;
		}

		public String Body {
			get; set;
		}

		public DateTime Date {
			get; set;
		}


		public virtual ApplicationUser User {
			get; set;
		}

		public virtual ForumThread Thread {
			get; set;
		}

	}
}