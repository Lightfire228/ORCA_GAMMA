using System.Web;
using System.Web.Mvc;

namespace Orca_Gamma {
	public class FilterConfig {
		public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
			filters.Add(new HandleErrorAttribute());
		}
	}
}
