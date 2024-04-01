namespace PMS.Server.Options;

public sealed class PasswordValidationOptions
{
    public bool RequireLowercase { get; set; }
    public bool RequireUppercase { get; set; }
    public bool RequireNonAlphanumeric { get; set; }
    public bool RequireDigit { get; set; }

    public int RequiredLength { get; set; }
}
