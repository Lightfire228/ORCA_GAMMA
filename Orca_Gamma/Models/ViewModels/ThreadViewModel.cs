using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orca_Gamma.Models.DatabaseModels;

namespace Orca_Gamma.Models.ViewModels
{
    public class ThreadViewModel
    {
        public ForumThread Threads { get; set; }
        public ThreadMessagePost Posts { get; set; }
    }
}