using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using a2.admin.Models;
using a2.admin.Filters;

namespace a2.admin.Controllers
{
    [AuthorizeAdmin]
    public class CustomerController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient();

        public CustomerController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        // GET: customer/index
        public async Task<IActionResult> Index()
        {
            var response = await Client.GetAsync("api/customer");
            if(!response.IsSuccessStatusCode)
                throw new Exception();
            var result = await response.Content.ReadAsStringAsync();
            var customers = JsonConvert.DeserializeObject<List<CustomerDto>>(result);
            return View(customers);
        }
    }
}
