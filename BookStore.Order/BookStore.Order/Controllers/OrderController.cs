using BookStore.Order.Entity;
using BookStore.Order.Interface;
using BookStore.Order.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace BookStore.Order.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderServices _order;
        private readonly IPaymentService _payment;
        ResponseEntity response;

        public OrderController(IOrderServices order, IPaymentService payment)
        {
            _order = order;
            response = new ResponseEntity();
            _payment = payment;

        }

        [Authorize]
        [HttpPost]
        [Route("Create")]
        public async Task<ResponseEntity> AddOrder(int bookId, int quantity)
        {
            string jwtTokenWithBearer = HttpContext.Request.Headers["Authorization"];
            string jwtToken = jwtTokenWithBearer.Substring("Bearer ".Length);

            int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Sid));
            
            string paymentResponse = await _order.PlaceOrder(jwtToken, userId, bookId, quantity);
            if (paymentResponse != null)
            {
                response.Data = paymentResponse;

            }
            else
            {
                response.IsSuccess = false;
                response.Message = "Something went wrong";
            }

            return response;
        }

        [Authorize]
        [HttpGet]
        [Route("GetOrders")]
        public ResponseEntity GetOrders()
        {
            int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Sid));
            IEnumerable<OrderEntity> orders = _order.GetOrders(userId);
            if (orders.Any())
            {
                response.Data = orders;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = "SOmething went wrong";
            }
            return response;
        }

        [HttpPost]
        [Route("PaymentConfirmation")]
        public async Task<ResponseEntity> PaymentConfirmation()
        {
            Stream body = Request.Body;
            PaymentResponseEntity paymentResponse = await _payment.GetPaymentStatus(body);
            if(paymentResponse.status == "success")
            {
                OrderEntity updateOrder = _order.AfterPayment(paymentResponse.txnid);
                response.Data = paymentResponse;
                response.IsSuccess = true;
                response.Message = "Payment received";
            }
            else
            {
               // return BadRequest(response.Message = "payment failed");
            }
            return response;
        }
    }
}
