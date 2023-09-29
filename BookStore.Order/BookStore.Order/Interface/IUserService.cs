using BookStore.Order.Entity;

namespace BookStore.Order.Interface;

public interface IUserService
{
    Task<UserEntity> GetUser(string jwtToken);
}
