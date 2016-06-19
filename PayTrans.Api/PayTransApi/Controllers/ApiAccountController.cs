using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using PayTransApi.Models;

namespace PayTransApi.Controllers
{
    [Authorize]
    public class ApiAccountController : ApiController
    {
        [HttpGet]
        public int GetLimit()
        {
            var id = User.Identity.GetUserId();
            var user = new ApplicationDbContext().Users.FirstOrDefault(x => x.Id == id);
            return user.Limit;
        }

        [HttpGet]
        public LinkedAccountModel[] LinkedAccounts()
        {
            var id = User.Identity.GetUserId();

            var linkedAccounts =
                new ApplicationDbContext().Users.FirstOrDefault(x => x.Id == id).LinkedAccounts.ToList();

            return linkedAccounts.Select(u => new LinkedAccountModel()
                    {
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Id = u.Id,
                        AvatarUrl = u.AvatarUrl,
                        Limit = u.Limit
                    }).ToArray();
        }

        [HttpGet]
        public AccountSettings GetSettings()
        {
            var id = User.Identity.GetUserId();
            var user = new ApplicationDbContext().Users.FirstOrDefault(x => x.Id == id);
            return user.AccountSettings;
        }

        [HttpPost]
        public object ChangeLimit(ChangeLimitRequest request)
        {
            var id = User.Identity.GetUserId();
            using (var context = new ApplicationDbContext())
            {
                var linkedAccount =
                    context.Users.FirstOrDefault(x => x.Id == id).LinkedAccounts.FirstOrDefault(x => x.Id == request.ChildId);
                linkedAccount.Limit = request.NewLimit;
                context.SaveChanges();
            }
            return new object();
        }
    }
}
