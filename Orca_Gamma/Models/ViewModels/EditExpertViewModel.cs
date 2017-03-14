using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orca_Gamma.Models.DatabaseModels;
using System.ComponentModel.DataAnnotations;

namespace Orca_Gamma.Models
{
    public class EditExpertViewModel
    {
        public Expert Expert
        {
            get; set;
        }

        public IEnumerable<Catagory> Categories
        {
            get; set;
        }

		[Display(Name = "Select your Catagory")]
        public int SelectedCatagory
        {
            get; set;
        }
    }
}