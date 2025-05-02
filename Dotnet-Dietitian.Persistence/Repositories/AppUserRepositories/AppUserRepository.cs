using Dotnet_Dietitian.Application.Interfaces.AppUserInterfaces;
using Dotnet_Dietitian.Domain.Entities;
using Dotnet_Dietitian.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Dotnet_Dietitian.Persistence.Repositories.AppUserRepositories;

public class AppUserRepository : BaseRepository<AppUser>, IAppUserRepository
{
    public AppUserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<AppUser> GetByUsernameAsync(string username)
    {
        return await _context.AppUsers
            .Include(u => u.AppRole)
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<AppUser> GetByUsernameAndPasswordAsync(string username, string password)
    {
        return await _context.AppUsers
            .Include(u => u.AppRole)
            .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
    }
}