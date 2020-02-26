using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace Fishermen.Models
{
    public partial class PhishermenContext : DbContext
    {
        public PhishermenContext()
        {
        }
        public PhishermenContext(DbContextOptions<PhishermenContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TblHauls> TblHauls { get; set; }
        public virtual DbSet<TblLocations> TblLocations { get; set; }
        public virtual DbSet<TblRegions> TblRegions { get; set; }
        public virtual DbSet<TblSystems> TblSystems { get; set; }
        public IConfiguration Configuration { get; }
       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TblHauls>(entity =>
            {
                entity.HasKey(e => e.HaulId)
                    .HasName("PK__tblHauls__4A3114BB9D848A16");

                entity.Property(e => e.Marked).IsUnicode(false);

                entity.Property(e => e.Run)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.TblHauls)
                    .HasForeignKey(d => d.LocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tblHauls_tblLocations");
            });

            modelBuilder.Entity<TblLocations>(entity =>
            {
                entity.Property(e => e.AreaName).IsUnicode(false);

                entity.Property(e => e.SubRegion).IsUnicode(false);

                entity.HasOne(d => d.Region)
                    .WithMany(p => p.TblLocations)
                    .HasForeignKey(d => d.RegionId)
                    .HasConstraintName("FK_tblLocations_tblRegions");

                entity.HasOne(d => d.System)
                    .WithMany(p => p.TblLocations)
                    .HasForeignKey(d => d.SystemId)
                    .HasConstraintName("FK_tblLocations_tblSystems");
            });

            modelBuilder.Entity<TblRegions>(entity =>
            {
                entity.Property(e => e.RegionName).IsUnicode(false);
            });

            modelBuilder.Entity<TblSystems>(entity =>
            {
                entity.Property(e => e.SystemName).IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
