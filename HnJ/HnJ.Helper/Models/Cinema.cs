﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HnJ.Helper.Models
{
    public class Cinema
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Address { get; set; }

        public Lazy<List<CinemaAmenity>> CinemaAmenities { get; set; }
    }
}
