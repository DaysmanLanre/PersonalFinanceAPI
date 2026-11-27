// Dtos/AccountResponse.cs
namespace PersonalFinanceAPI.Dtos;

public class AccountResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal OpeningBalance { get; set; }
}