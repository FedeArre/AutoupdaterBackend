﻿// <auto-generated />
using System;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DataAccess.Migrations
{
    [DbContext(typeof(AutoupdaterContext))]
    partial class AutoupdaterContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.26")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Objects.EAS", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Steam64")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("Username")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("eagGroupName")
                        .HasColumnType("varchar(767)");

                    b.HasKey("Id");

                    b.HasIndex("eagGroupName");

                    b.ToTable("EarlyAccessTesters");
                });

            modelBuilder.Entity("Objects.EarlyAccessGroup", b =>
                {
                    b.Property<string>("GroupName")
                        .HasColumnType("varchar(767)");

                    b.Property<string>("OwnerUsername")
                        .HasColumnType("varchar(767)");

                    b.HasKey("GroupName");

                    b.HasIndex("OwnerUsername");

                    b.ToTable("EarlyAccess");
                });

            modelBuilder.Entity("Objects.Mod", b =>
                {
                    b.Property<string>("ModId")
                        .HasColumnType("varchar(767)");

                    b.Property<int>("CPC")
                        .HasColumnType("int");

                    b.Property<string>("FileName")
                        .HasColumnType("text");

                    b.Property<string>("ModAuthor")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("ModId");

                    b.ToTable("Mods");
                });

            modelBuilder.Entity("Objects.ModAllowed", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("GroupName")
                        .HasColumnType("varchar(767)");

                    b.Property<string>("ModId")
                        .HasColumnType("varchar(767)");

                    b.Property<string>("ModIdentifierString")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("GroupName");

                    b.HasIndex("ModId");

                    b.ToTable("ModAllowed");
                });

            modelBuilder.Entity("Objects.ModDailyData", b =>
                {
                    b.Property<string>("ModId")
                        .HasColumnType("varchar(767)");

                    b.Property<DateTime>("DataDate")
                        .HasColumnType("datetime");

                    b.Property<int>("PlayerCount")
                        .HasColumnType("int");

                    b.HasKey("ModId");

                    b.ToTable("ModDailyData");
                });

            modelBuilder.Entity("Objects.ModVersion", b =>
                {
                    b.Property<string>("ModId")
                        .HasColumnType("varchar(767)");

                    b.Property<string>("DownloadLink")
                        .HasColumnType("text");

                    b.Property<int>("RequiredGameBuildId")
                        .HasColumnType("int");

                    b.Property<string>("Version")
                        .HasColumnType("text");

                    b.HasKey("ModId");

                    b.ToTable("ModVersion");
                });

            modelBuilder.Entity("Objects.User", b =>
                {
                    b.Property<string>("Username")
                        .HasColumnType("varchar(767)");

                    b.Property<string>("DiscordId")
                        .HasColumnType("text");

                    b.Property<string>("DiscordVerificationToken")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("TokenAPI")
                        .HasColumnType("text");

                    b.HasKey("Username");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Objects.EAS", b =>
                {
                    b.HasOne("Objects.EarlyAccessGroup", "eag")
                        .WithMany("Users")
                        .HasForeignKey("eagGroupName");
                });

            modelBuilder.Entity("Objects.EarlyAccessGroup", b =>
                {
                    b.HasOne("Objects.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerUsername");
                });

            modelBuilder.Entity("Objects.ModAllowed", b =>
                {
                    b.HasOne("Objects.EarlyAccessGroup", "Group")
                        .WithMany()
                        .HasForeignKey("GroupName");

                    b.HasOne("Objects.Mod", "Mod")
                        .WithMany("Allowed")
                        .HasForeignKey("ModId");
                });

            modelBuilder.Entity("Objects.ModVersion", b =>
                {
                    b.HasOne("Objects.Mod", null)
                        .WithOne("LatestVersion")
                        .HasForeignKey("Objects.ModVersion", "ModId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}