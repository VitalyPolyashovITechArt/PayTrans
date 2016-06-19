namespace PayTransApi.Models
{
    public class PayResult
    {
        public bool Succeeded { get; set; }

        public ErrorType ErrorType { get; set; }

        public ActiveTicket ActiveTicket { get; set; }
    }

    public enum ErrorType
    {
        OutOfLimit,
        PaymentSystemFailed
    }
}