using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orca_Gamma.Models.DatabaseModels;

namespace Orca_Gamma.Models.ViewModels
{
    public class MainIndexViewModel
    {
        public ApplicationUser User
        {
            get; set;
        }

        public Catagory Catagory
        {
            get; set;
        }

        public String Keywords
        {
            get; set;
        }
    }
}