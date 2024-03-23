using Microsoft.AspNetCore.Identity;

namespace PMS.Server.Models;

// We don't use roles currently, so we have this empty default role
public class DefaultRole : IdentityRole
{
}
