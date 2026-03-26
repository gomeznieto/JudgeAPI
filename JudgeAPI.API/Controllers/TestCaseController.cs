using JudgeAPI.Application.Features.TestCases.Dtos;
using JudgeAPI.Application.Features.TestCases.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JudgeAPI.API.Controllers
{
    [ApiController]
    [Route("api/problems/{problemId}/testcases")]
    [Authorize] // TODO: ADMIN
    public class TestCaseController(
            ITestCaseService testCaseService
            )
        : ControllerBase
    {
        private readonly ITestCaseService _testCaseService = testCaseService;


        [HttpGet("{id:int}", Name = "GetById")]
        public async Task<ActionResult<TestCaseResponseDTO>> GetById(int problemId, int id)
        {
            TestCaseResponseDTO response = await _testCaseService.GetTestCaseByIdAsync(problemId, id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<TestCaseResponseDTO>> Post(int problemId, [FromBody] TestCaseCreateDTO create)
        {
            Console.WriteLine("Entramos");
            TestCaseResponseDTO response = await _testCaseService.CreateTestCaseAsync(problemId, create);
            return CreatedAtAction(nameof(GetById), new { problemId, id = response.Id }, response);
        }

        [HttpGet(Name = "GetAllByProblem")]
        public async Task<ActionResult<List<TestCaseResponseDTO>>> GetAllByProblem(int problemId, [FromBody] bool onlySamples = false)
        {
            List<TestCaseResponseDTO> testcases = await _testCaseService.GetTestCasesByProblemIdAsync(problemId, onlySamples);
            return Ok(testcases);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TestCaseResponseDTO>> Put(int id, [FromBody] TestCaseUpdateDTO update)
        {
            if (id != update.Id)
            {
                return BadRequest("El Id del body no coincide con la ruta");
            }

            TestCaseResponseDTO response = await _testCaseService.UpdateTestCaseAsync(update);
            return Ok(response);
        }

        [HttpPost("{id}/move-to-problem/{newProblemId}")]
        public async Task<ActionResult<TestCaseResponseDTO>> MoveTestCase(int problemId, int id, int newProblemId)
        {
            TestCaseResponseDTO response = await _testCaseService.MoveTestCaseAsync(problemId, id, newProblemId);
            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, int problemId, [FromQuery] bool confirm = false)
        {
            if (!confirm)
            {
                return BadRequest("Debe confirmar la eliminación con ?confirm=true");
            }

            await _testCaseService.DeleteTestCaseAsync(problemId, id);
            return NoContent();
        }

    }
}
