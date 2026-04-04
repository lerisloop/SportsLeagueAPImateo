namespace SportsLeague.API.DTOs.Response
{
    public class TournamentSponsorResponseDTO
    {
        public int SponsorId { get; set; }
        public string SponsorName { get; set; } = string.Empty;

        public int TournamentId { get; set; }
        public string TournamentName { get; set; } = string.Empty;

        public decimal ContractAmount { get; set; }
    }
}
