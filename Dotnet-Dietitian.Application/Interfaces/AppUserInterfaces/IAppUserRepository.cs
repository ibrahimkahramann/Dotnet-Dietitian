using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.Application.Interfaces.AppUserInterfaces;

public interface IAppUserRepository : IRepository<AppUser>
{
    Task<AppUser> GetByUsernameAsync(string username);
    Task<AppUser> GetByUsernameAndPasswordAsync(string username, string password);
}