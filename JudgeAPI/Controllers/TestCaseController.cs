using JudgeAPI.Models.TestCase;
using JudgeAPI.Services.TestCase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JudgeAPI.Controllers
{
    [ApiController]
    [Route("api/problems/{problemId}/testcases")]
    [Authorize] // TODO ADMIN
    public class TestCaseController : ControllerBase
    {
        private readonly ITestCaseService _testCaseService;

        public TestCaseController(
            ITestCaseService testCaseService
        )
        {
            _testCaseService = testCaseService;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public async Task<ActionResult<TestCaseResponseDTO>> GetById(int problemId, int id)
        {
            var response = await _testCaseService.GetTestCaseByIdAsync(problemId, id);
            return Ok(response);
        }

          [HttpPost("/api/problems/{problemId}/testcases")]
        public async Task<ActionResult<TestCaseResponseDTO>> Post(int problemId, [FromBody]TestCaseCreateDTO create)
        {
            Console.WriteLine("Entramos");
            var response = await _testCaseService.CreateTestCaseAsync(problemId, create);
            return CreatedAtAction(nameof(GetById), new { problemId, id = response.Id }, response);
        }

        [HttpGet("/api/problems/{problemId}/testcases", Name = "GetAllByProblem")]
        public async Task<ActionResult<List<TestCaseResponseDTO>>> GetAllByProblem (int problemId, [FromBody]bool onlySamples = false)
        {
            var testcases = await _testCaseService.GetTestCasesByProblemIdAsync(problemId, onlySamples);
            return Ok(testcases);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TestCaseResponseDTO>> Put(int id, [FromBody] TestCaseUpdateDTO update)
        {
            if(id != update.Id)
                return BadRequest("El Id del body no coincide con la ruta");

            var response = await _testCaseService.UpdateTestCaseAsync(update);
            return Ok(response);
        }

        [HttpPost("/api/problems/{problemId}/testcases/{id}/move-to-problem/{newProblemId}")]

        public async Task<ActionResult<TestCaseResponseDTO>> MoveTestCase(int problemId, int id, int newProblemId)
        {
            var response = await _testCaseService.MoveTestCaseAsync(problemId, id, newProblemId);
            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] bool confirm = false)
        {
            if (!confirm)
                return BadRequest("Debe confirmar la eliminación con ?confirm=true");

            await _testCaseService.DeleteTestCaseAsync(id);

            return NoContent();
        }

    }
}
