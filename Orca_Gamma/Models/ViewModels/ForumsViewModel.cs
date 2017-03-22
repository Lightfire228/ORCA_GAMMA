using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orca_Gamma.Models.ViewModels
{
    public class ForumsViewModel
    {
        public int Id { get; set; }
        public string FirstPost { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }
        public ApplicationUser User { get; set; }
    }
}