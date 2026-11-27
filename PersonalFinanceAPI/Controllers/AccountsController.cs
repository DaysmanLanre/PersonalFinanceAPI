using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceAPI.DataAccess;
using PersonalFinanceAPI.Dtos;
using PersonalFinanceAPI.Entities;

namespace PersonalFinanceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController(FinanceDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AccountResponse>>> GetAll()
    {
        var accounts = await dbContext.Accounts
            .Select(account => new AccountResponse
            {
                Id = account.Id,
                Name = account.Name,
                OpeningBalance = account.OpeningBalance
            })
            .ToListAsync();

        return Ok(accounts);
    }

    [HttpGet("{id:int}", Name = "GetAccountById")]
    public async Task<ActionResult<AccountResponse>> GetById(int id)
    {
        var account = await dbContext.Accounts.FindAsync(id);

        if (account is null)
        {
            return Problem(
                detail: $"An account with ID {id} was not found.",
                statusCode: StatusCodes.Status404NotFound);
        }

        return Ok(new AccountResponse
        {
            Id = account.Id,
            Name = account.Name,
            OpeningBalance = account.OpeningBalance
        });
    }

    [HttpPost]
    public async Task<ActionResult<AccountResponse>> Create(CreateAccountRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Problem(
                detail: "Account name cannot be empty.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var account = new Account
        {
            Name = request.Name,
            OpeningBalance = request.OpeningBalance
        };

        dbContext.Accounts.Add(account);
        await dbContext.SaveChangesAsync();

        var response = new AccountResponse
        {
            Id = account.Id,
            Name = account.Name,
            OpeningBalance = account.OpeningBalance
        };

        return CreatedAtRoute("GetAccountById", new { id = account.Id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<AccountResponse>> Update(int id, UpdateAccountRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Problem(
                detail: "Account name cannot be empty.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var account = await dbContext.Accounts.FindAsync(id);

        if (account is null)
        {
            return Problem(
                detail: $"An account with ID {id} was not found.",
                statusCode: StatusCodes.Status404NotFound);
        }

        account.Name = request.Name;
        account.OpeningBalance = request.OpeningBalance;
        await dbContext.SaveChangesAsync();

        return Ok(new AccountResponse
        {
            Id = account.Id,
            Name = account.Name,
            OpeningBalance = account.OpeningBalance
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var account = await dbContext.Accounts.FindAsync(id);

        if (account is null)
        {
            return Problem(
                detail: $"An account with ID {id} was not found.",
                statusCode: StatusCodes.Status404NotFound);
        }

        var hasTransactions = await dbContext.Transactions
            .AnyAsync(transaction => transaction.AccountId == id);

        if (hasTransactions)
        {
            return Problem(
                detail: "This account has transactions and cannot be deleted.",
                statusCode: StatusCodes.Status409Conflict);
        }

        dbContext.Accounts.Remove(account);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("balances")]
    public async Task<ActionResult<List<AccountBalanceResponse>>> GetBalances()
    {
        var balances = await dbContext.Accounts
            .Select(account => new AccountBalanceResponse
            {
                AccountId = account.Id,
                AccountName = account.Name,
                OpeningBalance = account.OpeningBalance,
                TotalIncome = account.Transactions
                    .Where(t => t.Type == TransactionType.Income)
                    .Sum(t => (decimal?)t.Amount) ?? 0,
                TotalExpenses = account.Transactions
                    .Where(t => t.Type == TransactionType.Expense)
                    .Sum(t => (decimal?)t.Amount) ?? 0
            })
            .ToListAsync();

        return Ok(balances);
    }
}