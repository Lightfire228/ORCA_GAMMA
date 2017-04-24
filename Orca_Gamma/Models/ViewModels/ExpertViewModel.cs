using Orca_Gamma.Models.DatabaseModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Orca_Gamma.Models
{
    public class ExpertViewModel
    {
        [Key]
        [ForeignKey("User")]
        public String Id
        {
            get; set;
        }

        [ForeignKey("Catagory")]
        public int CatagoryId
        {
            get; set;
        }

        public Expert Expert
        {
            get; set;
        }

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