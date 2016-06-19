namespace PayTransApi.Models
{
    public class PayRequest1
    {
        public TransportType Type { get; set; }

        public int Route { get; set; }

        public string TransportNumber { get; set; }
    }
}