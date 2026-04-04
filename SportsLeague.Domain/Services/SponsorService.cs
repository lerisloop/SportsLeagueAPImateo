using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Services
{

    
        public class SponsorService : ISponsorService
        {
            private readonly ISponsorRepository _sponsorRepository;
            private readonly ITournamentRepository _tournamentRepository;
            private readonly ITournamentSponsorRepository _tournamentsponsorRepository;
            private readonly ILogger<SponsorService> _logger;

            public SponsorService(
                ISponsorRepository sponsorRepo,
                ITournamentRepository tournamentRepo,
                ITournamentSponsorRepository tsRepo,
                ILogger<SponsorService> logger)
            {
                _sponsorRepository = sponsorRepo;
                _tournamentRepository = tournamentRepo;
                _tournamentsponsorRepository = tsRepo;
                _logger = logger;
            }

           
            public async Task<IEnumerable<Sponsor>> GetAllAsync()
            {
                return await _sponsorRepository.GetAllAsync();
            }

           
            public async Task<Sponsor?> GetByIdAsync(int id)
            {
                return await _sponsorRepository.GetByIdAsync(id);
            }

            
       
        public async Task<Sponsor> CreateAsync(Sponsor sponsor)

        {

            // Validación de negocio: nombre único

            var existingSponsor = await _sponsorRepository.GetByNameAsync(sponsor.Name);

            if (existingSponsor != null)

            {

                _logger.LogWarning("Sponsor with name '{SponsorName}' already exists", sponsor.Name);

                throw new InvalidOperationException(

                $"Ya existe un equipo con el nombre '{sponsor.Name}'");

            }
            // Validacion de negocio: ContactEmail debe ser un formato válido 
            if (!sponsor.ContactEmail.Contains("@"))
            {
                _logger.LogWarning("Email not valid", sponsor.Name);

                throw new InvalidOperationException("Email inválido");

            }
            _logger.LogInformation("Creating sponsor: {sponsorName}", sponsor.Name);

            return await _sponsorRepository.CreateAsync(sponsor);

        }


     
        public async Task UpdateAsync(int id, Sponsor sponsor)

        {

            var existing = await _sponsorRepository.GetByIdAsync(id);

            if (existing == null)

                throw new KeyNotFoundException($"No se encontró el sponsor con ID {id}");

            if (existing.Name != sponsor.Name)

            {

                var teamWithSameName = await _sponsorRepository.GetByNameAsync(sponsor.Name);

                if (teamWithSameName != null)

                {

                    throw new InvalidOperationException(

                    $"Ya existe un equipo con el nombre '{sponsor.Name}'");

                }

            }

            if (!sponsor.ContactEmail.Contains("@"))
            {
                _logger.LogWarning("Email not valid", sponsor.Name);

                throw new InvalidOperationException("Email inválido");

            }





            existing.Name = sponsor.Name;
            existing.ContactEmail = sponsor.ContactEmail;
            existing.Phone = sponsor.Phone;
            existing.WebsiteUrl = sponsor.WebsiteUrl;
            existing.Category = sponsor.Category;
            existing.UpdatedAt = DateTime.UtcNow;


            _logger.LogInformation("Updating Sponsor with ID: {TournamentId}", id);

            await _sponsorRepository.UpdateAsync(existing);

        }


        public async Task DeleteAsync(int id)
        {
            var existing = await _sponsorRepository.GetByIdAsync(id);

            if (existing == null)
            {
                _logger.LogWarning("sponsor with ID {SponsorId} not found for deletion", id);

                throw new KeyNotFoundException(

                $"No se encontró el patrocinador con ID {id}");
            }
            _logger.LogInformation("Deleting sponsor with ID: {SponsorId}", id);
            await _sponsorRepository.DeleteAsync(id);
            }
        
            
            public async Task RegisterToTournament(int sponsorId, int tournamentId, decimal amount)
            {
                if (amount <= 0)
                    throw new InvalidOperationException("Monto inválido");

                var sponsor = await _sponsorRepository.GetByIdAsync(sponsorId);
                if (sponsor == null)
                    throw new InvalidOperationException("Sponsor no existe");

                var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
                if (tournament == null)
                    throw new KeyNotFoundException("Torneo no existe");

                var exists = await _tournamentsponsorRepository.GetByTournamentAndSponsorAsync(tournamentId, sponsorId);
                if (exists != null)
                    throw new InvalidOperationException("Ya vinculado");

                await _tournamentsponsorRepository.CreateAsync(new TournamentSponsor
                {
                    SponsorId = sponsorId,
                    TournamentId = tournamentId,
                    ContractAmount = amount,
                    JoinedAt = DateTime.UtcNow
                });
            }

            public async Task<IEnumerable<Tournament>> GetTournaments(int sponsorId)
            {
                var sponsor = await _sponsorRepository.GetByIdAsync(sponsorId);
                if (sponsor == null)
                    throw new KeyNotFoundException("Sponsor no existe");

                var list = await _tournamentsponsorRepository.GetBySponsorAsync(sponsorId);
                return list.Select(x => x.Tournament);
            }

            public async Task RemoveFromTournament(int sponsorId, int tournamentId)
            {
                var tournamentsponsor = await _tournamentsponsorRepository.GetByTournamentAndSponsorAsync(tournamentId, sponsorId);

                if (tournamentsponsor == null)
                    throw new KeyNotFoundException("Relación no existe");

                await _tournamentsponsorRepository.DeleteAsync(tournamentsponsor.Id);
            }
     
    }
    }

