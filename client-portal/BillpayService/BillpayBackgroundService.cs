using System;
using System.Threading;
using System.Threading.Tasks;
using a2.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;
using a2.Models;

namespace a2.Services
{
    public class BillpayService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<BillpayService> _logger;

        public BillpayService(IServiceProvider services, ILogger<BillpayService> logger)
        {
            _services = services;
            _logger = logger;

        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Billpay background service is running.");
            while (!cancellationToken.IsCancellationRequested)
            {
                await ProcessSchedule(cancellationToken);
                _logger.LogInformation("Billpay service is awaiting next schedule Tick");
                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }

        private async Task ProcessSchedule(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Billpay service is working.");
            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<McbaContext>();

            var billpayEntries = await context.BillPays.ToListAsync(cancellationToken);
            foreach (var billpay in billpayEntries)
            {
                if (billpay.ScheduledTimeUtc <= (DateTime.Now).ToUniversalTime() && !billpay.Failed && !billpay.PaymentLocked)
                {
                    ProcessPayment(billpay.BillPayID);
                }
            }

            _logger.LogInformation("Billpay service work complete.");
        }

        private async void ProcessPayment(int id)
        {
            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<McbaContext>();
            var billpay = await context.BillPays.FindAsync(id);
            var account = await context.Accounts.FindAsync(billpay.AccountNumber);
            var minBalance = 0;
            if (account.AccountType == AccountType.Checking)
                minBalance = 200;
            if (GetBalance(account) - billpay.Amount < minBalance)
            {
                billpay.Failed = true;
                await context.SaveChangesAsync();
                _logger.LogInformation("BillPay failed, insufficent funds");
                return;
            }

            switch (billpay.Period)
            {
                case 'O':
                    // One Off 
                    addTransaction(id);
                    deleteBillpay(id);
                    break;
                case 'M':
                    // Monthly
                    addTransaction(id);
                    addMonths(id, 1);
                    break;
                case 'Q':
                    // Quarterly 
                    addTransaction(id);
                    addMonths(id, 3);
                    break;
                case 'A':
                    // Annually
                    addTransaction(id);
                    addMonths(id, 12);
                    break;
            }
        }

        private async void addTransaction(int id)
        {
            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<McbaContext>();
            var billpay = await context.BillPays.FindAsync(id);
            var account = await context.Accounts.FindAsync(billpay.AccountNumber);
            account.Transactions.Add(
                new Transaction
                {
                    TransactionType = TransactionType.BillPay,
                    Amount = billpay.Amount,
                    Comment = "BillPay to Payee ID: " + billpay.PayeeID,
                    TransactionTimeUtc = DateTime.UtcNow,
                });
            await context.SaveChangesAsync();
            _logger.LogInformation("Billpay ID: " + billpay.BillPayID + " transaction processed.");
        }
        private async void deleteBillpay(int id)
        {
            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<McbaContext>();
            var billpay = await context.BillPays.FindAsync(id);
            context.BillPays.Remove(billpay);
            await context.SaveChangesAsync();
            _logger.LogInformation("Billpay ID: " + billpay.BillPayID + " removed.");
        }
        private async void addMonths(int id, int months)
        {
            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<McbaContext>();
            var billpay = await context.BillPays.FindAsync(id);
            billpay.ScheduledTimeUtc = billpay.ScheduledTimeUtc.AddMonths(months);
            await context.SaveChangesAsync();
            _logger.LogInformation("Billpay ID: " + billpay.BillPayID + " next payment date incremented by " + months + " months.");
        }
        private decimal GetBalance(Account account)
        {
            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<McbaContext>();
            var transactions = context.Transactions.Where(t => t.AccountNumber == account.AccountNumber).ToList();
            var depositList = transactions
                .Where(t => t.TransactionType == TransactionType.Deposit ||
                t.TransactionType == TransactionType.Transfer && t.DestinationAccountNumber == null).ToList();
            var withdrawalList = transactions
                .Where(t => t.TransactionType == TransactionType.Withdraw || t.TransactionType == TransactionType.ServiceCharge)
                .ToList();
            decimal deposits = 0;
            decimal withdrawals = 0;
            foreach (var transaction in depositList) { deposits += transaction.Amount; }
            foreach (var transaction in withdrawalList) { withdrawals += transaction.Amount; }

            return deposits - withdrawals;
        }
    }
}