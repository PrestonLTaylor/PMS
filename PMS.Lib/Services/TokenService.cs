using PMS.Services.Product;

namespace PMS.Lib.Services;

// NOTE: A simple singleton service for storing the JWT token we generate when successfully logging in
internal class TokenService
{
    virtual public string Token { get; set; } = "";
}
