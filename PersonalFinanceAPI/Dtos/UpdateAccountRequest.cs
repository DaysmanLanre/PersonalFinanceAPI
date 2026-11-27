// Dtos/UpdateAccountRequest.cs
namespace PersonalFinanceAPI.Dtos;

public class UpdateAccountRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal OpeningBalance { get; set; }
}