using BookStore.Order.Entity;

namespace BookStore.Order.Interface;

public interface IPaymentService
{
    Task<string> PayOrder(PaymentRequestEntity paymentRequestEntity);
    Task<PaymentResponseEntity> GetPaymentStatus(Stream body);
}
