﻿// <auto-generated />
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DataAccess.Migrations
{
    [DbContext(typeof(AutoupdaterContext))]
    [Migration("20220918164307_earlyAccessUpdate")]
    partial class earlyAccessUpdate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.26")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Objects.Mod", b =>
                {
                    b.Property<string>("ModId")
                        .HasColumnType("varchar(767)");

                    b.Property<int>("CPC")
                        .HasColumnType("int");

                    b.Property<string>("DownloadLink")
                        .HasColumnType("text");

                    b.Property<bool>("EarlyAccessEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("FileName")
                        .HasColumnType("text");

                    b.Property<string>("ModAuthor")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Version")
                        .HasColumnType("text");

                    b.HasKey("ModId");

                    b.ToTable("Mods");
                });

            modelBuilder.Entity("Objects.User", b =>
                {
                    b.Property<string>("Username")
                        .HasColumnType("varchar(767)");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.HasKey("Username");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
