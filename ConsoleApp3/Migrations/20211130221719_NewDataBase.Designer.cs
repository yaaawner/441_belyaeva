﻿// <auto-generated />
using System;
using ConsoleApp3;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ConsoleApp3.Migrations
{
    [DbContext(typeof(UserContext))]
    [Migration("20211130221719_NewDataBase")]
    partial class NewDataBase
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.0-rc.2.20475.6");

            modelBuilder.Entity("ConsoleApp3.DetectedObject", b =>
                {
                    b.Property<int>("DetectedObjectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("BitmapImage")
                        .HasColumnType("BLOB");

                    b.Property<string>("Path")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ResultsId")
                        .HasColumnType("INTEGER");

                    b.Property<float>("x1")
                        .HasColumnType("REAL");

                    b.Property<float>("x2")
                        .HasColumnType("REAL");

                    b.Property<float>("y1")
                        .HasColumnType("REAL");

                    b.Property<float>("y2")
                        .HasColumnType("REAL");

                    b.HasKey("DetectedObjectId");

                    b.HasIndex("ResultsId");

                    b.ToTable("DetectedObject");
                });

            modelBuilder.Entity("ConsoleApp3.Results", b =>
                {
                    b.Property<int>("ResultsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.HasKey("ResultsId");

                    b.ToTable("Results");
                });

            modelBuilder.Entity("ConsoleApp3.DetectedObject", b =>
                {
                    b.HasOne("ConsoleApp3.Results", null)
                        .WithMany("DetectedObjects")
                        .HasForeignKey("ResultsId");
                });

            modelBuilder.Entity("ConsoleApp3.Results", b =>
                {
                    b.Navigation("DetectedObjects");
                });
#pragma warning restore 612, 618
        }
    }
}