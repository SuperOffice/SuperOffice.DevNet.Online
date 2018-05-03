using MvcIntegrationServerApp.Migrations;
using MvcIntegrationServerApp.Models.Map;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace MvcIntegrationServerApp.Models
{
    public class AppDB : DbContext
    {
        public AppDB()
            : base("name=AppDB")
        {

        }

        public static void Initialize()
        {


            using (var db = new AppDB())
            {



                if (!db.Database.Exists() || !db.Database.CompatibleWithModel(false))
                {
                    Database.SetInitializer<AppDB>(new MigrateDatabaseToLatestVersion<AppDB, Configuration>());
                }
                else
                {
                    // Database exists and matches the current model.
                    // Doing this special thing to avoid the bug that arises sometimes.
                    Database.SetInitializer<AppDB>(null);
                }

                db.Database.Initialize(true);

            }
        }
        public static void UpgradeDatabase()
        {
            var migratorConfig = new Migrations.Configuration();
            var dbMigrator = new DbMigrator(migratorConfig);
            dbMigrator.Update();
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new CustomerMap());
        }

    }
}