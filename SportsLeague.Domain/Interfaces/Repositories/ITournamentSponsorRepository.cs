using System;
using System.Collections.Generic;
using System.Text;
using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories
{
    public interface ITournamentSponsorRepository : IGenericRepository<TournamentSponsor>
    {
        Task<TournamentSponsor?> GetByTournamentAndSponsorAsync(int tId, int sId);
        Task<IEnumerable<TournamentSponsor>> GetBySponsorAsync(int sponsorId);
    }
}
