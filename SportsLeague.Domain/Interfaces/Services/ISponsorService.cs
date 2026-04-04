using SportsLeague.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Interfaces.Services
{
    public interface ISponsorService
    {
       
        Task<IEnumerable<Sponsor>> GetAllAsync();
        Task<Sponsor?> GetByIdAsync(int id);
        Task<Sponsor> CreateAsync(Sponsor sponsor);
        Task UpdateAsync(int id, Sponsor sponsor);
        Task DeleteAsync(int id);

        
        Task RegisterToTournament(int sponsorId, int tournamentId, decimal contractAmount);
        Task<IEnumerable<Tournament>> GetTournaments(int sponsorId);

        Task RemoveFromTournament(int sponsorId, int tournamentId);
    }
}
