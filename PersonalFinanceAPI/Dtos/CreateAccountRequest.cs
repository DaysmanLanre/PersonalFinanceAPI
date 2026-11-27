// Dtos/CreateAccountRequest.cs
namespace PersonalFinanceAPI.Dtos;

using System.ComponentModel.DataAnnotations;

public class CreateAccountRequest
{
    
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public decimal OpeningBalance { get; set; }
}