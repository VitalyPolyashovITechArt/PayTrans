using System.Web.Http;

namespace PayTransApi.Controllers
{
    public class TestController : ApiController
    {
        [HttpGet]
        public void T()
        {
            Notifications.Instance.Send("hello", "0762bcd4-66c8-4978-8d97-e6663a139a78");
        }
    }
}
