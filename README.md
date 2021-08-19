# C# Simple Bank App

## How to run

### Customer Site
Located under the a2 directory. In the CLI run `dotnet run` server will be available on http://localhost:5000

### Admin Portal
Located under the a2-admin-mvc-api directory. In the CLI run `dotnet run` server will be available on http://localhost:5000
Login with username: 'admin' password: 'admin'. Please note the Customer site and Admin portal are not designed to be run at the same time.

### MS SQL Database

Create and add migrations:

`dotnet ef migrations add <migrationName>`

`dotnet ef database update`

Add session cache: 

`dotnet sql-cache create "server=127.0.0.1;uid=sa;database=Bank;pwd=Admin123!" dbo SessionCache`

To completely remove all migrations and start over:

`dotnet ef database update 0`
`dotnet ef migrations remove`

Docker container for local dev

`docker run --name mssql -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Admin123!" -p 1433:1433 -d mcr.microsoft.com/mssql/server`

Connect and create a db to use

`docker exec -it mssql /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Admin123!`

``CREATE DATABASE Bank;
GO
USE Bank;
GO``

Connection string should look somthing like this:

`"McbaContext": "server=127.0.0.1;uid=sa;database=Bank;pwd=Admin123!"`

## API Documentation

### Customer API 
Controller path: api/customer

Routes

GET: `api/customer` - Returns all customers

GET: `api/customer/{customer ID}` - Returns customer with the passed in ID

NOTE: The two below not fully implemented.

POST: `api/customer/lock` - Locks the customers account

POST: `api/customer/unlock` - unlocks the customers account

### Transaction API
Controller path: api/transaction

Routes

GET: `api/customer/{AccountNumber}` - Returns all transctions for account

GET: `api/customer/{AccountNumber}/{start}/{end}` - Returns transactions for an account within a data range. 

NOTE: date format must be ddMMyyyy. For example: 22082021.

### BillPay API
Controller path: api/billpay

Routes

GET: `api/billpay` - Gets all billpay entries.

NOTE: The two below not fully implemented.

POST: `api/billpay/lock` - Locks the specified billpay.

POST: `api/billpay/unlock` - Unlocks the specified billpay.

### Known Bugs

When scheduling a BillPay the time (not date, time only) cannot be set prior to current time.
Throws a model binding error, may be related to my use of dynamic object. Needs to be investigated.