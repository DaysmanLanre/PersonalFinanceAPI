namespace PersonalFinanceAPI.Dtos;

public class AccountBalanceResponse
{
    public int AccountId { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public decimal OpeningBalance { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Balance => OpeningBalance + TotalIncome - TotalExpenses;
}