namespace ECommerce.API.Models;

public static class UserRoles
{
    public const string Admin = "Admin";
    public const string Customer = "Customer";

    public static bool IsValid(string role)
    {
        return role is Admin or Customer;
    }
}
