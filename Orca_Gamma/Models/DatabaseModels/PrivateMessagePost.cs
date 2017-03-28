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

		public DateTime Date {
			get; set;
		}

		/**
		 * I didn't know if we wanted to include the functionality to delete
		 * private messages (the particular post, not the whole message), so
		 * I threw it in here while I was messing with this
		 * 
		 * -- Cass
		 */
		public bool IsDeleted {
			get; set;
		}

		public bool IsImportant {
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