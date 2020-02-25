using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fishermen.Models
{
    [Table("tblRegions")]
    public partial class TblRegions
    {
        public TblRegions()
        {
            TblLocations = new HashSet<TblLocations>();
        }

        [Key]
        [Column("RegionID")]
        public int RegionId { get; set; }
        [Required]
        [StringLength(50)]
        public string RegionName { get; set; }

        [InverseProperty("Region")]
        public virtual ICollection<TblLocations> TblLocations { get; set; }
    }
}
