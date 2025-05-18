using Dotnet_Dietitian.Application.Features.Results.AppUserResults;
using Dotnet_Dietitian.Application.Interfaces;
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
        // Şifreye göre kullanıcıyı kontrol et
        var values = await _appUserRepository.GetByFilterAsync(x => 
            x.Username == request.Username && 
            x.Password == request.Password);
        
        if (values != null)
        {
            var appRole = await _appRoleRepository.GetByFilterAsync(x => x.Id == values.AppRoleId);
            
            return new GetCheckAppUserQueryResult
            {
                IsExist = true,
                Username = values.Username,
                Id = values.Id,
                Role = appRole?.AppRoleName ?? "Bilinmeyen Rol"
            };
        }
        
        return new GetCheckAppUserQueryResult
        {
            IsExist = false
        };
    }
}