using System;

namespace Dotnet_Dietitian.Domain.Events
{
    public interface MesajOkunduEvent
    {
        Guid MesajId { get; }
        Guid OkuyanId { get; }
        string OkuyanTipi { get; }
        DateTime OkunmaZamani { get; }
    }
}