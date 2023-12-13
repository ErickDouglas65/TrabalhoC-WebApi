using System.Reflection.Metadata.Ecma335;

namespace Trabalho
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Cidade { get; set; }
        public int MaxRentals { get; set; }
        public int CurrentRentals { get; set; }
        public int Age { get; set; }

        // Properties navigation
        //public virtual ICollection<Game> Games { get; set; }
        public virtual ICollection<Rental> Rentals { get; set; }
    }
}
