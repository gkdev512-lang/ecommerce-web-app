using ECommerce.API.DTOs;

namespace ECommerce.API.Services;

public interface IPaymentService
{
    Task<PaymentResponse> CreateAsync(Guid userId, CreatePaymentRequest request, CancellationToken cancellationToken = default);
    Task<PaymentResponse?> GetByOrderIdAsync(Guid userId, Guid orderId, CancellationToken cancellationToken = default);
}
