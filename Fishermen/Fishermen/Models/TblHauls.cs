using System;
using System.Collections.Generic;

namespace Fishermen.Models
{
    public partial class TblHauls
    {
        public int HaulId { get; set; }
        public int LocationId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Caught { get; set; }
        public string Marked { get; set; }
        public string Run { get; set; }

        public virtual TblLocations Location { get; set; }
    }
}
