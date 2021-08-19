using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using a2.admin.Models;
using a2.admin.Models.DataManager;

namespace a2.admin.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerManager _repo;

        public CustomerController(CustomerManager repo)
        {
            _repo = repo;
        }
        [HttpGet]
        public IEnumerable<Customer> Get()
        {
            return _repo.GetAll();
        }
        
        [HttpGet("{id}")]
        public Customer Get(int id)
        {   
            return _repo.Get(id);
        }
        
        [HttpPost("{id}/lock")]
        public int Lock(int id)
        {
            return _repo.Lock(id);
        }
        [HttpPost("{id}/unlock")]
        public int UnLock(int id)
        {
            return _repo.UnLock(id);
        }
    }
}