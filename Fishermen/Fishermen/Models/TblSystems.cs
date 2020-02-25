using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fishermen.Models
{
    [Table("tblSystems")]
    public partial class TblSystems
    {
        public TblSystems()
        {
            TblLocations = new HashSet<TblLocations>();
        }

        [Key]
        [Column("SystemID")]
        public int SystemId { get; set; }
        [Required]
        [StringLength(100)]
        public string SystemName { get; set; }

        [InverseProperty("System")]
        public virtual ICollection<TblLocations> TblLocations { get; set; }
    }
}
