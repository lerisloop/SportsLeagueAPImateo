using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SponsorController : ControllerBase
    {
        private readonly ISponsorService _service;
        private readonly IMapper _mapper;

        public SponsorController(ISponsorService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create(SponsorRequestDTO dto)
        {
            try
            {
                var sponsor = _mapper.Map<Sponsor>(dto);
                var created = await _service.CreateAsync(sponsor);
                return Created("", created);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/tournaments")]
        public async Task<IActionResult> Register(int id, TournamentSponsorRequestDTO dto)
        {
            try
            {
                await _service.RegisterToTournament(id, dto.TournamentId, dto.ContractAmount);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/tournaments")]
        public async Task<IActionResult> GetTournaments(int id)
        {
            var data = await _service.GetTournaments(id);

            var result = data.Select(t => new TournamentResponseDTO
            {
                Id = t.Id,
                Name = t.Name
            });

            return Ok(data.Select(t => new TournamentResponseDTO // estalla aca ( solucionar) => solucionado creo
            {
                Id = t.Id,
                Name = t.Name
            }));
        }

        [HttpDelete("{id}/tournaments/{tid}")]
        public async Task<IActionResult> Delete(int id, int tid)
        {
            await _service.RemoveFromTournament(id, tid);
            return NoContent();
        }
        // faltaria uno que muestre todos los sponsors 
    }
}
