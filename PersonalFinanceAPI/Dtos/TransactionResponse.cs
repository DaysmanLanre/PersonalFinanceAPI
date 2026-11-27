// Dtos/TransactionResponse.cs
using PersonalFinanceAPI.Entities;

namespace PersonalFinanceAPI.Dtos;

public class TransactionResponse
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public DateOnly Date { get; set; }
    public int AccountId { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}