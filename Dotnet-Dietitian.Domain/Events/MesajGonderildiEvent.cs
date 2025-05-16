using System;

namespace Dotnet_Dietitian.Domain.Events
{
    public interface MesajGonderildiEvent
    {
        Guid MesajId { get; }
        Guid GonderenId { get; }
        string GonderenTipi { get; }
        Guid AliciId { get; }
        string AliciTipi { get; }
        DateTime GonderimZamani { get; }
    }
}