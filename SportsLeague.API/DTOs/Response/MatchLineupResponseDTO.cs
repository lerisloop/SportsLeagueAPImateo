namespace SportsLeague.API.DTOs.Response;

public class MatchLineupResponseDTO
{
    public int Id { get; set; }

    public int MatchId { get; set; }

    public int PlayerId { get; set; }

    /// <summary>
    /// Nombre completo del jugador (FirstName + LastName)
    /// </summary>
    public string PlayerName { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del equipo al que pertenece el jugador
    /// </summary>
    public string TeamName { get; set; } = string.Empty;

    public bool IsStarter { get; set; }

    public string Position { get; set; } = string.Empty;
}
