using Orca_Gamma.Models.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orca_Gamma.Models
{
    public class ExpertInfoViewModel
    {
        public Catagory Catagory
        {
            get; set;
        }

        public IEnumerable<Keyword> Keywords
        {
            get; set;
        }
    }
}