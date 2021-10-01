namespace SamuraiApp.Domain
{
    public class Quote
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public Samurai Samurai { get; set; }    //navigation property - this is for entity navigation purposes to make sure a relationship is established
        public int SamuraiId { get; set; }  //because Samurai class has a Quote object as a property, EF Core knows that
                                            //the foreign key is the parent class name plus Id
    }
}
