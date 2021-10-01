using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamuraiApp.Domain
{
    public class Battle
    {
        public int BattleId { get; set; }   //used this instead of just Id to make the column BattlesBatleId in the BattleSamurai table
                                            //Julie Lerman did this to explicitly show the conventions used by EF Core
        public string Name { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set;}
        
        
        public List<Samurai> Samurais { get; set; } = new List<Samurai>();  //this class collection reference is all that is needed in
                                                                            //EFCore 5 to establish many to many relationship
    }
}

