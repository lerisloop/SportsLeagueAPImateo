using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.DataAccess.Repositories
{
    public class TournamentSponsorRepository : GenericRepository<TournamentSponsor>, ITournamentSponsorRepository
    {
        public TournamentSponsorRepository(LeagueDbContext context) : base(context) { }

        public async Task<TournamentSponsor?> GetByTournamentAndSponsorAsync(int tId, int sId)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.TournamentId == tId && x.SponsorId == sId);
        }

        public async Task<IEnumerable<TournamentSponsor>> GetBySponsorAsync(int sponsorId)
        {
            return await _dbSet
                .Where(x => x.SponsorId == sponsorId)
                .Include(x => x.Tournament)
                .ToListAsync();
        }
    }
}
