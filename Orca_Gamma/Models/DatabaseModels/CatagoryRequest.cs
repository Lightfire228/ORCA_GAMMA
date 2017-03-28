using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Orca_Gamma.Models.DatabaseModels {
	public class CatagoryRequest {

		public int Id {
			get; set;
		}

		public String Name {
			get; set;
		}

		[ForeignKey("Expert")]
		public String RequestedBy {
			get; set;
		}


		public virtual Expert Expert {
			get; set;
		}
	}
}