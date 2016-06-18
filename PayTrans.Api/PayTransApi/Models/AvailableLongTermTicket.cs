namespace PayTransApi.Models
{
    public class AvailableLongTermTicket
    {
        public AvailableLongTermTicket(int id, int amountOfTrips, int amountOfDays, int cost)
        {
            Id = id;
            AmountOfTrips = amountOfTrips;
            AmountOfDays = amountOfDays;
            Cost = cost;
        }

        public int Id { get; set; }

        public int AmountOfTrips { get; set; }

        public int AmountOfDays { get; set; } 

        public int Cost { get; set; }
    }
}