using PMS.Server.Models;

namespace PMS.Server.Services;

public interface IJwtGeneratorService
{
    public string Generate(UserModel user);
}
