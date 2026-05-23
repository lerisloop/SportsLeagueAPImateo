using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;

namespace SportsLeague.DataAccess.Repositories;

public class MatchLineupRepository : GenericRepository<MatchLineup>, IMatchLineupRepository
{
    public MatchLineupRepository(LeagueDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchId)
    {
        return await _dbSet
            .Where(Ml => Ml.MatchId == matchId)
            .ToListAsync();
    }

    public async Task<IEnumerable<MatchLineup>> GetByMatchWithDetailsAsync(int matchId)
    {
        return await _dbSet
            .Where(Ml => Ml.MatchId == matchId)
            .Include(Ml => Ml.Player)
                .ThenInclude(p => p.Team)
            .OrderBy(Ml => Ml.Player.TeamId)
            .ThenByDescending(Ml => Ml.IsStarter)
            .ToListAsync();
    }

    public async Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(
        int matchId, int teamId)
    {
        return await _dbSet
            .Where(Ml => Ml.MatchId == matchId && Ml.Player.TeamId == teamId)
            .Include(Ml => Ml.Player)
                .ThenInclude(p => p.Team)
            .OrderByDescending(Ml => Ml.IsStarter)
            .ToListAsync();
    }

    public async Task<MatchLineup?> GetByMatchAndPlayerAsync(int matchId, int playerId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(l => l.MatchId == matchId && l.PlayerId == playerId);
    }

    public async Task<int> CountStartersByMatchAndTeamAsync(int matchId, int teamId)
    {
        return await _dbSet
            .Where(Ml => Ml.MatchId == matchId
                     && Ml.Player.TeamId == teamId
                     && Ml.IsStarter)
            .CountAsync();
    }
}