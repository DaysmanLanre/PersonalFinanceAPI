using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceAPI.DataAccess;
using PersonalFinanceAPI.Dtos;
using PersonalFinanceAPI.Entities;

namespace PersonalFinanceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController(FinanceDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TransactionResponse>>> GetAll(
        int? accountId,
        int? categoryId,
        TransactionType? type,
        DateOnly? from,
        DateOnly? to)
    {
        var query = dbContext.Transactions.AsQueryable();

        if (accountId is not null)
            query = query.Where(transaction => transaction.AccountId == accountId);

        if (categoryId is not null)
            query = query.Where(transaction => transaction.CategoryId == categoryId);

        if (type is not null)
            query = query.Where(transaction => transaction.Type == type);

        if (from is not null)
            query = query.Where(transaction => transaction.Date >= from);

        if (to is not null)
            query = query.Where(transaction => transaction.Date <= to);

        var transactions = await query
            .OrderByDescending(transaction => transaction.Date)
            .Select(transaction => new TransactionResponse
            {
                Id = transaction.Id,
                Description = transaction.Description,
                Amount = transaction.Amount,
                Type = transaction.Type,
                Date = transaction.Date,
                AccountId = transaction.AccountId,
                AccountName = transaction.Account!.Name,
                CategoryId = transaction.CategoryId,
                CategoryName = transaction.Category!.Name
            })
            .ToListAsync();

        return Ok(transactions);
    }

    [HttpGet("summary")]
    public async Task<ActionResult<List<CategorySpendingResponse>>> GetMonthlySummary(int year, int month)
    {
        var start = new DateOnly(year, month, 1);
        var end = start.AddMonths(1);

        var summary = await dbContext.Transactions
            .Where(t => t.Type == TransactionType.Expense && t.Date >= start && t.Date < end)
            .GroupBy(t => new { t.CategoryId, t.Category!.Name })
            .Select(group => new CategorySpendingResponse
            {
                CategoryId = group.Key.CategoryId,
                CategoryName = group.Key.Name,
                TotalSpent = group.Sum(t => t.Amount)
            })
            .OrderByDescending(item => item.TotalSpent)
            .ToListAsync();

        return Ok(summary);
    }

    [HttpGet("{id:int}", Name = "GetTransactionById")]
    public async Task<ActionResult<TransactionResponse>> GetById(int id)
    {
        var transaction = await dbContext.Transactions
            .Include(transaction => transaction.Account)
            .Include(transaction => transaction.Category)
            .FirstOrDefaultAsync(transaction => transaction.Id == id);

        if (transaction is null)
        {
            return Problem(
                detail: $"A transaction with ID {id} was not found.",
                statusCode: StatusCodes.Status404NotFound);
        }

        return Ok(ToResponse(transaction));
    }

    [HttpPost]
    public async Task<ActionResult<TransactionResponse>> Create(CreateTransactionRequest request)
    {
        var error = await ValidateAsync(
            request.Description, request.Amount, request.AccountId, request.CategoryId);

        if (error is not null)
            return error;

        var transaction = new Transaction
        {
            Description = request.Description,
            Amount = request.Amount,
            Type = request.Type,
            Date = request.Date,
            AccountId = request.AccountId,
            CategoryId = request.CategoryId
        };

        dbContext.Transactions.Add(transaction);
        await dbContext.SaveChangesAsync();

        await dbContext.Entry(transaction).Reference(t => t.Account).LoadAsync();
        await dbContext.Entry(transaction).Reference(t => t.Category).LoadAsync();

        return CreatedAtRoute(
            "GetTransactionById",
            new { id = transaction.Id },
            ToResponse(transaction));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TransactionResponse>> Update(int id, UpdateTransactionRequest request)
    {
        var transaction = await dbContext.Transactions.FindAsync(id);

        if (transaction is null)
        {
            return Problem(
                detail: $"A transaction with ID {id} was not found.",
                statusCode: StatusCodes.Status404NotFound);
        }

        var error = await ValidateAsync(
            request.Description, request.Amount, request.AccountId, request.CategoryId);

        if (error is not null)
            return error;

        transaction.Description = request.Description;
        transaction.Amount = request.Amount;
        transaction.Type = request.Type;
        transaction.Date = request.Date;
        transaction.AccountId = request.AccountId;
        transaction.CategoryId = request.CategoryId;

        await dbContext.SaveChangesAsync();

        await dbContext.Entry(transaction).Reference(transaction => transaction.Account).LoadAsync();
        await dbContext.Entry(transaction).Reference(transaction => transaction.Category).LoadAsync();

        return Ok(ToResponse(transaction));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var transaction = await dbContext.Transactions.FindAsync(id);

        if (transaction is null)
        {
            return Problem(
                detail: $"A transaction with ID {id} was not found.",
                statusCode: StatusCodes.Status404NotFound);
        }

        dbContext.Transactions.Remove(transaction);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    private async Task<ActionResult?> ValidateAsync(
        string description, decimal amount, int accountId, int categoryId)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return Problem(
                detail: "Description cannot be empty.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        if (amount <= 0)
        {
            return Problem(
                detail: "Amount must be greater than 0.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        if (!await dbContext.Accounts.AnyAsync(account => account.Id == accountId))
        {
            return Problem(
                detail: $"Account with ID {accountId} does not exist.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        if (!await dbContext.Categories.AnyAsync(category => category.Id == categoryId))
        {
            return Problem(
                detail: $"Category with ID {categoryId} does not exist.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        return null;
    }

    private static TransactionResponse ToResponse(Transaction transaction) => new()
    {
        Id = transaction.Id,
        Description = transaction.Description,
        Amount = transaction.Amount,
        Type = transaction.Type,
        Date = transaction.Date,
        AccountId = transaction.AccountId,
        AccountName = transaction.Account!.Name,
        CategoryId = transaction.CategoryId,
        CategoryName = transaction.Category!.Name
    };
}