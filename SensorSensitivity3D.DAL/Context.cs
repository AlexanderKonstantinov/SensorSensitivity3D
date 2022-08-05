using System;
using System.Drawing;
using SensorSensitivity3D.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SensorSensitivity3D.DAL
{
    public class Context : DbContext
    {
        private static string _dbPath;
        //public static Context Instance => _context ?? (_context = new Context());

        public static void ContextConfiguring(string dbPath)
        {
            _dbPath = dbPath;
        }

        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<Geophone> Geophones { get; set; }
        public DbSet<ControlZone> ControlZones { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={_dbPath}");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Configuration>()
                .Property(e => e.Name)
                .IsRequired();

            modelBuilder.Entity<Geophone>()
                .Property(e => e.X)
                .IsRequired();

            modelBuilder.Entity<Geophone>()
                .Property(e => e.Y)
                .IsRequired();

            modelBuilder.Entity<Geophone>()
                .Property(e => e.Z)
                .IsRequired();

            modelBuilder.Entity<Geophone>()
                .Property(e => e.R)
                .IsRequired();

            modelBuilder.Entity<Geophone>()
                .Property(e => e.Color)
                .IsRequired();

            modelBuilder.Entity<Geophone>()
                .Property(e => e.GIsVisible)
                .IsRequired();

            modelBuilder.Entity<Geophone>()
                .Property(e => e.SIsVisible)
                .IsRequired();

            modelBuilder.Entity<Geophone>()
                .Property(e => e.IsGood)
                .IsRequired();

            modelBuilder.Entity<ControlZone>()
                .Property(e => e.Color)
                .IsRequired();

            modelBuilder.Entity<ControlZone>()
                .Property(e => e.IsVisible)
                .IsRequired();

            modelBuilder.Entity<ControlZone>()
                .Property(e => e.XMin)
                .IsRequired();

            modelBuilder.Entity<ControlZone>()
                .Property(e => e.XMax)
                .IsRequired();

            modelBuilder.Entity<ControlZone>()
                .Property(e => e.YMin)
                .IsRequired();

            modelBuilder.Entity<ControlZone>()
                .Property(e => e.YMax)
                .IsRequired();

            modelBuilder.Entity<ControlZone>()
                .Property(e => e.ZMin)
                .IsRequired();

            modelBuilder.Entity<ControlZone>()
                .Property(e => e.ZMax)
                .IsRequired();

            modelBuilder.Entity<ControlZone>()
                .Property(e => e.IsCalculated)
                .IsRequired();

            modelBuilder.Entity<ControlZone>()
                .Property(e => e.SMin)
                .IsRequired();

            modelBuilder.Entity<ControlZone>()
                .Property(e => e.SMax)
                .IsRequired();

            modelBuilder.Entity<ControlZone>()
                .Property(e => e.K)
                .IsRequired();

            Seed(modelBuilder);
        }
        
        private void Seed(in ModelBuilder modelBuilder)
        {
            var rnd = new Random();
            const string drawingName = @"Pioner.dxf";
            //const string drawingPath = @"C:\Users\Александер\Documents\GitHub\SensorSensitivity3D\SensorSensitivity3D\Pioner.dxf";

            for (var i = 0; i < 30; ++i)
            {
                modelBuilder.Entity<Geophone>()
                .HasData(
                new Geophone
                {
                    Id = i + 1,
                    ConfigId = rnd.Next(1, 4),
                    Name = $"Геофон {i}",
                    HoleNumber = i,
                    X = rnd.Next(-820, -450),
                    Y = rnd.Next(-1260, -990),
                    Z = rnd.Next(100, 210),
                    IsGood = true,
                    Color = $"#{Color.FromArgb(70, rnd.Next(256), rnd.Next(256), rnd.Next(256)).Name}",
                    GIsVisible = rnd.Next(0, 2) > 0,
                    SIsVisible = rnd.Next(0, 2) > 0,
                    R = 50
                });
            }

            modelBuilder.Entity<Configuration>()
                .HasData(
                new Configuration
                {
                    Id = 1,
                    Name = "Конфигурация 1",
                    SubstrateName = drawingName
                },

                new Configuration
                {
                    Id = 2,
                    Name = "Конфигурация 2",
                    SubstrateName = drawingName
                },

                new Configuration
                {
                    Id = 3,
                    Name = "Конфигурация 3",
                    SubstrateName = drawingName
                }
                );
        }
    }
}
