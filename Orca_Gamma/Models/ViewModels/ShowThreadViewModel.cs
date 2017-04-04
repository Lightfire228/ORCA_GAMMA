using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orca_Gamma.Models.DatabaseModels;

namespace Orca_Gamma.Models.ViewModels
{
    public class ShowThreadViewModel
    {
        public ForumThread ForumThread { get; set; }
        public List<ThreadMessagePost> ThreadMessagePost {get; set; }
    }
}