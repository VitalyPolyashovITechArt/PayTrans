using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PayTransApi.Models
{
    public class AccountSettings
    {
        [Key]
        public int Id { get; set; }

        public bool AllowExpirationNotifications { get; set; }

        public PaySettings PaySettings { get; set; }
    }

    public class PaySettings
    {
        [Key]
        public int Id { get; set; }

        public ICollection<PayPalSettings> PayPalSettings { get; set; }

        public ICollection<CreditCardSettings> CreditCardSettings { get; set; }

        public ICollection<MobileSettings> MobileSettings { get; set; }
    }

    public class PayPalSettings
    {
        [Key]
        public int Id { get; set; }

        public string Email { get; set; }
    }

    public class CreditCardSettings
    {
        [Key]
        public int Id { get; set; }

        public string CardNumber { get; set; }

        public string Expiration { get; set; }
    }

    public class MobileSettings
    {
        [Key]
        public int Id { get; set; }

        public string OperatorName { get; set; }

        public string Number { get; set; }
    }
}