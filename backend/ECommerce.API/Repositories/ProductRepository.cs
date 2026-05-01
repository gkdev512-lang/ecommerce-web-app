using ECommerce.API.Data;
using ECommerce.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ProductRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<(IReadOnlyList<Product> Products, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = BaseQuery();
        return GetPagedAsync(query, pageNumber, pageSize, cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await BaseQuery()
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);
    }

    public Task<(IReadOnlyList<Product> Products, int TotalCount)> GetByCategoryAsync(
        int categoryId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = BaseQuery()
            .Where(product => product.CategoryId == categoryId);

        return GetPagedAsync(query, pageNumber, pageSize, cancellationToken);
    }

    public Task<(IReadOnlyList<Product> Products, int TotalCount)> SearchAsync(
        string keyword,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var normalizedKeyword = keyword.Trim();
        var query = BaseQuery()
            .Where(product => product.Name.Contains(normalizedKeyword)
                || product.Description.Contains(normalizedKeyword));

        return GetPagedAsync(query, pageNumber, pageSize, cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _dbContext.Products.AddAsync(product, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        product.Category = null;
        _dbContext.Products.Update(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Product product, CancellationToken cancellationToken = default)
    {
        product.Category = null;
        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<Product> BaseQuery()
    {
        return _dbContext.Products
            .AsNoTracking()
            .Include(product => product.Category)
            .OrderByDescending(product => product.CreatedAt)
            .ThenBy(product => product.Name);
    }

    private static async Task<(IReadOnlyList<Product> Products, int TotalCount)> GetPagedAsync(
        IQueryable<Product> query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var totalCount = await query.CountAsync(cancellationToken);
        var products = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (products, totalCount);
    }
}
