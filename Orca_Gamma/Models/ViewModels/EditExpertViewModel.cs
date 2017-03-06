using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orca_Gamma.Models.DatabaseModels;

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

        //public Dictionary<string, string> Categories
        //{
        //    get; set;
        //}

        public Catagory SelectedCatagory
        {
            get; set;
        }
    }
}