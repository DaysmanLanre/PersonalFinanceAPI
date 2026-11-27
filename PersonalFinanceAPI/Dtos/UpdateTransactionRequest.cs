// Dtos/UpdateTransactionRequest.cs
using PersonalFinanceAPI.Entities;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceAPI.Dtos;

public class UpdateTransactionRequest
{
    
    [Required, MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, 1_000_000_000)]
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public DateOnly Date { get; set; }
    public int AccountId { get; set; }
    public int CategoryId { get; set; }
}

