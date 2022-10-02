using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess
{
    public class AutoupdaterContext : DbContext
    {
        public DbSet<Mod> Mods { get; set; }
        //public DbSet<Telemetry> Telemetry { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<EarlyAccessStatus> EarlyAccess { get; set; }
    
        public AutoupdaterContext(DbContextOptions<AutoupdaterContext> options)
        : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Telemetry>();
            modelBuilder.Ignore<TelemetryHandler>();
            
            modelBuilder.Entity<EarlyAccessStatus>(entity => {
                entity.Property(m => m.Username).HasMaxLength(127);
                entity.Property(m => m.Steam64).HasMaxLength(127);
                entity.Property(m => m.ModId).HasMaxLength(127);
                entity.HasKey(k => new { k.ModId, k.Steam64 });
            });
        }
    }
}
