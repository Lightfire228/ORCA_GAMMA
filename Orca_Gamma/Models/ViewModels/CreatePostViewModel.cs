using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orca_Gamma.Models.ViewModels
{
    public class CreatePostViewModel
    {
        public int Id { get; set; }
        public String Body { get; set; }
        public String Subject { get; set; }
        public String Name { get; set; }
    }
}