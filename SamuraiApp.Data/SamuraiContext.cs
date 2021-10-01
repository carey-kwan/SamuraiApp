using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamuraiApp.Data
{
    //DbContext is an important class in Entity Framework API.It is a bridge 
    //between your domain or entity classes and the database. The primary class that 
    //is responsible for interacting with data as objects DbContext.

    public class SamuraiContext : DbContext //DbContext provides logic for EF Core to interact with the database
    {
        public DbSet<Samurai> Samurais { get; set; }    //dbsets become tables in the database
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Battle> Battles { get; set; }
        public DbSet<SamuraiBattleStat> SamuraiBattleStats { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //usesqlserver extenstion came from nuget package
            optionsBuilder.UseSqlServer(
                "Data Source= (localdb)\\MSSQLLocalDB; Initial Catalog=SamuraiAppData"
                , options=>options.MaxBatchSize(100) 
                )
            //.LogTo(Console.WriteLine);

            //.LogTo(Console.Write, new[] {DbLoggerCategory.Database.Command.Name,
            //                         DbLoggerCategory.Database.Transaction.Name
            //                        },
            //LogLevel.Debug);

            .LogTo(Console.Write, new[] { DbLoggerCategory.Database.Command.Name },
                LogLevel.Debug)
            .EnableSensitiveDataLogging();

            //Other LogTo Target Examples
            //
            //private StreamWriter _writer
            //  = new StreamWriter("EFCoreLog.txt", append: true);
            //optionsBuilder
            //  .LogTo(_writer.WriteLine)
            //
            //---OR---
            //
            //optionsBuilder
            //  .LogTo(log=>Debug.WriteLine(log));      //lambda expression for debug.writeline

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //many to many is expressed using HasMany and WithMany
            //note: there is no difference which class object is 
            //asscociated with HasMany or WithMany

            modelBuilder.Entity<Samurai>()          //samurai is used as the reference point for navigation. Samurai is used here instead of Battle, because Samurai was the original focus of the project
                .HasMany(s => s.Battles)            //points to Battles property of the Samurai class
                .WithMany(b => b.Samurais)          //points to Samurais property of the Battle class   //these three lines lets EF Core that we are referencing this relationship in the model
                .UsingEntity<BattleSamurai>                 //instead of inferring the many to many join
                (bs => bs.HasOne<Battle>().WithMany(),      //these lines explicitly define the relationship between Battle and Samurai with the BattleSamurai class
                bs => bs.HasOne<Samurai>().WithMany())      //this states that BattleSamurai has a one to many relationship with Samurai and BattleSamurai has a one to many relationship to Battle
                .Property(bs => bs.DateJoined)
                .HasDefaultValueSql("getdate()");   //last line is only added to define a default value for the Datejoined column    

            modelBuilder.Entity<Horse>().ToTable("Horses");

            ////NOTE: EF Core will never ever track entities marked with HasNoKey()
            modelBuilder.Entity<SamuraiBattleStat>().HasNoKey().ToView("SamuraiBattleStats");   //the .ToView is to make sure that the context knows this is a view otherwise the DBset 
            //                                                                                    //above will be interpreted as a table that needs to be created during a migration.


        }

    }
}
