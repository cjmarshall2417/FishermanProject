using System;
using System.Collections.Generic;

namespace Fishermen.Models
{
    public partial class TblLocations
    {
        public TblLocations()
        {
            TblHauls = new HashSet<TblHauls>();
        }

        public int AreaNumber { get; set; }
        public string AreaName { get; set; }
        public int? RegionId { get; set; }
        public int? SystemId { get; set; }
        public int LocationId { get; set; }

        public virtual ICollection<TblHauls> TblHauls { get; set; }
    }
}
