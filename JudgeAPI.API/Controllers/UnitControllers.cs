using JudgeAPI.Application.Features.Units.Dtos;
using JudgeAPI.Application.Features.Units.Interfaces;
using JudgeAPI.Models.Unit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JudgeAPI.API.Controllers
{
    [ApiController]
    [Route("api/units")]
    [Authorize]
    public class UnitControllers(IUnitService unitService) : ControllerBase
    {
        private readonly IUnitService _unitService = unitService;

        [HttpGet(Name = "GetAllUnits")]
        public async Task<ActionResult<List<UnitResponseDTO>>> GetAll()
        {
            List<UnitResponseDTO> units = await _unitService.GetAllAsync();
            return Ok(units);
        }

        [HttpPost]
        public async Task<ActionResult<UnitResponseDTO>> Post([FromBody] UnitCreateDTO unitDTO)
        {
            UnitResponseDTO responseDTO = await _unitService.CreateAsync(unitDTO);
            return CreatedAtAction(nameof(GetUnit), new { id = responseDTO.Id }, responseDTO);

        }

        [HttpGet("{id:int}", Name = "GetUnit")]
        public async Task<ActionResult<UnitResponseDTO>> GetUnit(int id)
        {
            UnitResponseDTO unit = await _unitService.GetByIdAsync(id);
            return Ok(unit);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<UnitResponseDTO>> Put(int id, [FromBody] UnitUpdateDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("El Id del body no coincide con la ruta");
            }

            UnitResponseDTO responseDTO = await _unitService.UpdateAsync(dto);
            return Ok(responseDTO);
        }
    }
}
