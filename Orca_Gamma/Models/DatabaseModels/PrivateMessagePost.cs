using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Orca_Gamma.Models.DatabaseModels {
	public class PrivateMessagePost {

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
		[ForeignKey("PrivateMessage")]
		public int PartOf {
			get; set;
		}
        
		public String Body {
			get; set;
		}


		public virtual ApplicationUser User {
			get; set;
		}

		public virtual PrivateMessage PrivateMessage {
			get; set;
		}

	}
}