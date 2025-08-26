using JudgeAPI.Data;
using JudgeAPI.Entities;
using JudgeAPI.Models;
using JudgeAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JudgeAPI.Controllers
{
    [ApiController]
    [Route("api/units")]
    [Authorize]
    public class UnitControllers : ControllerBase
    {
        private readonly IUnitService _unitService;

        public UnitControllers(IUnitService unitService)
        {
            _unitService = unitService;
        }

        [HttpGet(Name = "GetAllUnits")]
        public async Task<ActionResult<List<UnitResponseDTO>>> GetAll()
        {
            var units = await _unitService.GetAllAsync();
            return Ok(units);
        }

        [HttpPost]
        public async Task<ActionResult<UnitResponseDTO>> Post([FromBody]UnitCreateDTO unitDTO)
        {
            var responseDTO = await _unitService.CreateAsync(unitDTO);
            return CreatedAtAction(nameof(GetUnit), new {id = responseDTO.Id}, responseDTO);

        }

        [HttpGet("{id:int}", Name = "GetUnit")]
        public async Task<ActionResult<UnitResponseDTO>> GetUnit(int id)
        {
            var unit = await _unitService.GetByIdAsync(id);
            return Ok(unit);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<UnitResponseDTO>> Put(int id, [FromBody] UnitUpdateDTO dto)
        {
            if(id != dto.Id)
                return BadRequest("El Id del body no coincide con la ruta");

            var responseDTO = await _unitService.UpdateAsync(dto);
            return Ok(responseDTO);
        }
    }
}
