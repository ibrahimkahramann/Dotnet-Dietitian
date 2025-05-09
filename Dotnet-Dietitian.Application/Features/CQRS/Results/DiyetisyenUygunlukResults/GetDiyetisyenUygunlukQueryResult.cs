namespace Dotnet_Dietitian.Application.Features.CQRS.Results.DiyetisyenUygunlukResults
{
    public class GetDiyetisyenUygunlukQueryResult
    {
        public Guid Id { get; set; }
        public Guid DiyetisyenId { get; set; }
        public string DiyetisyenAdSoyad { get; set; }
        public DateTime BaslangicZamani { get; set; }
        public DateTime BitisZamani { get; set; }
        public bool Muayit { get; set; }
        public bool TekrarlanirMi { get; set; }
        public string? TekrarlanmaSikligi { get; set; }
    }
}