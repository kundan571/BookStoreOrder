using BookStore.Order.Entity;
using BookStore.Order.Interface;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BookStore.Order.Services;

public class PaymentService : IPaymentService
{
    public async Task<string> PayOrder(PaymentRequestEntity paymentRequestEntity)
    {
        using(HttpClient client = new HttpClient())
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(ToDictionary(paymentRequestEntity));
            HttpResponseMessage response = await client.PostAsync("https://test.payu.in/_payment", content);
            string responseContent = response.RequestMessage.RequestUri.ToString();
            return responseContent;
        }
    }

    private string GetHash(PaymentRequestEntity paymentDetails, string merchantSalt)
    {
        string generatedHash = "";
        var hashString = $"{paymentDetails.key}|{paymentDetails.txnid}|{paymentDetails.amount}|{paymentDetails.productinfo}|{paymentDetails.firstname}|{paymentDetails.email}|||||||||||{merchantSalt}";

        using (var sha512 = SHA512.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(hashString);
            byte[] hashBytes = sha512.ComputeHash(bytes);

            foreach (byte hashByte in hashBytes)
            {
                generatedHash += String.Format("{0:x2}", hashByte);
            }

            return generatedHash;
        }
    }

    private Dictionary<string, string> ToDictionary(PaymentRequestEntity paymentDetails)
    {
        string successUrl = "https://localhost:7114/api/Order/PaymentConfirmation";
        string failureUrl = "https://localhost:7114/api/Order/PaymentConfirmation";
        string key = "Dz0d3J";
        string salt = "AIXQYrYLIyimk1soBn8iuTXEoGRjwfCm";

        paymentDetails.surl = successUrl;
        paymentDetails.furl = failureUrl;
        paymentDetails.key = key;
        paymentDetails.hash = GetHash(paymentDetails, salt);

        Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

        var properties = typeof(PaymentRequestEntity).GetProperties();
        foreach (var property in properties)
        {
            var value = property.GetValue(paymentDetails).ToString();
            keyValuePairs.Add(property.Name, value);
        }
        return keyValuePairs;
    }

    public async Task<PaymentResponseEntity> GetPaymentStatus(Stream body)
    {
        string content = await new StreamReader(body).ReadToEndAsync();
        var parsedData = HttpUtility.ParseQueryString(content);
        PaymentResponseEntity response = new()
        {
            txnid = parsedData["txnid"],
            mihpayid = parsedData["mihpayid"],
            status = parsedData["status"],
            hash = parsedData["hash"],
            bank_ref_num = parsedData["bank_ref_num"],
            error = parsedData["error"],
            error_message = parsedData["error_message"]
        };
        return response;

    }
}
