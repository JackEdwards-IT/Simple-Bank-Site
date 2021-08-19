using System.Collections.Generic;
using System;
using System.Linq;
using a2.admin.Data;
using a2.admin.Models.Repository;

namespace a2.admin.Models.DataManager
{
    public class TransactionManager
    {
        private readonly McbaContext _context;
        public TransactionManager(McbaContext context)
        {
            _context = context;
        }
        public IEnumerable<Transaction> Get(int accountNumber, DateTime? start, DateTime? end)
        {
            var transactions = _context.Transactions.Where(t => t.AccountNumber == accountNumber).ToList();
            if (start == null && end == null)
            {
                return transactions;
            }
            return transactions.Where(t => t.TransactionTimeUtc >= start && t.TransactionTimeUtc <= end).ToList();
        }
    }
}

