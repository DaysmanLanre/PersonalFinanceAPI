// Dtos/UpdateAccountRequest.cs
using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceAPI.Dtos;

public class UpdateAccountRequest
{
   
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public decimal OpeningBalance { get; set; }
}