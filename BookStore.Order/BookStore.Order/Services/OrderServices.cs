using BookStore.Order.Entity;
using BookStore.Order.Interface;

namespace BookStore.Order.Services;

public class OrderServices : IOrderServices
{
    private readonly OrderContext _db;
    private readonly IBookServices _book;
    private readonly IUserService _user;
    private readonly IPaymentService _payment;

    public OrderServices(OrderContext db, IBookServices book, IUserService user, IPaymentService payment)
    {
        _db = db;
        _book = book;
        _user = user;
        _payment = payment;

    }
    public async Task<string> PlaceOrder(string token, int userId, int bookId, int quantity)
    {
        OrderEntity newOrder = new OrderEntity()
        {
            OrderId = Guid.NewGuid().ToString(),
            BookId = bookId,
            UserId = userId,
            Quantity = quantity,
            Book = await _book.GetBookById(bookId),
            User = await _user.GetUser(token)
        };
        _db.Orders.Add(newOrder);
        _db.SaveChanges();

        PaymentRequestEntity paymentRequest = new()
        {
            firstname = newOrder.User.Name,
            email = newOrder.User.Email,
            amount = (newOrder.Book.DiscountedPrice * newOrder.Quantity).ToString(),
            phone = newOrder.User.PhoneNumber,
            productinfo = newOrder.Book.BookName,
            txnid = newOrder.OrderId,
        };

        string payment = await _payment.PayOrder(paymentRequest);
        return payment;
    }

    public IEnumerable<OrderEntity> GetOrders(int userId)
    {
        IEnumerable<OrderEntity> orders = _db.Orders.Where(o => o.UserId == userId);
        return orders;
    }

    public OrderEntity AfterPayment(string orderId)
    {
        OrderEntity order = _db.Orders.FirstOrDefault(x => x.OrderId == orderId);
        if (order != null)
        {
            order.IsPaid = true;
            _db.Orders.Update(order);
            _db.SaveChanges();
        }
        return order;
    }

}
