using System;
using System.ComponentModel.DataAnnotations;

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

        public string Token
        {
            get
            {
                return Encryptor.Crypt(string.Format("{0}|{1}|{2}|{3}|{4}", Id, Type, Route, TransportNumber, ValidateDate));
            }
        }
    }

    public enum TransportType
    {
        Tram,
        TrolleyBus,
        Bus
    }
}