using JudgeAPI.Models.Submission;
using JudgeAPI.Services.Submissions;
using JudgeAPI.Excerptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JudgeAPI.Controllers
{
    [ApiController]
    [Route("api/problems/{problemId}/submission")]
    [Authorize]
    public class SubmissionController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISubmissionService _submissionService;

        public SubmissionController(
            IHttpContextAccessor httpContextAccessor,
            ISubmissionService submissionService
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _submissionService = submissionService;
        }

        // --- POST SUBMISSION ---
        [HttpPost]
        public async Task<ActionResult<SubmissionResponseDTO>> Submit(int problemId, SubmissionCreateDTO create)
        {
          try {

            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
              return Unauthorized();

            var submissionResponse = await _submissionService.CreateSubmissionAsync(userId, problemId, create);

            // Enlistamos el submission en Redis para despertar al servicio que lo va a ejecutar
            if(submissionResponse == null)
              return NotFound();

            await _submissionService.AnalyzeSubmissionAsync(submissionResponse.Id);

            return CreatedAtAction(nameof(GetSubmission), new { submissionId = submissionResponse.Id, problemId = problemId }, submissionResponse);
          }catch(SubmissionTooSoonException ex){
            return Conflict(new {error = ex.Message});
          }
        }

        // --- GET SUBMISSION BY PROBLEM AND ID
        [HttpGet("{submissionId:int}")]
        public async Task<ActionResult<SubmissionResponseWrapper>> GetSubmission(int submissionId)
        {
          var response = await _submissionService.GetSubmissionAsync(submissionId);

          if (response == null)
            return NotFound();

          if (response.IsPending)
            return Ok(response.Summary);
          else
            return Ok(response.Results);
        }

    }
}
