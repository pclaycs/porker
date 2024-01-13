﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MrPorker.Data;

#nullable disable

namespace MrPorker.Migrations
{
    [DbContext(typeof(BotDbContext))]
    [Migration("20240113121823_AddAlexMeasurementTable")]
    partial class AddAlexMeasurementTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("MrPorker.Data.Models.HoroscopeModel", b =>
                {
                    b.Property<ulong>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Sign")
                        .HasColumnType("INTEGER");

                    b.HasKey("UserId");

                    b.ToTable("Horoscopes");
                });

            modelBuilder.Entity("MrPorker.Data.Models.PhraseModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Phrases");
                });

            modelBuilder.Entity("MrPorker.Data.Models.SubModels.AddymerMeasurementModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<float>("BasalMetabolicRate")
                        .HasColumnType("REAL");

                    b.Property<float>("BodyFat")
                        .HasColumnType("REAL");

                    b.Property<float>("BodyMassIndex")
                        .HasColumnType("REAL");

                    b.Property<float>("BodyWater")
                        .HasColumnType("REAL");

                    b.Property<float>("BoneMass")
                        .HasColumnType("REAL");

                    b.Property<float>("FatFreeBodyWeight")
                        .HasColumnType("REAL");

                    b.Property<float>("MetabolicAge")
                        .HasColumnType("REAL");

                    b.Property<float>("MuscleMass")
                        .HasColumnType("REAL");

                    b.Property<float>("Protein")
                        .HasColumnType("REAL");

                    b.Property<string>("Remarks")
                        .HasColumnType("TEXT");

                    b.Property<float>("SkeletalMuscle")
                        .HasColumnType("REAL");

                    b.Property<float>("SubcutaneousFat")
                        .HasColumnType("REAL");

                    b.Property<long>("Timestamp")
                        .HasColumnType("INTEGER");

                    b.Property<float>("VisceralFat")
                        .HasColumnType("REAL");

                    b.Property<float>("Weight")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("AddymerMeasurements");
                });

            modelBuilder.Entity("MrPorker.Data.Models.SubModels.AlexMeasurementModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<float>("BasalMetabolicRate")
                        .HasColumnType("REAL");

                    b.Property<float>("BodyFat")
                        .HasColumnType("REAL");

                    b.Property<float>("BodyMassIndex")
                        .HasColumnType("REAL");

                    b.Property<float>("BodyWater")
                        .HasColumnType("REAL");

                    b.Property<float>("BoneMass")
                        .HasColumnType("REAL");

                    b.Property<float>("FatFreeBodyWeight")
                        .HasColumnType("REAL");

                    b.Property<float>("MetabolicAge")
                        .HasColumnType("REAL");

                    b.Property<float>("MuscleMass")
                        .HasColumnType("REAL");

                    b.Property<float>("Protein")
                        .HasColumnType("REAL");

                    b.Property<string>("Remarks")
                        .HasColumnType("TEXT");

                    b.Property<float>("SkeletalMuscle")
                        .HasColumnType("REAL");

                    b.Property<float>("SubcutaneousFat")
                        .HasColumnType("REAL");

                    b.Property<long>("Timestamp")
                        .HasColumnType("INTEGER");

                    b.Property<float>("VisceralFat")
                        .HasColumnType("REAL");

                    b.Property<float>("Weight")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("AlexMeasurements");
                });

            modelBuilder.Entity("MrPorker.Data.Models.SubModels.PaulMeasurementModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<float>("BasalMetabolicRate")
                        .HasColumnType("REAL");

                    b.Property<float>("BodyFat")
                        .HasColumnType("REAL");

                    b.Property<float>("BodyMassIndex")
                        .HasColumnType("REAL");

                    b.Property<float>("BodyWater")
                        .HasColumnType("REAL");

                    b.Property<float>("BoneMass")
                        .HasColumnType("REAL");

                    b.Property<float>("FatFreeBodyWeight")
                        .HasColumnType("REAL");

                    b.Property<float>("MetabolicAge")
                        .HasColumnType("REAL");

                    b.Property<float>("MuscleMass")
                        .HasColumnType("REAL");

                    b.Property<float>("Protein")
                        .HasColumnType("REAL");

                    b.Property<string>("Remarks")
                        .HasColumnType("TEXT");

                    b.Property<float>("SkeletalMuscle")
                        .HasColumnType("REAL");

                    b.Property<float>("SubcutaneousFat")
                        .HasColumnType("REAL");

                    b.Property<long>("Timestamp")
                        .HasColumnType("INTEGER");

                    b.Property<float>("VisceralFat")
                        .HasColumnType("REAL");

                    b.Property<float>("Weight")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("Measurements");
                });
#pragma warning restore 612, 618
        }
    }
}
