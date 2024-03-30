using Microsoft.AspNetCore.Identity;

namespace PMS.Server.Models;

public sealed class UserModel : IdentityUser
{
    public UserModel() : base() { }
    public UserModel(string username) : base(username) { }
}
