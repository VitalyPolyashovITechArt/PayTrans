using System;
using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using PayTransApi.Models;

namespace PayTransApi.Controllers
{
    [Authorize]
    public class ApiTicketController : ApiController
    {
        private const int Rate = 5500;

        private readonly AvailableLongTermTicket[] AvailableTickets = new AvailableLongTermTicket[]
        {
            new AvailableLongTermTicket(1, 20, 30, 75),
            new AvailableLongTermTicket(2, 20, 60, 85), 
            new AvailableLongTermTicket(3, 60, 30, 200), 
            new AvailableLongTermTicket(4, 60, 60, 250)
        };

        [HttpGet]
        public ActiveTicket Active()
        {
            var id = User.Identity.GetUserId();
            var user = new ApplicationDbContext().Users.FirstOrDefault(x => x.Id == id);
            return user.ActiveTicket;
        }

        [HttpGet]
        public LongTermTicket LongTerm()
        {
            var id = User.Identity.GetUserId();
            var user = new ApplicationDbContext().Users.FirstOrDefault(x => x.Id == id);
            return user.LongTermTicket;
        }

        [HttpGet]
        public AvailableLongTermTicket[] GetAvailableLongTermTickets()
        {
            return AvailableTickets;
        }

        [HttpPost]
        public BuyResult BuyLongTermTicket(int ticketId)
        {
            if (new Random().Next(5) == 0)
            {
                return new BuyResult()
                {
                    Succeeded = false,
                    ErrorType = BuyErrorType.PaymentSystemFailed
                };
            }
            
            var id = User.Identity.GetUserId();
            var chosenTicket = AvailableTickets.First(x => x.Id == ticketId);

            using (var context = new ApplicationDbContext())
            {
                var user = context.Users.FirstOrDefault(x => x.Id == id);

                if (user.LongTermTicket != null)
                {
                    return new BuyResult()
                    {
                        Succeeded = false,
                        ErrorType = BuyErrorType.AlreadyExists
                    };
                }

                if (user.ParentUser != null)
                {
                    if (user.Limit - chosenTicket.Cost < 0)
                    {
                        return new BuyResult()
                        {
                            Succeeded = false,
                            ErrorType = BuyErrorType.OutOfLimit
                        };
                    }
                }

                var longTermTicket = new LongTermTicket()
                {
                    ExpirationDateTime = DateTime.UtcNow.AddDays(chosenTicket.AmountOfDays),
                    RemainingTrips = chosenTicket.AmountOfTrips
                };

                if (user.LongTermTicket.RemainingTrips > 0)
                {
                    user.LongTermTicket.RemainingTrips--;
                }
                else if (user.ParentUser != null)
                {
                    user.Limit -= Rate;
                    // TODO send notification
                }
                else
                {
                    // TODO pay
                }

                user.LongTermTicket = longTermTicket;
                context.SaveChanges();
            }

            return new BuyResult()
            {
                Succeeded = true,
                DaysToExpiration = chosenTicket.AmountOfDays,
                RemainingTrips = chosenTicket.AmountOfTrips
            };
        }

        [HttpPost]
        public PayResult Pay(PayRequest request)
        {
            if (request.Type == TransportType.Tram) // emulate payment system failure
            {
                return new PayResult()
                {
                    Succeeded = false,
                    ErrorType = ErrorType.PaymentSystemFailed
                };
            }

            var id = User.Identity.GetUserId();
            ActiveTicket activeTicket;
            using (var context = new ApplicationDbContext())
            {
                var user = context.Users.FirstOrDefault(x => x.Id == id);
                
                if (user.ParentUser != null)
                {
                    if (user.Limit - Rate < 0)
                    {
                        return new PayResult()
                        {
                            Succeeded = false,
                            ErrorType = ErrorType.OutOfLimit
                        };
                    }
                }
                
                activeTicket = new ActiveTicket()
                {
                    Route = request.Route,
                    TransportNumber = request.TransportNumber,
                    Type = request.Type,
                    ValidateDate = DateTime.UtcNow
                };

                if (user.LongTermTicket.RemainingTrips > 0)
                {
                    user.LongTermTicket.RemainingTrips--;
                }
                else if (user.ParentUser != null)
                {
                    user.Limit -= Rate;
                    // TODO send notification
                }
                else
                {
                    // TODO pay
                }

                user.ActiveTicket = activeTicket;
                context.SaveChanges();
                activeTicket.Id = user.ActiveTicket.Id;
            }

            return new PayResult()
            {
                Succeeded = true,
                ActiveTicket = activeTicket,
                Token = Encryptor.Crypt(string.Format("{0}|{1}|{2}|{3}|{4}", activeTicket.Id, activeTicket.Type, activeTicket.Route, activeTicket.TransportNumber, activeTicket.ValidateDate))
            };
        }
    }
}
