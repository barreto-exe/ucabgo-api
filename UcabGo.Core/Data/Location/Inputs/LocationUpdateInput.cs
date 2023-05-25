﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UcabGo.Core.Data.Location.Inputs
{
    public class LocationUpdateInput
    {
        [Required]
        public int Id { get; set; }
        public string Alias { get; set; }
        public string Zone { get; set; }
        public string Detail { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
    }
}