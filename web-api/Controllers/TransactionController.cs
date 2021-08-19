using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using a2.admin.Filters;

namespace a2.admin.Controllers
{
    [AuthorizeAdmin]
    public class TransactionController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient();

        public TransactionController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;
        public IActionResult Index() => View();

    }
}
