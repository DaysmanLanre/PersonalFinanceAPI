using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceAPI.DataAccess;
using PersonalFinanceAPI.Dtos;
using PersonalFinanceAPI.Entities;

namespace PersonalFinanceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(FinanceDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CategoryResponse>>> GetAll()
    {
        var categories = await dbContext.Categories
            .OrderBy(category => category.Name)
            .Select(category => new CategoryResponse { Id = category.Id, Name = category.Name })
            .ToListAsync();

        return Ok(categories);
    }

    [HttpGet("{id:int}", Name = "GetCategoryById")]
    public async Task<ActionResult<CategoryResponse>> GetById(int id)
    {
        var category = await dbContext.Categories.FindAsync(id);

        if (category is null)
        {
            return Problem(
                detail: $"A category with ID {id} was not found.",
                statusCode: StatusCodes.Status404NotFound);
        }

        return Ok(new CategoryResponse { Id = category.Id, Name = category.Name });
    }

    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> Create(CategoryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Problem(
                detail: "Category name cannot be empty.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var nameExists = await dbContext.Categories
            .AnyAsync(category => category.Name == request.Name);

        if (nameExists)
        {
            return Problem(
                detail: $"A category named {request.Name} already exists.",
                statusCode: StatusCodes.Status409Conflict);
        }

        var category = new Category { Name = request.Name };
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        var response = new CategoryResponse { Id = category.Id, Name = category.Name };
        return CreatedAtRoute("GetCategoryById", new { id = category.Id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryResponse>> Update(int id, CategoryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Problem(
                detail: "Category name cannot be empty.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var category = await dbContext.Categories.FindAsync(id);

        if (category is null)
        {
            return Problem(
                detail: $"A category with ID {id} was not found.",
                statusCode: StatusCodes.Status404NotFound);
        }

        var nameExists = await dbContext.Categories
            .AnyAsync(existing => existing.Id != id && existing.Name == request.Name);

        if (nameExists)
        {
            return Problem(
                detail: $"A category named {request.Name} already exists.",
                statusCode: StatusCodes.Status409Conflict);
        }

        category.Name = request.Name;
        await dbContext.SaveChangesAsync();

        return Ok(new CategoryResponse { Id = category.Id, Name = category.Name });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await dbContext.Categories.FindAsync(id);

        if (category is null)
        {
            return Problem(
                detail: $"A category with ID {id} was not found.",
                statusCode: StatusCodes.Status404NotFound);
        }

        var inUse = await dbContext.Transactions
            .AnyAsync(transaction => transaction.CategoryId == id);

        if (inUse)
        {
            return Problem(
                detail: "This category is used by transactions and cannot be deleted.",
                statusCode: StatusCodes.Status409Conflict);
        }

        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }
}   