using System.IdentityModel.Tokens.Jwt;

namespace PMS.Lib.Services;

// NOTE: A simple singleton service for storing the JWT token & metadata we recieve when successfully logging in
public class TokenService
{
    public void ResetToken()
    {
        _token = "";
    }

    private string _token = "";
    virtual internal string Token
    {
        get => _token;
        set
        {
            _token = value;
            Expiration = new JwtSecurityToken(value).ValidTo;
        }
    }

    public DateTime Expiration { get; private set; } = DateTime.MinValue;
}
