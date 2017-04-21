using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orca_Gamma.Models.DatabaseModels;
using System.ComponentModel.DataAnnotations;


namespace Orca_Gamma.Models.ViewModels
{
    public class LeadTransferViewModel
    {
        public Project project
        {
            get; set;
        }

        public List<String> collabList
        {
            get; set;
        }

        public String selectedCollab
        {
            get; set;
        }
    }
}