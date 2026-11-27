// Dtos/CreateTransactionRequest.cs
using PersonalFinanceAPI.Entities;

namespace PersonalFinanceAPI.Dtos;

public class CreateTransactionRequest
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public DateOnly Date { get; set; }
    public int AccountId { get; set; }
    public int CategoryId { get; set; }
}