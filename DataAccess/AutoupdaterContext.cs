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
        public DbSet<EarlyAccessGroup> EarlyAccess { get; set; }
        public DbSet<EAS> EarlyAccessTesters { get; set; }
        public DbSet<ModDailyData> ModDailyData { get; set; }
        public DbSet<EarlyAccessModObject> EAModObjects { get; set; }

        public AutoupdaterContext(DbContextOptions<AutoupdaterContext> options)
        : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Telemetry>();
            modelBuilder.Ignore<TelemetryHandler>();
            //modelBuilder.Entity<EAS>().HasKey(eas => new { eas.Steam64, eas.Username, eas.OwnerUsername, eas.Group });
            modelBuilder.Entity<EAS>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
