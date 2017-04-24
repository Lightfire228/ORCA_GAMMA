using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orca_Gamma.Models
{
    public class ProjectViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { set; get; }
        public DateTime DateStarted { set; get; }
        public DateTime DateFinished { set; get; }
        public ApplicationUser User { get; set; }
        public List<ApplicationUser> CollaboratorList { get; set; }
    }


}