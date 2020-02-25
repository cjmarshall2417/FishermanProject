using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fishermen.Models
{
    [Table("tblLocations")]
    public partial class TblLocations
    {
        public TblLocations()
        {
            TblHauls = new HashSet<TblHauls>();
        }

        [Key]
        [Column("LocationID")]
        public int LocationId { get; set; }
        public int AreaNumber { get; set; }
        [StringLength(100)]
        public string AreaName { get; set; }
        [StringLength(50)]
        public string SubRegion { get; set; }
        [Column("RegionID")]
        public int? RegionId { get; set; }
        [Column("SystemID")]
        public int? SystemId { get; set; }

        [ForeignKey(nameof(RegionId))]
        [InverseProperty(nameof(TblRegions.TblLocations))]
        public virtual TblRegions Region { get; set; }
        [ForeignKey(nameof(SystemId))]
        [InverseProperty(nameof(TblSystems.TblLocations))]
        public virtual TblSystems System { get; set; }
        [InverseProperty("Location")]
        public virtual ICollection<TblHauls> TblHauls { get; set; }
    }
}
