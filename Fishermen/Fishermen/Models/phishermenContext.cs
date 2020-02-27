using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Fishermen.Models
{
    public partial class phishermenContext : DbContext
    {
        public phishermenContext()
        {
        }

        public phishermenContext(DbContextOptions<phishermenContext> options)
            : base(options)
        {
        }

        public virtual DbSet<StatewideSteelhead19612016> StatewideSteelhead19612016 { get; set; }
        public virtual DbSet<TblHauls> TblHauls { get; set; }
        public virtual DbSet<TblLocations> TblLocations { get; set; }
        public virtual DbSet<TblQueries> TblQueries { get; set; }
        public virtual DbSet<TblRegions> TblRegions { get; set; }
        public virtual DbSet<TblSystems> TblSystems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StatewideSteelhead19612016>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("'Statewide Steelhead 1961-2016$'");

                entity.Property(e => e.AreaName).HasMaxLength(255);

                entity.Property(e => e.ExcelId)
                    .HasColumnName("ExcelID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.MU)
                    .HasColumnName("M/U")
                    .HasMaxLength(255);

                entity.Property(e => e.Region).HasMaxLength(255);

                entity.Property(e => e.Run).HasMaxLength(255);

                entity.Property(e => e.Subregion).HasMaxLength(255);

                entity.Property(e => e.System).HasMaxLength(255);
            });

            modelBuilder.Entity<TblHauls>(entity =>
            {
                entity.HasKey(e => e.HaulId)
                    .HasName("PK__tblHauls__4A3114BB05F876B1");

                entity.ToTable("tblHauls");

                entity.Property(e => e.HaulId).HasColumnName("HaulID");

                entity.Property(e => e.LocationId).HasColumnName("LocationID");

                entity.Property(e => e.Marked)
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.Run)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.TblHauls)
                    .HasForeignKey(d => d.LocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tblHaulsUpdated_tblLocationsUpdated");
            });

            modelBuilder.Entity<TblLocations>(entity =>
            {
                entity.HasKey(e => e.LocationId)
                    .HasName("PK__tblLocat__E7FEA477FEB38653");

                entity.ToTable("tblLocations");

                entity.Property(e => e.LocationId).HasColumnName("LocationID");

                entity.Property(e => e.AreaName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RegionId).HasColumnName("RegionID");

                entity.Property(e => e.SystemId).HasColumnName("SystemID");
            });

            modelBuilder.Entity<TblQueries>(entity =>
            {
                entity.HasKey(e => e.QueryId);

                entity.ToTable("tblQueries");

                entity.Property(e => e.QueryId).HasColumnName("queryID");

                entity.Property(e => e.QueryName)
                    .IsRequired()
                    .HasColumnName("queryName")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.QueryUrl)
                    .IsRequired()
                    .HasColumnName("queryURL")
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TblRegions>(entity =>
            {
                entity.HasKey(e => e.RegionId)
                    .HasName("PK_tblRegionsUpdated");

                entity.ToTable("tblRegions");

                entity.Property(e => e.RegionId).HasColumnName("RegionID");

                entity.Property(e => e.RegionName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TblSystems>(entity =>
            {
                entity.HasKey(e => e.SystemId)
                    .HasName("PK_tblSystemsUpdated");

                entity.ToTable("tblSystems");

                entity.Property(e => e.SystemId).HasColumnName("SystemID");

                entity.Property(e => e.SystemName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
