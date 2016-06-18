using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PayTransApi.Models;

namespace PayTransApi.Controllers
{
//    [Authorize]
    [Route("Account")]
    public class ApiAccountController : ApiController
    {
        [HttpGet]
        public ApplicationUser[] LinkedAccounts()
        {
            return new ApplicationDbContext().Users.ToArray();
        }
    }
}
