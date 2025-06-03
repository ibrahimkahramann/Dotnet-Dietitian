using Dotnet_Dietitian.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.Application.Interfaces;

public interface IIlerlemeOlcumRepository : IRepository<IlerlemeOlcum>
{
    Task<IReadOnlyList<IlerlemeOlcum>> GetIlerlemeOlcumlerByHastaIdAsync(Guid hastaId);
    Task<IlerlemeOlcum> GetSonIlerlemeOlcumByHastaIdAsync(Guid hastaId);
} 