using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fishermen.Models
{
    [Table("tblHauls")]
    public partial class TblHauls
    {
        [Key]
        [Column("HaulID")]
        public int HaulId { get; set; }
        [Column("LocationID")]
        public int LocationId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Caught { get; set; }
        [StringLength(2)]
        public string Marked { get; set; }
        [StringLength(1)]
        public string Run { get; set; }

        [ForeignKey(nameof(LocationId))]
        [InverseProperty(nameof(TblLocations.TblHauls))]
        public virtual TblLocations Location { get; set; }
    }
}
