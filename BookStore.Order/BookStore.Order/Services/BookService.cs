using BookStore.Order.Entity;
using BookStore.Order.Interface;
using Newtonsoft.Json;

namespace BookStore.Order.Services;

public class BookService : IBookServices
{
    public async Task<BookEntity> GetBookById(int id)
    {
       // BookEntity book = null;
        using(HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync($"https://localhost:7050/api/Book/GetById?id={id}");
            if (response.IsSuccessStatusCode)
            {
                // apiContent -> string -> converted to ResponseEntity -> as string -> converted to BookEntity 
                string apiContent = await response.Content.ReadAsStringAsync();
                ResponseEntity responseEntity = JsonConvert.DeserializeObject<ResponseEntity>(apiContent);
                string bookContent = responseEntity.Data.ToString();
                BookEntity book = JsonConvert.DeserializeObject<BookEntity>(bookContent);
                return book;
            }

            return null;
        }
        
    }
}
