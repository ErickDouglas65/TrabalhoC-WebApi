namespace Trabalho
{
    public class Rental
    {
        public int Id { get; set; }
        public DateTime? RentalDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }

        public int GameId { get; set; }
        public Game Game { get; set; }
    }
}
