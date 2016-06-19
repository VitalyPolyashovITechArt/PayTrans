using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using Newtonsoft.Json;
using PayTransApi.Models;
using TransportType = PayTransApi.Models.TransportType;

namespace PayTransApi.Controllers
{
    [Authorize]
    public class ApiTicketController : ApiController
    {
        private const int Rate = 5500;

        private readonly AvailableLongTermTicket[] AvailableTickets = {
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
        public LongTermTicketViewModel LongTerm()
        {
            var id = User.Identity.GetUserId();
            var user = new ApplicationDbContext().Users.FirstOrDefault(x => x.Id == id);

            if (user.LongTermTicket == null || user.LongTermTicket.RemainingTrips == 0)
            {
                return new LongTermTicketViewModel();
            }

            return new LongTermTicketViewModel()
            {
                RemainingTrips = user.LongTermTicket.RemainingTrips,
                DaysToExpiration = (int) (user.LongTermTicket.ExpirationDateTime - DateTime.UtcNow).TotalDays
            };
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

                if (user.LongTermTicket != null && user.LongTermTicket.RemainingTrips > 0)
                {
                    user.LongTermTicket.RemainingTrips--;
                }
                else if (user.ParentUser != null)
                {
                    user.Limit -= chosenTicket.Cost;
                    Notifications.Instance.Send($"Your child spent {chosenTicket.Cost} rubles on long term ticket", user.ParentUser.Id);
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
        public PayResult Pay(PayRequest payRequest)
        {
            var parts = payRequest.Qrcode.Split('|');
            var request = new PayRequest1()
            {
                Type = (TransportType)int.Parse(parts[0]),
                Route = int.Parse(parts[1]),
                TransportNumber = parts[2]
            };

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
                        Notifications.Instance.Send("Your child is out of limit", user.ParentUser.Id);
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

                if (user.LongTermTicket != null && user.LongTermTicket.RemainingTrips > 0)
                {
                    user.LongTermTicket.RemainingTrips--;
                }
                else if (user.ParentUser != null)
                {
                    user.Limit -= Rate;
                    Notifications.Instance.Send($"Your child spent {Rate} rubles on single ticket", user.ParentUser.Id);
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
                ActiveTicket = activeTicket
            };
        }
    }

    public class Notifications
    {
        public static Notifications Instance = new Notifications();

        public void Send(string text, string tag)
        {
            Hub.SendGcmNativeNotificationAsync(JsonConvert.SerializeObject(new { text }), tag);
        }

        public NotificationHubClient Hub { get; set; }

        private Notifications()
        {
            Hub = NotificationHubClient.CreateClientFromConnectionString("Endpoint=sb://paytransns.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=jOBi9kf4c/wO9Dqa1PlJBaLshbjeoNAtRY9irtCkKfg=",
                                                                         "PayTransHub");
        }
    }

    public class PayRequest
    {
        public string Qrcode { get; set; }
    }
}
