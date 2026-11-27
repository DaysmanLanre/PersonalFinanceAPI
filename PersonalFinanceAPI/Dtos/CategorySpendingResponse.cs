namespace PersonalFinanceAPI.Dtos;

public class CategorySpendingResponse
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal TotalSpent { get; set; }
}