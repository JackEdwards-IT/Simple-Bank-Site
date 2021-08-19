using System;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using a2.Data;
using a2.Models;
using a2.Utilities;
using a2.Filters;
using System.Linq;
using System.Collections.Generic;
using System.Dynamic;
using X.PagedList;

namespace a2.Controllers
{
    [AuthorizeCustomer]
    public class CustomerController : Controller
    {
        private readonly McbaContext _context;
        private readonly decimal _ATMfee = 0.10m;
        private readonly decimal _TransferFee = 0.20m;
        private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;
        public CustomerController(McbaContext context) => _context = context;
        public async Task<IActionResult> Index()
        {
            var customer = await _context.Customers.FindAsync(CustomerID);

            var accountList = new List<AccountViewModel>();
            foreach (var account in customer.Accounts)
            {
                accountList.Add(new AccountViewModel
                {
                    AccountNumber = account.AccountNumber,
                    AccountType = account.AccountType,
                    Balance = GetBalance(account),
                });
            }
            return View(accountList);
        }

        public async Task<IActionResult> Deposit(int id) => View(await _context.Accounts.FindAsync(id));
        public async Task<IActionResult> DepositPage()
        {
            var customer = await _context.Customers.FindAsync(CustomerID);
            var accountList = new List<AccountViewModel>();
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
            model.Customer = customer;
            model.Accounts = accountList;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Deposit(int id, decimal amount, string comment)
        {
            if (id <= 0)
            {
                return NotFound();
            }
            var account = await _context.Accounts.FindAsync(id);

            if (amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            if (amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
            if (!ModelState.IsValid)
            {
                ViewBag.Amount = amount;
                return View(account);
            }

            account.Transactions.Add(
                new Transaction
                {
                    Comment = comment,
                    TransactionType = TransactionType.Deposit,
                    Amount = amount,
                    TransactionTimeUtc = DateTime.UtcNow
                });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Withdraw()
        {
            var customer = await _context.Customers.FindAsync(CustomerID);
            var accountList = new List<AccountViewModel>();
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
            model.Customer = customer;
            model.Accounts = accountList;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Withdraw(int id, decimal amount, string comment)
        {
            if (id <= 0)
            {
                return NotFound();
            }
            var account = await _context.Accounts.FindAsync(id);
            // Check if any free transactions allowed
            var transactions = await _context.Transactions.Where(t => t.AccountNumber == account.AccountNumber).ToListAsync();
            int transactionCount = transactions.Where(t => t.TransactionType == TransactionType.Transfer || t.TransactionType == TransactionType.Withdraw).Count();

            if (amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            if (amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
            var minBalance = 0;
            if (account.AccountType == AccountType.Checking)
                minBalance = 200;
            if (GetBalance(account) - amount < minBalance)
                ModelState.AddModelError(nameof(amount), "Withdrawal must not exceed avaialable funds");
            if (!ModelState.IsValid)
            {
                ViewBag.Amount = amount;
                return View(account);
            }
            decimal fee = 0;
            if (transactionCount >= 4)
            {
                account.Transactions.Add(
                    new Transaction
                    {
                        TransactionType = TransactionType.ServiceCharge,
                        Amount = _ATMfee,
                        TransactionTimeUtc = DateTime.UtcNow
                    });
                fee = _ATMfee;
            }
            account.Transactions.Add(
                new Transaction
                {
                    Comment = comment,
                    TransactionType = TransactionType.Withdraw,
                    Amount = amount,
                    TransactionTimeUtc = DateTime.UtcNow
                });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Transfer()
        {
            var customer = await _context.Customers.FindAsync(CustomerID);
            var accountList = new List<AccountViewModel>();
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
            model.Customer = customer;
            model.Accounts = accountList;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Transfer(int idFrom, int idTo, decimal amount, string comment)
        {
            if (idTo <= 0 || idFrom <= 0 || idTo == idFrom)
            {
                return NotFound();
            }
            var account = await _context.Accounts.FindAsync(idFrom);
            var accountTo = await _context.Accounts.FindAsync(idTo);

            if (accountTo == null)
            {
                return NotFound();
            }
            // Check if any free transactions allowed
            var transactions = await _context.Transactions.Where(t => t.AccountNumber == account.AccountNumber).ToListAsync();
            int transactionCount = transactions.Where(t => t.TransactionType == TransactionType.Transfer || t.TransactionType == TransactionType.Withdraw).Count();
            decimal fee = 0;
            if (transactionCount >= 4)
            {
                account.Transactions.Add(
                    new Transaction
                    {
                        TransactionType = TransactionType.ServiceCharge,
                        Amount = _TransferFee,
                        TransactionTimeUtc = DateTime.UtcNow
                    });
                fee = _ATMfee;
            }
            account.Transactions.Add(
                new Transaction
                {
                    Comment = comment,
                    TransactionType = TransactionType.Transfer,
                    Amount = amount,
                    DestinationAccountNumber = idTo,
                    TransactionTimeUtc = DateTime.UtcNow
                });
            accountTo.Transactions.Add(
                new Transaction
                {
                    Comment = comment,
                    TransactionType = TransactionType.Transfer,
                    Amount = amount,
                    TransactionTimeUtc = DateTime.UtcNow
                });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Profile()
        {
            var customer = await _context.Customers.FindAsync(CustomerID);
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(int customerID, string name, string tfn, string address, string suburb,
        string state, string postCode, string mobile)
        {
            if (ModelState.IsValid)
            {
                var customer = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.CustomerID == customerID);
                customer = customer with
                {
                    CustomerID = customerID,
                    Name = name,
                    Tfn = tfn,
                    Address = address,
                    Suburb = suburb,
                    State = state,
                    PostCode = postCode,
                    Mobile = mobile
                };
                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Statements()
        {
            var customer = await _context.Customers.FindAsync(CustomerID);
            return View(customer);
        }

        public async Task<PartialViewResult> GetStatements(int accountNumber, int page)
        {
            ViewBag.accountNumber = accountNumber;

            int pageSize = 4;
            if (page < 0) page = 1;
            if (accountNumber <= 0) return PartialView("_GetStatements");
            var transactions = await _context.Transactions.Where(t => t.AccountNumber == accountNumber)
                        .OrderBy(t => t.TransactionTimeUtc).ToPagedListAsync(page, pageSize);
            return PartialView("_GetStatements", transactions);
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
