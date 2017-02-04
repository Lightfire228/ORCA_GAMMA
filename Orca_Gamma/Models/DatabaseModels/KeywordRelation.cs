using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Orca_Gamma.Models.DatabaseModels {
	public class KeywordRelation {

		[Key][Column(Order = 1)]
		[ForeignKey("Expert")]
		public String ExpertId {
			get; set;
		}

		[Key][Column(Order = 2)]
		[ForeignKey("Keyword")]
		public int KeywordId {
			get; set;
		}

		public virtual Expert Expert {
			get; set;
		}

		public virtual Keyword Keyword {
			get; set;
		}
	}
}