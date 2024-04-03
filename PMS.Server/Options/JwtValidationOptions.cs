namespace PMS.Server.Options;

public sealed class JwtValidationOptions
{
    public string Issuer { get; set; } = "";
    public string Audience { get; set; } = "";
    public string Secret { get; set; } = "";
}
