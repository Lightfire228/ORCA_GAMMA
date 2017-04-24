using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
namespace Orca_Gamma.Models.ViewModels
{
    public class CreatePostViewModel
    {
        public int Id { get; set; }
        [Required]
        public String Body { get; set; }
        [Required]
        public String Subject { get; set; }
        [Required]
        public String FirstPost { get; set; }
    }
}