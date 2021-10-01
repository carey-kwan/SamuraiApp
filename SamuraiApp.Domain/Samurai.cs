using System.Collections.Generic;

namespace SamuraiApp.Domain
{
    public class Samurai
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Quote> Quotes { get; set; } = new List<Quote>();    //samurai has a one to many relationship with quotes
        public List<Battle> Battles { get; set; } = new List<Battle>(); //samurai has a many to many relationship to battle
                                                                        //NOTE: EFCore 5 does not require coding of a BattleSamurai class
                                                                        //to establish the relationship.  it is handled automatically.
        public Horse Horse { get; set; }
    }
}
