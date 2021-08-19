using System.Collections.Generic;
using System.Linq;
using a2.admin.Data;
using a2.admin.Models.Repository;

namespace a2.admin.Models.DataManager
{
    public class CustomerManager : IDataRepository<Customer, int>
    {
        private readonly McbaContext _context;
        public CustomerManager(McbaContext context)
        {
            _context = context;
        }
        public Customer Get(int id)
        {
            return _context.Customers.Find(id);
        }
        public IEnumerable<Customer> GetAll()
        {
            return _context.Customers.ToList();
        }
        public int Add(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
            return customer.CustomerID;
        }
        public int Delete(int id)
        {
            _context.Customers.Remove(_context.Customers.Find(id));
            _context.SaveChanges();
            return id;
        }
        public int Update(int id, Customer customer)
        {
            _context.Update(customer);
            _context.SaveChanges();
            return id;
        }

        public int Lock(int id)
        {
            var customer = _context.Customers.Find(id);
            customer.AccountLocked = true;
            _context.SaveChanges();
            return id;
        }
        
        public int UnLock(int id)
        {
            var customer = _context.Customers.Find(id);
            customer.AccountLocked = false;
            _context.SaveChanges();
            return id;
        }
    }
}