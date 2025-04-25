using System.Linq.Expressions;
using Dotnet_Dietitian.Application.Interfaces.AppUserInterfaces;
using Dotnet_Dietitian.Domain.Entities;
using Dotnet_Dietitian.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Dotnet_Dietitian.Persistence.Repositories.AppUserRepositories;

public class AppUserRepository : IAppUserRepository
{
    private readonly ApplicationDbContext _context;

    public AppUserRepository(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<List<AppUser>> GetByFilterAsync(Expression<Func<AppUser, bool>> filter)
    {
        var values = await _context.AppUsers.Where(filter).ToListAsync();
        return values;
    }
}