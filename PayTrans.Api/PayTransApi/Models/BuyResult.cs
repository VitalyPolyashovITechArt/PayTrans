namespace PayTransApi.Models
{
    public class BuyResult
    {
        public bool Succeeded { get; set; }

        public BuyErrorType ErrorType { get; set; }

        public int RemainingTrips { get; set; }

        public int DaysToExpiration { get; set; }
    }

    public enum BuyErrorType
    {
        OutOfLimit,
        PaymentSystemFailed, // TODO
        AlreadyExists
    }
}