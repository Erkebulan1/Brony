﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brony.Service.Models.Stadiums
{
    public class StadiumCreateModel
    {
        public string Name { get; set; }
        public float Width { get; set; }
        public float Length { get; set; }
        public decimal Price { get; set; }
        public string Location { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public int BeforeCancellationTimeInHours { get; set; }
        public TimeSpan StartWorkingTime { get; internal set; }
        public TimeSpan EndWorkingTime { get; internal set; }
    }
}
