namespace Trabalho
{
    public class Game
    {   public int Id { get; set; }
        public string Name { get; set; }
        public bool IsRented { get; set; }
        public string Genre { get; set; }
        public bool IsAvailable { get; set; }
        public string AgeRating { get; set; }

        // Properties navigation
        //public virtual ICollection<Client> Clients { get; set; }
        public virtual ICollection<Rental> Rentals { get; set; }
    }
}
