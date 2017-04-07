using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orca_Gamma.Models.ViewModels
{
    public class PMIndexViewModel
    {
        public int Id
        {
            get; set;
        }
        
        public String UserId
        {
            get; set;
        }
        
        public DateTime Date
        {
            get; set;
        }
        
        public String Subject
        {
            get; set;
        }

        public Boolean IsDeleted
        {
            get; set;
        }

        public Boolean IsImportant
        {
            get; set;
        }


        public virtual ApplicationUser User
        {
            get; set;
        }

        public String LastPost
        {
            get; set;
        }

        public DateTime LastReplyTime
        {
            get; set;
        }
    }
}