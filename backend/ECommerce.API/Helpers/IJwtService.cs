using ECommerce.API.Models;

namespace ECommerce.API.Helpers;

public interface IJwtService
{
    (string token, DateTime expiresAtUtc) GenerateToken(User user);
}
