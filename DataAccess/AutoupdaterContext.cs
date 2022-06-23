﻿using Microsoft.EntityFrameworkCore;
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

        public AutoupdaterContext(DbContextOptions<AutoupdaterContext> options)
        : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Telemetry>();
        }
    }
}
