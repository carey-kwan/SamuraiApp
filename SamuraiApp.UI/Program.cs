using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SamuraiApp.UI
{
    class Program
    {
        private static SamuraiContext _context = new SamuraiContext();
        private static SamuraiContext _contextNT = new SamuraiContext();    //when you do a QuickWatch to see the values of the _contextNT.ChangeTracker.Entries().results
                                                                            //it is empty because changes in value are not being tracked at all
        private static void Main(string[] args)
        {
            //_context.Database.EnsureCreated();
            //GetSamurais("Before Add:");
            //AddSamurai();
            //GetSamurais("After Add:");
            //Console.Write("Press any key...");
            //Console.ReadKey();

            //AddSamurais("Julie2", "Sampson2");
            //GetSamurais("");

            //AddSamuraisByName("Shimada", "Okamoto", "Kikuchio", "Hayashida");
            //AddVariousTypes();
            //QueryFilters();
            //QueryAggregates();
            //RetrieveAndUpdateSamurai();
            //RetrieveAndUpdateMultipleSamurais();
            //MultipleDatabaseOperations();
            //RetrieveAndDeleteASamurai();
            //QueryAndUpdateBattles_Disconnected();
            //InsertNewSamuraiWithAQuote();
            //InsertNewSamuraiWithManyQuotes();
            //AddQuoteToExistingSamuraiNotTracked(2);
            //Simpler_AddQuoteToExistingSamuraiNotTracked(2);
            //EagerLoadSamuraiWithQuotes();
            //ProjectSomeProperties();
            //ProjectSamuraisWithQuotes();
            //ExplicitLoadQuotes();
            //FilteringWithRelatedData();
            //ModifyingRelatedDataWhenTracked();                    //will need to change the Id to run a second time as there is a delete
            //ModifyingRelatedDataWhenNotTracked();
            //AddingNewSamuraiToAnExisingBattle();
            //ReturnBattleWithSamurais();
            //ReturnAllBattlesWithSamurais();
            //AddAllSamuraisToAllBattles();
            //RemoveSamuraiFromBattle();
            //WillNotRemoveSamuraiFromABattle();
            //RemoveSamuraiFromBattleExplicit();
            //AddNewSamuraiWithHorse();
            //AddNewHorseToSamuraiUsingId();
            //AddnewHorseToSamuraiObject();
            //AddNewHorseToDisconnectedSamuraiObject();

            //ReplaceHorse();

            Console.Write($"\n\rPress any key...");
            Console.ReadKey();

        }

        //single insert by object
        private static void AddSamurai()
        {
            var samurai = new Samurai { Name = "Sampson" };
            _context.Samurais.Add(samurai);   // samurai gets added as an in-memory collection
            _context.SaveChanges();           //this will save the data the context is tracking to the database
        }

        //multiple inserts by object
        private static void AddSamurais(params string[] names)
        {
            foreach (string name in names)
            {
                _context.Samurais.Add(new Samurai { Name = name});
            }
            _context.SaveChanges();
        }

        private static void GetSamurais(string text)
        {
            var samurais = _context.Samurais
                .TagWith("ConsoleApp.Program.GetSamurais method")   //this shows up as a comment when using SQL Server Profiler to track what is being executed
                .ToList();
            Console.WriteLine($"{text}: Samurai count is {samurais.Count}");
            foreach (var samurai in samurais)
            {
                Console.WriteLine(samurai.Name);
            }
        }

        //example of a merge join.  interesting sql statement
        private static void AddSamuraisByName(params string[] names)
        {
            foreach (string name in names)
            {
                _context.Samurais.Add(new Samurai { Name = name });
            }
            _context.SaveChanges();
        }

        //multiple inserts by object
        private static void AddVariousTypes()   //adding random data, not related
        {
            //_context.Samurais.AddRange(
            //    new Samurai { Name = "Shimada" },
            //    new Samurai { Name = "Okamoto" });
            //_context.Battles.AddRange(
            //    new Battle { Name = "Battle of Anegawa" },
            //    new Battle { Name = "Battle of Nagashino" });

            _context.AddRange(new Samurai { Name = "Shimada" },
                  new Samurai { Name = "Okamoto" },
                  new Battle { Name = "Battle of Anegawa" },
                  new Battle { Name = "Battle of Nagashino" });

            _context.SaveChanges();
        }

        private static void AddSamurais(Samurai[] samurais)
        {
            //AddRange can take an array or an IEnumerable e.g. List<Samurai>
            _context.Samurais.AddRange(samurais);
            _context.SaveChanges();
        }

        //select using LIKE
        private static void QueryFilters()
        {
            //s is the input parameter represents samurai type that is being queries
            //=> is the lambda operator
            //the rest is the lambda expression that needs to be evaluated that uses the input parameter

            //var name = "Sampson";
            //var samurais = _context.Samurais.Where(s => s.Name == name).ToList();
            var filter = "J%";
            var samurais = _context.Samurais
                .Where(s => EF.Functions.Like(s.Name, filter)).ToList();
        }

        private static void QueryAggregates()
        {
            //var name = "Sampson";
            //var samurai = _context.Samurais.Where(s => s.Name == name).FirstOrDefault();
            //--OR--
            //var samurai = _context.Samurais.FirstOrDefault(s => s.Name == name);

            //the below statement is a DbSet command and not a LINQ method
            var samurai = _context.Samurais.Find(2);    //select using Id, not LINQ, DbSet function
        }

        //retrieve single row and update 
        private static void RetrieveAndUpdateSamurai()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Name += "San";
            _context.SaveChanges();
        }

        //skip 1 row, retrieve next 4 and perform batch update on 4 rows
        private static void RetrieveAndUpdateMultipleSamurais()
        {
            var samurais = _context.Samurais.Skip(1).Take(4).ToList();
            samurais.ForEach(s => s.Name += "San");
            _context.SaveChanges();
        }

        //update and insert operations executed with one SaveChanges()
        private static void MultipleDatabaseOperations()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Name += "San";
            _context.Samurais.Add(new Samurai { Name = "Shino" });
            _context.SaveChanges();
        }

        //select samurai by Id and then delete.  delete is performed by marking a row for removal and then doing a SaveChanges()
        private static void RetrieveAndDeleteASamurai()
        {
            //var samurai = _context.Samurais.Find(18);     //note: i did not a record with samuraiId=18 in the table
            var samurai = _context.Samurais.Find(12);       //will need to change the Id if ran a second time
            _context.Samurais.Remove(samurai);
            _context.SaveChanges();
        }

        private static void QueryAndUpdateBattles_Disconnected()
        {
            List<Battle> disconnectedBattles;
            using (var context1 = new SamuraiContext())
            {
                disconnectedBattles = _context.Battles.ToList();
            } //context1 is disposed, this is to simulate a disconnected scenario
            disconnectedBattles.ForEach(b =>
            {
                b.StartDate = new DateTime(1570, 01, 01);
                b.EndDate = new DateTime(1570, 12, 1);
            });
            using (var context2 = new SamuraiContext()) //since this is "disconnected," all fields and the Id are passed in for update.  in a connected scenario, only the id and modified field(s) are updated
            {
                context2.UpdateRange(disconnectedBattles);  //UpdateRange marks the objects as modified
                context2.SaveChanges();
            }
        }

        //insert a parent and one child record
        private static void InsertNewSamuraiWithAQuote()
        {
            var samurai = new Samurai
            {
                Name = "Kambei Shimada",
                Quotes = new List<Quote> {
                    new Quote { Text = "I've come to save you" }
                }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        //insert parent and multiple child records
        private static void InsertNewSamuraiWithManyQuotes()
        {
            var samurai = new Samurai
            {
                Name = "Kyūzō",
                Quotes = new List<Quote> {
                    new Quote {Text = "Watch out for my sharp sword!"},
                    new Quote {Text="I told you to watch out for the sharp sword! Oh well!" }
                }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        //add child record to existing parent
        private static void AddQuoteToExistingSamuraiWhileTracked()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Quotes.Add(new Quote
            {
                Text = "I bet you're happy that I've saved you!"
            });
            _context.SaveChanges();
        }

        //scenario: new child added to existing parent
        private static void AddQuoteToExistingSamuraiNotTracked(int samuraiId)
        {
            var samurai = _context.Samurais.Find(samuraiId);
            samurai.Quotes.Add(new Quote
            {
                Text = "Now that I saved you, will you feed me dinner?"
            });
            using (var newContext = new SamuraiContext())   //new dbcontect in disconnected scenario
            {                                               //change tracker response to scenario:
                //newContext.Samurais.Update(samurai);        //1. as child's key is not set, state will automatically be "added."  this has an extra update for samurai even though there are no changes to samurai.
                newContext.Samurais.Attach(samurai);          //attach fixes that. state is instead "unmodified." 
                newContext.SaveChanges();                     //2. child's FK value to parent (e.g. Quote.Samuraiid) is set to parent's key
            }
        }


        //really no need for the context to do anything with the 
        //samurai object, so it is easier to set the foreign key
        private static void Simpler_AddQuoteToExistingSamuraiNotTracked(int samuraiId)
        {
            var quote = new Quote { Text = "Thanks for dinner!", SamuraiId = samuraiId };
            using var newContext = new SamuraiContext();    //C# 8 - do not have to wrap variable with ()
            {
                newContext.Quotes.Add(quote);
                newContext.SaveChanges();
            }
        }


        //eager loading allows you to retreive data with related data in the same call
        public static void EagerLoadSamuraiWithQuotes()
        {
            //the next two commands retreive all samurais and includes their quotes if they have any
            //var samuraiWithQuotes = _context.Samurais.Include(s => s.Quotes).ToList();          //this method does it with one query using LEFT JOIN
            //var splitQuery = _context.Samurais.AsSplitQuery().Include(s => s.Quotes).ToList();  //this method returns the data with two queries, but has better performance

            //var filteredInclude = _context.Samurais
            //    .Include(s => s.Quotes.Where(q => q.Text.Contains("Thanks"))).ToList();

            //var filterPrimaryEntityWithInclude =
            //    _context.Samurais.Where(s => s.Name.Contains("Sampson"))
            //        .Include(s => s.Quotes).FirstOrDefault();

            //notes *****
            //include child objects
            //_context.Samurais.Include(s => s.Quotes)

            //include children and grandchildren
            //_context.Samurais.Include(s=>s.Quotes).ThenInclude(q=>q.Translations)

            //include just grandchildren
            //_context.Samurais.Include(s=>s.Quotes.Translations)

            //include different children
            //_context.Samurais.Include(s=>s.Quotes).Include(s=>s.Clan)

        }
        //END EAGER LOADING



        //BEGIN QUERY PROJECTION
        //query projection defines the shape of the query results. basically like how you 
        //write columns in a SELECT statement by either rearranging or excluding columns
        private static void ProjectSomeProperties()
        {
            var someProperties = _context.Samurais.Select(s => new { s.Id, s.Name }).ToList();  //projecting an anonymous type, type is know only to method
            var idAndNames = _context.Samurais.Select(s => new IdAndName(s.Id, s.Name)).ToList();
        }
        public struct IdAndName
        {
            public IdAndName(int id, string name)
            {
                Id = id;
                Name = name;
            }
            public int Id;
            public string Name;
        }

        private static void ProjectSamuraisWithQuotes()
        {
            //var somePropsWithQuotes = _context.Samurais
            //.Select(s => new { s.Id, s.Name, s.Quotes })
            //.Select(s => new { s.Id, s.Name, NumberOfQuotes=s.Quotes.Count })
            //.ToList();

            //.Select(s => new { s.Id, s.Name,
            //    HappyQuotes = s.Quotes.Where(q => q.Text.Contains("happy"))
            //})
            //.ToList();
            //NOTE:  1. EF Core can only track entities recognized by the DbContext.
            //       2. Annonymous types are not tracked.
            //       3. Entities that are properties of an anonymous type are tracked.


            //projecting full entity objects while filtering the related objects that are also returned
            var samuraisAndQuotes = _context.Samurais
                .Select(s => new
                {
                    Samurai = s,
                    HappyQuotes = s.Quotes.Where(q => q.Text.Contains("happy"))
                })
                .ToList();
            var firstsamurai = samuraisAndQuotes[0].Samurai.Name += " The Happiest";    //entities are tracked when they are returned by the results of a projected query
        }
        //*** END QUERY PROJECTION ***



        //explicit loading is retrieving related data for objects already in memory
        private static void ExplicitLoadQuotes()
        {
            //make sure there's a horse in the DB, then clear the context's change tracker
            _context.Set<Horse>().Add(new Horse { SamuraiId = 1, Name = "Mr. Ed" });
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            //----------
            var samurai = _context.Samurais.Find(1);
            _context.Entry(samurai).Collection(s => s.Quotes).Load();   //can only use load for a single object
            _context.Entry(samurai).Reference(s => s.Horse).Load();     //so cannot use it for a list of samurais
        }


        //private static void LazyLoadQuotes()    //not recommended
        //{
        //    var samurai = _context.Samurais.Find(2);
        //    var quoteCount = samurai.Quotes.Count();    //won't run without lazy loading setup
        //}


        private static void FilteringWithRelatedData()  //just return the samurai data.  do not see the quote
        {
            var samurais = _context.Samurais
                                .Where(s => s.Quotes.Any(q => q.Text.Contains("happy")))
                                .ToList();
        }


        //BEGIN Modifying Related Data ***
        //***************************************
        private static void ModifyingRelatedDataWhenTracked()
        {
            var samurai = _context.Samurais.Include(s => s.Quotes)
                                .FirstOrDefault(s => s.Id == 2);    //get samurai with samuraiId=2 and update first quote
            samurai.Quotes[0].Text = "Did you hear that?";
            _context.Quotes.Remove(samurai.Quotes[2]);              //can delete related data when change tracker is active.  originally 4 quotes.
            _context.SaveChanges();
        }

        private static void ModifyingRelatedDataWhenNotTracked()
        {
            var samurai = _context.Samurais.Include(s => s.Quotes)
                                    .FirstOrDefault(s => s.Id == 2);
            var quote = samurai.Quotes[0];
            quote.Text += "Did you hear that again?";

            using var newContext = new SamuraiContext();
            //newContext.Quotes.Update(quote);                      //updates all the quotes associated with the samurai and updates the name of the samurai.  all objects in the graph are updated
            newContext.Entry(quote).State = EntityState.Modified;   //updates only the specific item
            newContext.SaveChanges();
        }

        private static void AddingNewSamuraiToAnExisingBattle()
        {
            var battle = _context.Battles.FirstOrDefault();
            battle.Samurais.Add(new Samurai { Name = "Takeda Shingen" });
            _context.SaveChanges();

        }

        //NOTE: Battle and Samurai tables have a many to many relationship
        private static void ReturnBattleWithSamurais()
        {
            var battle = _context.Battles.Include(b => b.Samurais).FirstOrDefault();

            //SELECT t.BattleId, t.EndDate, t.Name, t.StartDate, t0.BattleId, t0.SamuraiId, t0.DateJoined, t0.Id, t0.Name
            //FROM (
            //        SELECT TOP(1)[b].[BattleId], [b].[EndDate], [b].[Name], [b].[StartDate]
            //        FROM [Battles] As[b]
            //) AS[t]
            //LEFT JOIN (
            //    SELECT  [b0].[BattleId], [b0].[SamuraiId], [b0].[DateJoined], [s].[Id], [s].[Name]
            //    FROM [BattleSamurai] AS [b0]
            //    INNER JOIN [Samurais] AS[s] ON [b0].[SamuraiId] = s.[Id]
            //) AS[t0] ON[t].[BattleId] = [t0].[BattleId]
        }

        private static void ReturnAllBattlesWithSamurais()
        {
            var battles = _context.Battles.Include(b => b.Samurais).ToList();
        }

        private static void AddAllSamuraisToAllBattles()
        {
            //NOTE: There is already a row in the BattleSamurai table, so the below code causes a PK/FK violation
            //var allbattles = _context.Battles.ToList();
            //var allSamurais = _context.Samurais.ToList();
            //foreach (var battle in allbattles)
            //{
            //    battle.Samurais.AddRange(allSamurais);
            //}
            //_context.SaveChanges();

            //var allbattles = _context.Battles.ToList();
            //var allSamurais = _context.Samurais.Where(s=>s.Id!=22).ToList();    //cheap workaround. instructor said real solution is too advanced for getting started course
            //foreach (var battle in allbattles)
            //{
            //    battle.Samurais.AddRange(allSamurais);
            //}
            //_context.SaveChanges();

            //easiest solution is to eager load the samurais along with the battles? -- need to research this
            var allbattles = _context.Battles.Include(b => b.Samurais).ToList();
            var allSamurais = _context.Samurais.ToList();
            foreach (var battle in allbattles)
            {
                battle.Samurais.AddRange(allSamurais);  //the context will mark any preexisting samurais as unchanged
            }                                           //so only inserts that will be sent to the database are the ones that are truly new
            _context.SaveChanges(); //note that this will hurt performance if there is a lot of related data, because it slows down the change tracker
        }


        private static void RemoveSamuraiFromBattle()
        {
            //note: have to have the full graph in memory to remove a many to may join
            var battleWithSamurai = _context.Battles
                .Include(b => b.Samurais.Where(s => s.Id == 12))    //example of removing one samurai from one battle
                .Single(s => s.BattleId == 1);                      //will need to change Id value if ran again
            var samurai = battleWithSamurai.Samurais[0];
            battleWithSamurai.Samurais.Remove(samurai);
            _context.SaveChanges();
        }

        private static void WillNotRemoveSamuraiFromABattle()
        {
            //just getting the battle and samurai will not work
            var battle = _context.Battles.Find(1);
            var samurai = _context.Samurais.Find(12);
            battle.Samurais.Remove(samurai);
            _context.SaveChanges();     //relationship is not being tracked
        }  //NOTE: Deleting a M2M relationship is easier with a stored procedure ot explicit M2M mapping

        private static void RemoveSamuraiFromBattleExplicit()
        {
            var b_s = _context.Set<BattleSamurai>()
                .SingleOrDefault(bs => bs.BattleId == 1 && bs.SamuraiId == 10);
            if (b_s != null)
            {
                //b_s.DateJoined = DateTime.Now;    //can use the same logic above to update data

                _context.Remove(b_s);       //_context.Set<BattleSamurai>().Remove works too
                _context.SaveChanges();
            }
        }



        private static void AddNewSamuraiWithHorse()
        {
            var samurai = new Samurai { Name = "Jina Ujichika" };
            samurai.Horse = new Horse { Name = "Silver" };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }


        private static void AddNewHorseToSamuraiUsingId()
        {   //adding new horse to an existing samurai who does not have a horse yet

            var horse = new Horse { Name = "Scout", SamuraiId = 2 };
            _context.Add(horse);
            _context.SaveChanges();
        }

        private static void AddnewHorseToSamuraiObject()
        {
            var samurai = _context.Samurais.Find(2);
            samurai.Horse = new Horse { Name = "Black Beauty" };
            _context.SaveChanges();
        }

        private static void AddNewHorseToDisconnectedSamuraiObject()
        {
            var samurai = _context.Samurais.AsNoTracking().FirstOrDefault(s => s.Id == 5);
            samurai.Horse = new Horse { Name = "Mr. Ed" };

            using var newContext = new SamuraiContext();
            newContext.Samurais.Attach(samurai);    //using attach, it checks to see if a record exists already and if there is, it marks the data as unmodified to avoid a PK violation
            newContext.SaveChanges();
        }


        //SCENARIO - what happens if a samurai gets a new horse?
        //EX: Changing the child of an existing parent
        //NOTE: database model does not allow a horse to exist without a samurai

        private static void ReplaceHorse()
        {
            var samurai = _context.Samurais.Include(s => s.Horse)               //EF Core will delete the old horse from the database
                                            .FirstOrDefault(s => s.Id == 5);    //and do an insert with the new horse
            samurai.Horse = new Horse { Name = "Trigger" };
            _context.SaveChanges();
        }

        private static void GetSamuraiWithHorse()
        {
            var samurais = _context.Samurais.Include(s => s.Horse).ToList();    //context for horse does not exist
        }

        private static void GetHorsesWithSamurai()
        {
            var horseonly = _context.Set<Horse>().Find(3);

            var horseWithSamurai = _context.Samurais.Include(s => s.Horse)
                                            .FirstOrDefault(s => s.Horse.Id == 3);

            var horseSamuraiPairs = _context.Samurais
                .Where(s => s.Horse != null)
                .Select(s => new { Horse = s.Horse, Samuari = s })
                .ToList();
        }





        //BEGIN WORKING WITH VIEWS AND STORED PROCEDURES
        private static void QuerySamuraiBattleStats()
        {   //SamuraiBattleStats is a view
            //var stats = _context.SamuraiBattleStats.ToList();

            //var firststat = _context.SamuraiBattleStats.FirstOrDefault();
            //var sampsonState = _context.SamuraiBattleStats
            //    .FirstOrDefault(b => b.Name == "SampsonSan");

            var findone = _context.SamuraiBattleStats.Find(2);  //dones not work.find is a method of DbSet, so this throws
                                                                //an exception since there are no keys
        }

        private static void QueryUsingRawSql()
        {
            var samurais = _context.Samurais.FromSqlRaw("Select * from samurais").ToList();
        }

        private static void QueryRelatedUsingRawSql()
        {
            var samurais = _context.Samurais.FromSqlRaw(
                "Select Id, Name from samurais").Include(s => s.Quotes).ToList();
        }

        private static void QueryUsingRawSqlWithInterpolation()
        {
            string name = "Kikuchyo";
            var samurais = _context.Samurais
                .FromSqlInterpolated($"Select * from Samurais Where Name= {name}")
                .ToList();
        }

        private static void DANGERQueryUsingRawSqlWithInterpolation()
        {   //this returns invalid column name for the parameter 
            //string name = "Kikuchyo";
            //var samurais = _context.Samurais
            //    .FromSqlRaw($"Select * from Samurais Where Name= {name}")
            //    .ToList();

            //this risks SQL injection
            string name = "Kikuchyo";
            var samurais = _context.Samurais
                .FromSqlRaw($"Select * from Samurais Where Name= '{name}'")
                .ToList();
        }

        private static void QueryUsingSqlRawStoredProc()
        {
            var text = "Happy";
            var samurais = _context.Samurais.FromSqlRaw(
                "EXEC dbo.SamuraiWhoSaidAWord {0}", text).ToList();
        }

        private static void QueryUsingFromSqlIntStoredproc()
        {
            var text = "Happy";
            var samurais = _context.Samurais.FromSqlInterpolated(
                $"EXEC dbo.SamuraiWhoSaidAWord {text}").ToList();
        }


        private static void ExecuteSomeRawSql()
        {
            var samuraiId = 2;
            //var affected = _context.Database
            //    .ExecuteSqlRaw("EXEC DeleteQuotesForSamurai {0}", samuraiId);

            var affected = _context.Database
                .ExecuteSqlInterpolated($"EXEC DeleteQuotesForSamurai {samuraiId}");

        }



    }
}

