using ECommerce.API.DTOs;
using ECommerce.API.Models;
using ECommerce.API.Repositories;

namespace ECommerce.API.Services;

public class PaymentService : IPaymentService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;

    public PaymentService(IOrderRepository orderRepository, IPaymentRepository paymentRepository)
    {
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<PaymentResponse> CreateAsync(
        Guid userId,
        CreatePaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null || order.UserId != userId)
        {
            throw new KeyNotFoundException("Order not found.");
        }

        var existingPayment = await _paymentRepository.GetByOrderIdAsync(request.OrderId, cancellationToken);
        if (existingPayment is not null)
        {
            throw new InvalidOperationException("Payment already exists for this order.");
        }

        var status = NormalizeStatus(request.Status);
        var payment = new Payment
        {
            OrderId = order.Id,
            Amount = order.TotalAmount,
            PaymentMethod = request.PaymentMethod.Trim(),
            Status = status,
            TransactionId = GenerateTransactionId(),
            PaidAt = status == PaymentStatuses.Success ? DateTime.UtcNow : null
        };

        var created = await _paymentRepository.AddAsync(payment, cancellationToken);
        return ToResponse(created);
    }

    public async Task<PaymentResponse?> GetByOrderIdAsync(
        Guid userId,
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var payment = await _paymentRepository.GetByOrderIdAsync(orderId, cancellationToken);
        if (payment is null || payment.Order?.UserId != userId)
        {
            return null;
        }

        return ToResponse(payment);
    }

    private static string NormalizeStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return PaymentStatuses.Success;
        }

        if (status.Equals(PaymentStatuses.Pending, StringComparison.OrdinalIgnoreCase))
        {
            return PaymentStatuses.Pending;
        }

        if (status.Equals(PaymentStatuses.Success, StringComparison.OrdinalIgnoreCase))
        {
            return PaymentStatuses.Success;
        }

        if (status.Equals(PaymentStatuses.Failed, StringComparison.OrdinalIgnoreCase))
        {
            return PaymentStatuses.Failed;
        }

        throw new ArgumentException("Payment status must be Pending, Success, or Failed.", nameof(status));
    }

    private static string GenerateTransactionId()
    {
        return $"TXN-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}"[..36];
    }

    private static PaymentResponse ToResponse(Payment payment)
    {
        return new PaymentResponse
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            Amount = payment.Amount,
            PaymentMethod = payment.PaymentMethod,
            Status = payment.Status,
            TransactionId = payment.TransactionId,
            PaidAt = payment.PaidAt
        };
    }
}
