// Dtos/CreateAccountRequest.cs
namespace PersonalFinanceAPI.Dtos;

public class CreateAccountRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal OpeningBalance { get; set; }
}