using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services;

public class MatchLineupService : IMatchLineupService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly IMatchLineupRepository _MatchlineupRepository;
    private readonly ILogger<MatchLineupService> _logger;

    public MatchLineupService(
        IMatchRepository matchRepository,
        IPlayerRepository playerRepository,
        IMatchLineupRepository MatchLineupRepository,
        ILogger<MatchLineupService> logger)
    {
        _matchRepository = matchRepository;
        _playerRepository = playerRepository;
        _MatchlineupRepository = MatchLineupRepository;
        _logger = logger;
    }

    public async Task<MatchLineup> AddPlayerToLineupAsync(int matchId, MatchLineup lineup)
    {
       
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            throw new KeyNotFoundException(
                $"No se encontró el partiod con ID {matchId}");

       
        var player = await _playerRepository.GetByIdAsync(lineup.PlayerId);
        if (player == null)
            throw new KeyNotFoundException(
                $"No se encontró el jugador con ID {lineup.PlayerId}");

       
        if (player.TeamId != match.HomeTeamId && player.TeamId != match.AwayTeamId)
            throw new InvalidOperationException(
                "El jugador no pertenece a ninguno de los equipos del partido");

     
        var existing = await _MatchlineupRepository.GetByMatchAndPlayerAsync(matchId, lineup.PlayerId);
        if (existing != null)
            throw new InvalidOperationException(
                "este jugador ya ha sido registrado");

        if (lineup.IsStarter)
        {
            var starterCount = await _MatchlineupRepository
                .CountStartersByMatchAndTeamAsync(matchId, player.TeamId);

            if (starterCount >= 11)
                throw new InvalidOperationException(
                    "no se pueden tener mas de 11 titulares en un equipo");
        }
        
        if (match.Status != MatchStatus.Scheduled)
            throw new InvalidOperationException(
                "Solo se pueden registrar en partidso agendados");

        lineup.MatchId = matchId;

        _logger.LogInformation(
            "Adding player {PlayerId} to lineup for match {MatchId}. IsStarter: {IsStarter}",
            lineup.PlayerId, matchId, lineup.IsStarter);

        return await _MatchlineupRepository.CreateAsync(lineup);
    }

    public async Task<IEnumerable<MatchLineup>> GetLineupByMatchAsync(int matchId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            throw new KeyNotFoundException(
                $"No se encontró el partido con ID {matchId}");

        var matchLineup = await _MatchlineupRepository.GetByMatchAsync(matchId);
        _logger.LogInformation("Getting players of match with ID: {MatchId}", matchId);
        return matchLineup;

   
    }

    public async Task<IEnumerable<MatchLineup>> GetLineupByMatchAndTeamAsync(
        int matchId, int teamId)
    {
        var exist = await _matchRepository.GetByIdAsync(matchId);
        if (exist == null)
            throw new KeyNotFoundException(
                $"No se encontró el partido con ID {matchId}");

        return await _MatchlineupRepository.GetByMatchAndTeamAsync(matchId, teamId);
    }

    public async Task DeleteLineupEntryAsync(int matchId, int lineupId)
    {
        var entry = await _MatchlineupRepository.GetByIdAsync(lineupId);
        if (entry == null || entry.MatchId != matchId)
            throw new KeyNotFoundException(
                $"No se encontró la entrada de alineación con ID {lineupId} para el partido {matchId}");

        _logger.LogInformation(
            "Removing lineup entry {LineupId} from match {MatchId}", lineupId, matchId);

        await _MatchlineupRepository.DeleteAsync(lineupId);
    }
}