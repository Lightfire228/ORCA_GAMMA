using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Orca_Gamma.Models.DatabaseModels {
	public class PrivateMessageBetween {

		[Key][Column(Order = 1)]
		[ForeignKey("PrivateMessage")]
		public int PrivateMessageId {
			get; set;
		}

		[Key][Column(Order = 2)]
		[ForeignKey("User")]
		public String UserId {
			get; set;
		}


		public virtual PrivateMessage PrivateMessage {
			get; set;
		}

		public virtual ApplicationUser User {
			get; set;
		}

	}
}