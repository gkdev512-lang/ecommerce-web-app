using ECommerce.API.DTOs;
using ECommerce.API.Models;
using ECommerce.API.Repositories;

namespace ECommerce.API.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<PagedResponse<ProductResponse>> GetAllAsync(
        ProductQueryParameters query,
        CancellationToken cancellationToken = default)
    {
        var (products, totalCount) = await _productRepository.GetAllAsync(
            query.PageNumber,
            query.PageSize,
            cancellationToken);

        return ToPagedResponse(products, totalCount, query);
    }

    public async Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        return product is null ? null : ToResponse(product);
    }

    public async Task<PagedResponse<ProductResponse>> GetByCategoryAsync(
        int categoryId,
        ProductQueryParameters query,
        CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
        if (category is null)
        {
            throw new KeyNotFoundException("Category not found.");
        }

        var (products, totalCount) = await _productRepository.GetByCategoryAsync(
            categoryId,
            query.PageNumber,
            query.PageSize,
            cancellationToken);

        return ToPagedResponse(products, totalCount, query);
    }

    public async Task<PagedResponse<ProductResponse>> SearchAsync(
        string keyword,
        ProductQueryParameters query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return new PagedResponse<ProductResponse>
            {
                Items = [],
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalCount = 0,
                TotalPages = 0
            };
        }

        var (products, totalCount) = await _productRepository.SearchAsync(
            keyword,
            query.PageNumber,
            query.PageSize,
            cancellationToken);

        return ToPagedResponse(products, totalCount, query);
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureCategoryExistsAsync(request.CategoryId, cancellationToken);

        var product = new Product
        {
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            CategoryId = request.CategoryId,
            ImageUrl = request.ImageUrl.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _productRepository.AddAsync(product, cancellationToken);

        var created = await _productRepository.GetByIdAsync(product.Id, cancellationToken);
        return ToResponse(created ?? product);
    }

    public async Task<ProductResponse?> UpdateAsync(
        Guid id,
        UpdateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            return null;
        }

        await EnsureCategoryExistsAsync(request.CategoryId, cancellationToken);

        product.Name = request.Name.Trim();
        product.Description = request.Description.Trim();
        product.Price = request.Price;
        product.StockQuantity = request.StockQuantity;
        product.CategoryId = request.CategoryId;
        product.ImageUrl = request.ImageUrl.Trim();

        await _productRepository.UpdateAsync(product, cancellationToken);

        var updated = await _productRepository.GetByIdAsync(id, cancellationToken);
        return ToResponse(updated ?? product);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            return false;
        }

        await _productRepository.DeleteAsync(product, cancellationToken);
        return true;
    }

    private async Task EnsureCategoryExistsAsync(int categoryId, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
        if (category is null)
        {
            throw new KeyNotFoundException("Category not found.");
        }
    }

    private static PagedResponse<ProductResponse> ToPagedResponse(
        IReadOnlyList<Product> products,
        int totalCount,
        ProductQueryParameters query)
    {
        return new PagedResponse<ProductResponse>
        {
            Items = products.Select(ToResponse).ToList(),
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
        };
    }

    private static ProductResponse ToResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty,
            ImageUrl = product.ImageUrl,
            CreatedAt = product.CreatedAt
        };
    }
}
