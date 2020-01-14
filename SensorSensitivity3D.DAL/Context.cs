using System;
using System.Collections.Generic;
using System.Drawing;
using SensorSensitivity3D.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SensorSensitivity3D.DAL
{
    public class Context : DbContext
    {
        private static Context _context = null;

        static Context() => _context = new Context();

        public static Context Instance => _context;


        public List<Configuration> Configurations;
        public List<Drawing> Drawings;
        public List<Geophone> Geophones;

        public Context()
        {
            Geophones = new List<Geophone>();
            var rnd = new Random();
            for (var i = 0; i < 30; ++i)
            {
                Geophones.Add(new Geophone
                {
                    Id = i,
                    ConfigId = rnd.Next(0, 3),
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

            Drawings = new List<Drawing>
            {
                new Drawing ()
                {
                    Id = 0,
                    ConfigId = 0,
                    Path = @"G:\Git\SensorSensitivity3D\SensorSensitivity3D\Pioner.dxf"
                }
            };

            Configurations = new List<Configuration>
            {
                new Configuration
                {
                    Id = 0,
                    Name = "Конфигурация 1"
                },

                new Configuration
                {
                    Id = 1,
                    Name = "Конфигурация 2"
                },

                new Configuration
                {
                    Id = 2,
                    Name = "Конфигурация 3"
                }
            };
        }


        //public DbSet<Configuration> Configurations { get; set; }
        //public DbSet<Configuration> Models { get; set; }
        //public DbSet<Configuration> Sensors { get; set; }

        //private static bool _created;

        //public Context(DbContextOptions<Context> options)
        //    : base(options)
        //{
        //    if (!_created)
        //    {
        //        _created = true;
        //        Database.EnsureCreated();
        //    }
        //}

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    var connectionStringBuilder = new SqliteConnectionStringBuilder {DataSource = "test.db"};
        //    var connectionString = connectionStringBuilder.ToString();
        //    var connection = new SqliteConnection(connectionString);

        //    optionsBuilder.UseSqlite(connection);
        //}

        //public class ContextFactory : IDesignTimeDbContextFactory<Context>
        //{
        //    public Context CreateDbContext(string[] args)
        //    {
        //        var optionsBuilder = new DbContextOptionsBuilder<Context>();
        //        optionsBuilder.UseSqlite("Data Source=SSDB.db");

        //        return new Context(optionsBuilder.Options);
        //    }
        //}
    }
}
