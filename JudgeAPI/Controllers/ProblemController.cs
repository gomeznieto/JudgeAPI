using JudgeAPI.Models;
using JudgeAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JudgeAPI.Controllers
{
    [ApiController]
    [Route("api/units/{unitId}/problems")]
    [Authorize]
    public class ProblemController : ControllerBase
    {
        private readonly IProblemService _problemService;

        public ProblemController(
            IProblemService problemService    
        )
        {
            _problemService = problemService;
        }

        // --- GET ALL PROBLEMS BY UNIT ---
        [HttpGet(Name = "GetAllProblems")]
        public async Task<ActionResult<UnitWithProblemsDTO>> GetByUnit(int unitId)
        {
            var response = await _problemService.GetUnitWithProblemsAsync(unitId);
            return Ok(response);
        }

        // --- GET PROBLEM BY UNIT AND ID ---
        [HttpGet("{id:int}", Name = "GetProblem")]
        public async Task<ActionResult<ProblemResponseDTO>> GetProblem(int id)
        {
            var problem = await _problemService.GetById(id);
            return Ok(problem);
        }

        // --- POST ---
        [HttpPost]
        public async Task<ActionResult<ProblemResponseDTO>> Post(int unitId, [FromBody]ProblemCreateDTO dto)
        {
            dto.UnitId = unitId;
            var responseDTO = await _problemService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetByUnit), new {id = responseDTO.Id, unitId = responseDTO.UnitId}, responseDTO);
        }

        // --- UPDATE ---
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProblemResponseDTO>> Put (int unitId, int id, ProblemUpdateDTO dto)
        {

            if (dto is null)
                return BadRequest("El cuerpo del mensaje no puede estar vacío");

            if (id != dto.Id)
                return BadRequest("El Id del body no coincide con la ruta");

            dto.UnitId = unitId;

            var problem = await _problemService.UpdateAsync(dto);

            return Ok(problem);
        }

        // --- DELETE ---
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete (int id, bool confirm = false)
        {
            if (!confirm)
                return BadRequest("Debe confirmar la eliminación con ?confirm=true");

            await _problemService.DeleteProblemAsync(id);

            return NoContent();
        }
    }
}
