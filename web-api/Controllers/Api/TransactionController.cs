using System.Collections.Generic;
using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using a2.admin.Models;
using a2.admin.Models.DataManager;


namespace a2.admin.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly TransactionManager _repo;
        public TransactionController(TransactionManager repo)
        {
            _repo = repo;
        }
        [HttpGet("{id}")]
        public IEnumerable<Transaction> Get(int id)
        {
            return _repo.Get(id, null, null);
        }
        [HttpGet("{id}/{start}/{end}")]
        public IEnumerable<Transaction> Get(int id, string start, string end)
        {
            String format = "ddMMyyyy";
            DateTime startDate;
            DateTime endDate;

            if (DateTime.TryParseExact(start, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate) &&
            DateTime.TryParseExact(end, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
                return _repo.Get(id, startDate, endDate);
            throw new ApplicationException("Invalid date format");
        }
    }
}