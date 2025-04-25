using Dotnet_Dietitian.Application.Features.Results.AppUserResults;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Application.Interfaces.AppUserInterfaces;
using Dotnet_Dietitian.Application.Queries.AppUserQueries;
using Dotnet_Dietitian.Domain.Entities;
using MediatR;

namespace Dotnet_Dietitian.Application.Handlers.AppUserHandlers;

public class GetCheckAppUserQueryHandler : IRequestHandler<GetCheckAppUserQuery, GetCheckAppUserQueryResult>
{
    private readonly IRepository<AppUser> _appUserRepository;
    private readonly IRepository<AppRole> _appRoleRepository;
    

    public GetCheckAppUserQueryHandler(IRepository<AppUser> appUserRepository, IRepository<AppRole> appRoleRepository)
    {
        _appUserRepository = appUserRepository;
        _appRoleRepository = appRoleRepository;
    }

    public async Task<GetCheckAppUserQueryResult> Handle(GetCheckAppUserQuery request, CancellationToken cancellationToken)
    {
        var values = new GetCheckAppUserQueryResult();
        var user = await _appUserRepository.GetByFilterAsync(x => x.Username == request.Username && x.Password==request.Password);
        if (user == null)
        {
            values.IsExist = false;
        }
        else
        {
            values.IsExist = true;
            values.Username = user.Username;
            values.Role = (await _appRoleRepository.GetByFilterAsync(x => x.AppRoleId == user.AppRoleId)).AppRoleName;
            values.Id = user.AppUserId;
        }

        return values;
    }
    
}