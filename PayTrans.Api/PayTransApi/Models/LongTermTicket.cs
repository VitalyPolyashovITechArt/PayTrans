using System;
using System.ComponentModel.DataAnnotations;

namespace PayTransApi.Models
{
    public class LongTermTicket
    {
        [Key]
        public int Id { get; set; }

         public int RemainingTrips { get; set; }

         public DateTime ExpirationDateTime { get; set; } // public int DaysToExpiration { get; set; }
    }
}