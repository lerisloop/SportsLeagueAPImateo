namespace SportsLeague.API.DTOs.Request;

public class MatchLineupRequestDTO
{
    public int PlayerId { get; set; }

    public bool IsStarter { get; set; }

    /// <summary>
    /// Posición asignada para este partido. Ej: "GK", "CB", "CDM", "CAM", "ST"
    /// </summary>
    public string Position { get; set; } = string.Empty;
}
