using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using a2.admin.Models;
using a2.admin.Models.DataManager;
using System;

namespace a2.admin.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillpayController : ControllerBase
    {
        private readonly BillPayManager _repo;
        public BillpayController(BillPayManager repo)
        {
            _repo = repo;
        }

        [HttpGet("{id}")]
        public BillPay GetId(int id)
        {
            return _repo.Get(id);
        }
        [HttpGet]
        public IEnumerable<BillPay> Get()
        {
            return _repo.GetAll();
        }
        [HttpPut("/lock")]
        public Boolean Lock([FromBody] BillPay billpay)
        {
            Console.WriteLine("\nLock Billpay Controller ID: " + billpay.BillPayID + "\n");
            return _repo.LockBillPay(billpay.BillPayID);
        }
        [HttpPut("/unlock")]
        public Boolean UnLock([FromBody] BillPay billpay)
        {
            return _repo.UnlockBillPay(billpay.BillPayID);
        }
    }
}