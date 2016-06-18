using System;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace PayTransApi.Models
{
    public class ActiveTicket
    {
        [Key]
        public int Id { get; set; }

        public TransportType Type { get; set; } 

        public DateTime ValidateDate { get; set; }

        public int Route { get; set; }

        public string TransportNumber { get; set; }
    }

    public enum TransportType
    {
        Tram,
        TrolleyBus,
        Bus
    }
}