using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
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
        private readonly ISponsorService _sponsorService;
        private readonly IMapper _mapper;

        public SponsorController(ISponsorService service, IMapper mapper)
        {
            _sponsorService = service;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create(SponsorRequestDTO dto)
        {
            try
            {
                var sponsor = _mapper.Map<Sponsor>(dto);
                var created = await _sponsorService.CreateAsync(sponsor);
                return Created("", created);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

     
        [HttpGet]

        public async Task<ActionResult<IEnumerable<SponsorResponseDTO>>> GetAll()

        {

            var Sponsor = await _sponsorService.GetAllAsync();

            return Ok(_mapper.Map<IEnumerable<SponsorResponseDTO>>(Sponsor));

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sponsor = await _sponsorService.GetByIdAsync(id);

            if (sponsor == null)
                return NotFound(new { message = $"Sponsor con ID {id} no encontrado" });

            return Ok(_mapper.Map<SponsorResponseDTO>(sponsor));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SponsorRequestDTO dto)
        {
            try
            {
                var sponsor = _mapper.Map<Sponsor>(dto);

                await _sponsorService.UpdateAsync(id, sponsor);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });

            }

        }
        

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSponsor(int id)
        {
            try
            {
                await _sponsorService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

     
        [HttpPost("{id}/tournaments")]
        public async Task<IActionResult> Register(int id, TournamentSponsorRequestDTO dto)
        {
            try
            {
                await _sponsorService.RegisterToTournament(id, dto.TournamentId, dto.ContractAmount);
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
            try
            {

                var tournaments = await _sponsorService.GetTournaments(id);
                var result = _mapper.Map<IEnumerable<TournamentResponseDTO>>(tournaments);
                if (!result.Any())
                {
                    return NotFound(new { message = $"El sponsor no esta vinculado a ningun torneo" });
                }

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}/tournaments/{tid}")]
        public async Task<IActionResult> Delete(int id, int tid)
        {
            await _sponsorService.RemoveFromTournament(id, tid);
            return NoContent();
        }
       
    }
}
