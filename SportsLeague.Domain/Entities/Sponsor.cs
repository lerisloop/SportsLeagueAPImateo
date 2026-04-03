using SportsLeague.Domain.Enums;
namespace SportsLeague.Domain.Entities
{
    public class Sponsor : AuditBase
    {
        public String Name { get; set; }
        public String ContactEmail { get; set; }

        public String Phone { get; set; }
        public String WebsiteUrl { get; set; }
        public SponsorCategory Category { get; set; }

        public ICollection<TournamentSponsor> TournamentSponsors { get; set; } = new List<TournamentSponsor>();

    }
}
