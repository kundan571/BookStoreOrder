using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Order.Entity;

public class OrderEntity
{
    [Key]
    public string OrderId { get; set; }
    [Required]
    public int BookId { get; set; }
    [Required]
    public int UserId { get; set; }
    [Required]
    public int Quantity { get; set; }
    public bool IsPaid { get; set; } = false;
    [NotMapped]
    public float OrderAmount { get; set; }
    [NotMapped]
    public BookEntity Book { get; set; }
    [NotMapped]
    public UserEntity User { get; set; }
}
