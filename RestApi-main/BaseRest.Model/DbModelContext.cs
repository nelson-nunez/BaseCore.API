using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Proxies;
using BaseRest.Core.Model.Model;

namespace BaseRest.Core.Model
{
    public class DbModelContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                              .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                              .AddJsonFile("appsettings.json")
                              .Build();
                var tt = configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(tt))
                    throw new Exception("No hay string de conexión..."); 
                
                optionsBuilder.UseSqlServer(tt)
                              .UseLazyLoadingProxies();
            }
        }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>().HasKey(x => new { x.Id});
            modelBuilder.Entity<Gender>().HasKey(x => new { x.Id});

            //Elimina ciclos de eliminacion en cascada
            var cascadeFKs = modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetForeignKeys()).Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);
            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            modelBuilder.Seed();
        }

        #region DB SET
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Gender> Gender { get; set; }
        public DbSet<AppUser> AppUser { get; set; }

        #endregion
    }

    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            if (modelBuilder != null)
            {
                SeedCustomer(modelBuilder);
                SeedGender(modelBuilder);
                SeedAppUser(modelBuilder);
            }
        }
        private static void SeedCustomer(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().HasData(
            new Customer
            {
                Id = 1,
                Name = "Juana Maria Perez",
                BirthDate = new DateTime(1990, 1, 1),
                CUIL = "12345678910",
                GenderId = 1,
                Phone = "364412345678",
                Created = DateTime.Now,
            },            
            new Customer
            {
                Id = 2,
                Name = "Carlos Canto",
                BirthDate = new DateTime(1994, 1, 1),
                CUIL = "12345678910",
                GenderId = 2,
                Phone = "364412345678",
                Created = DateTime.Now,
            },            
            new Customer
            {
                Id = 3,
                Name = "Manuel Sosa",
                BirthDate = new DateTime(1980, 1, 1),
                CUIL = "12345678910",
                GenderId = 3,
                Phone = "364412345678",
                Created = DateTime.Now,
            });
        }
        private static void SeedGender(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Gender>().HasData(
            new Gender
            {
                Id = 1,
                Name = "Femenino",
                Created = DateTime.Now,
            },
            new Gender
            {
                Id = 2,
                Name = "Masculino",
                Created = DateTime.Now,
            },
            new Gender
            {
                Id = 3,
                Name = "Otro",
                Created = DateTime.Now,
            });
        }
        private static void SeedAppUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>().HasData(
            new AppUser
            {
                Id = 1,
                Name = "admin",
                Password = "admin",
            });
        }
    }
}
