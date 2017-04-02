using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Orca_Gamma.Models.DatabaseModels {
	public class ThreadKeyword {

		[Key][Column(Order = 1)]
		[ForeignKey("Keyword")]
		public int KeywordId {
			get; set;
		}

		[Key][Column(Order = 2)]
		[ForeignKey("Thread")]
		public int ThreadId {
			get; set;
		}


		public virtual Keyword Keyword {
			get; set;
		}

		public virtual ForumThread Thread {
			get; set;
		}

	}
}