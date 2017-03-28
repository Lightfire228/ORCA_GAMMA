using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Orca_Gamma.Models.DatabaseModels {
	public class UpDownVote {

		[Key][Column(Order = 1)]
		[ForeignKey("User")]
		public String VotedBy {
			get; set;
		}

		[Key][Column(Order = 2)]
		[ForeignKey("Post")]
		public int PostVoted {
			get; set;
		}

		/**
		 * When a user upvotes a post, this is true.
		 * When a user downvotes a post, this is false.
		 * 
		 * This DB design enforces the user can only have 1 vote
		 * per post, and this is the toggle for up or down
		 * 
		 * -- Cass
		 */
		public bool IsUpVote {
			get; set;
		}


		public virtual ApplicationUser User {
			get; set;
		}

		public virtual ThreadMessagePost Post {
			get; set;
		}
	}
}