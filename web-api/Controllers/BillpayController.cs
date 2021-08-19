using System;
using System.Text;
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
    public class BillpayController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient();

        public BillpayController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        public async Task<IActionResult> Index()
        {
            var response = await Client.GetAsync("api/billpay");
            if (!response.IsSuccessStatusCode)
                throw new Exception();
            var result = await response.Content.ReadAsStringAsync();
            var billpays = JsonConvert.DeserializeObject<List<BillPayDto>>(result);
            return View(billpays);
        }

        
        public async Task<IActionResult> LockAccount(int? id)
        {
            if (id == null)
                return NotFound();
            var response = await Client.GetAsync($"api/billpay/{id}");
            if (!response.IsSuccessStatusCode)
                throw new Exception();
            var result = await response.Content.ReadAsStringAsync();
            if (ModelState.IsValid)
            {
                var content = new StringContent(result, Encoding.UTF8, "application/json");
                var putResponse = Client.PutAsync("/api/billpay/lock", content).Result;
                if (putResponse.IsSuccessStatusCode)
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        
        public async Task<IActionResult> UnLockAccount(int? id)
        {
            if (id == null)
                return NotFound();
            var response = await Client.GetAsync($"api/billpay/{id}");
            if (!response.IsSuccessStatusCode)
                throw new Exception();
            var result = await response.Content.ReadAsStringAsync();
            if (ModelState.IsValid)
            {
                var content = new StringContent(result, Encoding.UTF8, "application/json");
                var putResponse = Client.PutAsync("/api/billpay/unlock", content).Result;
                if (putResponse.IsSuccessStatusCode)
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}