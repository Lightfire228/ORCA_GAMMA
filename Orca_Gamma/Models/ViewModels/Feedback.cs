using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orca_Gamma.Models
{
    //takes what user enters and stores it for check -Geoff
    public class Feedback
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
    }
}