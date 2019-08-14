﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBB.Web.Models
{
    public class RegisterViewModel
    {
        [Required, DisplayName ("Nome")]
        public string Name { get; set; }

        [Required, DisplayName ("Password")]
        public string Password { get; set; }

        [Required, DisplayName("Repetir Password")]
        public string RepeatPassword { get; set; }

        [Required, DisplayName("Apresentação")]
        public string Description { get; set; }
    }
}
