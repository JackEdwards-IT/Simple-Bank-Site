using System.Collections.Generic;
using System.Linq;
using a2.admin.Data;
using a2.admin.Models.Repository;
using System;

namespace a2.admin.Models.DataManager
{
    public class BillPayManager
    {
        private readonly McbaContext _context;
        public BillPayManager(McbaContext context)
        {
            _context = context;
        }

        public BillPay Get(int id)
        {
            return _context.BillPays.Find(id);
        }
        public IEnumerable<BillPay> GetAll()
        {
            return _context.BillPays.ToList();
        }

        public Boolean LockBillPay(int id)
        {
            var billpay = _context.BillPays.Find(id);
            if (billpay == null) { return false; }
            billpay.PaymentLocked = true;
            _context.SaveChanges();
            return true;
        }

        public Boolean UnlockBillPay(int id)
        {
            var billpay = _context.BillPays.Find(id);
            if (billpay == null) { return false; }
            billpay.PaymentLocked = false;
            _context.SaveChanges();
            return true;
        }
    }
}