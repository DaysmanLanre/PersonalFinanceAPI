// Dtos/CategoryRequest.cs
using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceAPI.Dtos;

public class CategoryRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}