﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Orca_Gamma.Models
{

    public class ProjectViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { set; get; }

        public string DateStarted { set; get; }
        public string DateFinished { set; get; }

    }


}