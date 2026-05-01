using ECommerce.API.DTOs;

namespace ECommerce.API.Services;

public interface IProductService
{
    Task<PagedResponse<ProductResponse>> GetAllAsync(ProductQueryParameters query, CancellationToken cancellationToken = default);
    Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResponse<ProductResponse>> GetByCategoryAsync(int categoryId, ProductQueryParameters query, CancellationToken cancellationToken = default);
    Task<PagedResponse<ProductResponse>> SearchAsync(string keyword, ProductQueryParameters query, CancellationToken cancellationToken = default);
    Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
    Task<ProductResponse?> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
