using System;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using System.Dynamic;
using a2.Data;
using a2.Models;
using a2.Filters;

namespace a2.Controllers
{
    [AuthorizeCustomer]
    public class BillpayController : Controller
    {
        private readonly McbaContext _context;
        private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

        public BillpayController(McbaContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var accountsList = await _context.Accounts.Where(a => a.CustomerID == CustomerID).ToListAsync();
            List<BillPay> billpayEntries = new List<BillPay>();
            foreach (var account in accountsList)
            {
                var entries = await _context.BillPays.Where(b => b.AccountNumber == account.AccountNumber).ToListAsync();
                billpayEntries.AddRange(entries);
            }
            return View(billpayEntries);
        }

        public async Task<IActionResult> Delete(int ID)
        {
            var billpay = await _context.BillPays.FindAsync(ID);
            if (billpay == null)
            {
                return NotFound();
            }
            _context.BillPays.Remove(billpay);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int ID)
        {
            var billpay = await _context.BillPays.FindAsync(ID);
            if (billpay == null) { return NotFound(); }
            ViewBag.Amount = billpay.Amount;
            return View(billpay);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int ID, string date, string time, decimal amount)
        {
            var billpay = await _context.BillPays.FindAsync(ID);
            if (billpay == null || amount <= 0) { return NotFound(); }
            DateTime scheduledTime;
            if (!DateTime.TryParse(date + time, out scheduledTime)) { return View(NotFound()); }
            if (scheduledTime < DateTime.Now) { return View(NotFound()); }
            billpay.ScheduledTimeUtc = scheduledTime.ToUniversalTime();
            billpay.Amount = amount;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AddPayee()
        {   
            var customer = await _context.Customers.FindAsync(CustomerID);
            ViewBag.customerID = CustomerID;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add([Bind("PayeeID,Name,Address,Suburb,State,PostCode,Phone,CustomerID")] Payee payee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(payee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(payee);
        }

        public async Task<IActionResult> AddPayment()
        {
            var payeeList = await _context.Payee.Where(p => p.CustomerID == CustomerID).ToListAsync();
            var accountList = new List<AccountViewModel>();
            var customer = await _context.Customers.FindAsync(CustomerID);
            foreach (var account in customer.Accounts)
            {
                accountList.Add(new AccountViewModel
                {
                    AccountNumber = account.AccountNumber,
                    AccountType = account.AccountType,
                    Balance = GetBalance(account),
                });
            }

            dynamic model = new ExpandoObject();
            model.Payees = payeeList;
            model.Accounts = accountList;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddPayment(int payeeID, Decimal amount, Char period, string date, string time, int accountID)
        {
            var payeeList = await _context.Payee.Where(p => p.CustomerID == CustomerID).ToListAsync();
            var account = await _context.Accounts.FindAsync(accountID);

            if (payeeID <= 0 || amount <= 0 || account == null)
            { return View(NotFound()); }

            if (!period.Equals(Period.OneOff) && !period.Equals(Period.Annually) && !period.Equals(Period.Quarterly) && !period.Equals(Period.Monthly))
            { return View(NotFound()); }

            DateTime scheduledTime;
            if (!DateTime.TryParse(date + time, out scheduledTime)) { return View(NotFound()); }

            if (scheduledTime < DateTime.Now) { return View(NotFound()); }
            var payee = await _context.Payee.FindAsync(payeeID);
            if (payee == null) { return View(NotFound()); }

            _context.BillPays.AddRange(
                new BillPay
                {
                    AccountNumber = accountID,
                    PayeeID = payeeID,
                    Amount = amount,
                    ScheduledTimeUtc = scheduledTime.ToUniversalTime(),
                    Period = period,
                    Failed = false,
                    PaymentLocked = false,
                });
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private decimal GetBalance(Account account)
        {
            var transactions = _context.Transactions.Where(t => t.AccountNumber == account.AccountNumber).ToList();
            var depositList = transactions
                .Where(t => t.TransactionType == TransactionType.Deposit ||
                t.TransactionType == TransactionType.Transfer && t.DestinationAccountNumber == null).ToList();
            var withdrawalList = transactions
                .Where(t => t.TransactionType == TransactionType.Withdraw || t.TransactionType == TransactionType.ServiceCharge
                || t.TransactionType == TransactionType.BillPay)
                .ToList();
            decimal deposits = 0;
            decimal withdrawals = 0;
            foreach (var transaction in depositList) { deposits += transaction.Amount; }
            foreach (var transaction in withdrawalList) { withdrawals += transaction.Amount; }

            return deposits - withdrawals;
        }
    }
}