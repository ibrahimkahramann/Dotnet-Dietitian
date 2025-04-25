using System.Linq.Expressions;
using Dotnet_Dietitian.Domain.Entities;

namespace Dotnet_Dietitian.Application.Interfaces.AppUserInterfaces;

public interface IAppUserRepository
{
     Task<List<AppUser>> GetByFilterAsync(Expression<Func<AppUser, bool>> filter);
}