using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Services
{

    
        public class SponsorService : ISponsorService
        {
            private readonly ISponsorRepository _sponsorRepo;
            private readonly ITournamentRepository _tournamentRepo;
            private readonly ITournamentSponsorRepository _tsRepo;

            public SponsorService(
                ISponsorRepository sponsorRepo,
                ITournamentRepository tournamentRepo,
                ITournamentSponsorRepository tsRepo)
            {
                _sponsorRepo = sponsorRepo;
                _tournamentRepo = tournamentRepo;
                _tsRepo = tsRepo;
            }

            // 🔹 GET ALL
            public async Task<IEnumerable<Sponsor>> GetAllAsync()
            {
                return await _sponsorRepo.GetAllAsync();
            }

            // 🔹 GET BY ID
            public async Task<Sponsor?> GetByIdAsync(int id)
            {
                return await _sponsorRepo.GetByIdAsync(id);
            }

            // 🔹 CREATE
            public async Task<Sponsor> CreateAsync(Sponsor s)
            {
                if (await _sponsorRepo.ExistsByNameAsync(s.Name))
                    throw new InvalidOperationException("Nombre duplicado");

                if (!s.ContactEmail.Contains("@"))
                    throw new InvalidOperationException("Email inválido");

                return await _sponsorRepo.CreateAsync(s);
            }

            // 🔹 UPDATE
            public async Task UpdateAsync(int id, Sponsor sponsor)
            {
                var existing = await _sponsorRepo.GetByIdAsync(id);

                if (existing == null)
                    throw new KeyNotFoundException("Sponsor no existe");

                if (await _sponsorRepo.ExistsByNameAsync(sponsor.Name) &&
                    existing.Name.ToLower() != sponsor.Name.ToLower())
                    throw new InvalidOperationException("Nombre duplicado");

                if (!sponsor.ContactEmail.Contains("@"))
                    throw new InvalidOperationException("Email inválido");

                existing.Name = sponsor.Name;
                existing.ContactEmail = sponsor.ContactEmail;
                existing.Phone = sponsor.Phone;
                existing.WebsiteUrl = sponsor.WebsiteUrl;
                existing.Category = sponsor.Category;
                existing.UpdatedAt = DateTime.UtcNow;

                await _sponsorRepo.UpdateAsync(existing);
            }

            // 🔹 DELETE
            public async Task DeleteAsync(int id)
            {
                var existing = await _sponsorRepo.GetByIdAsync(id);

                if (existing == null)
                    throw new KeyNotFoundException("Sponsor no existe");

                await _sponsorRepo.DeleteAsync(id);
            }

            // 🔹 N:M → REGISTRAR
            public async Task RegisterToTournament(int sponsorId, int tournamentId, decimal amount)
            {
                if (amount <= 0)
                    throw new InvalidOperationException("Monto inválido");

                var sponsor = await _sponsorRepo.GetByIdAsync(sponsorId);
                if (sponsor == null)
                    throw new KeyNotFoundException("Sponsor no existe");

                var tournament = await _tournamentRepo.GetByIdAsync(tournamentId);
                if (tournament == null)
                    throw new KeyNotFoundException("Torneo no existe");

                var exists = await _tsRepo.GetByTournamentAndSponsorAsync(tournamentId, sponsorId);
                if (exists != null)
                    throw new InvalidOperationException("Ya vinculado");

                await _tsRepo.CreateAsync(new TournamentSponsor
                {
                    SponsorId = sponsorId,
                    TournamentId = tournamentId,
                    ContractAmount = amount,
                    JoinedAt = DateTime.UtcNow
                });
            }

            // 🔹 N:M → LISTAR TORNEOS
            public async Task<IEnumerable<Tournament>> GetTournaments(int sponsorId)
            {
                var sponsor = await _sponsorRepo.GetByIdAsync(sponsorId);
                if (sponsor == null)
                    throw new KeyNotFoundException("Sponsor no existe");

                var list = await _tsRepo.GetBySponsorAsync(sponsorId);
                return list.Select(x => x.Tournament);
            }

            // 🔹 N:M → ELIMINAR RELACIÓN
            public async Task RemoveFromTournament(int sponsorId, int tournamentId)
            {
                var ts = await _tsRepo.GetByTournamentAndSponsorAsync(tournamentId, sponsorId);

                if (ts == null)
                    throw new KeyNotFoundException("Relación no existe");

                await _tsRepo.DeleteAsync(ts.Id);
            }
        }
    }

