using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Orca_Gamma.Models
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "RoleName")]
        public string Name { get; set; }
    }

    public class EditUserViewModel
    {
        public string Id { get; set; }

        //[Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "User Name")]
        //[EmailAddress]
        public string UserName { get; set; }

		[Display(Name = "First Name")]
		public string FirstName {
			get; set;
		}

		[Display(Name = "Last Name")]
		public string LastName {
			get; set;
		}

		[Display(Name = "Phone Number")]
		public string PhoneNumber {
			get; set;
		}

		public IEnumerable<SelectListItem> RolesList { get; set; }
    }
}