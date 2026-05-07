using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using JudgeAPI.Application.Features.Submissions.Interfaces;
using JudgeAPI.Application.Features.Submissions.Dtos;
using JudgeAPI.Application.Common.Exceptions;

namespace JudgeAPI.API.Controllers
{
    [ApiController]
    [Route("api/problems/{problemId}/submission")]
    [Authorize]
    public class SubmissionController(
            ISubmissionService submissionService
            )
 : ControllerBase
    {
        private readonly ISubmissionService _submissionService = submissionService;

        // --- POST SUBMISSION PROBLEM ---
        // POST: api/problems/{problemId}/submission
        // Auth: Bearer [Token]
        // Body {
        // "code": "#include <iostream>\nint main(){ int a, b; std::cin>>a>>b; std::cout<<a+b; return 0;",
        // "language": "C++"
        // }
        [HttpPost]
        public async Task<ActionResult<SubmissionResponseDTO>> Submit(int problemId, SubmissionCreateDTO create)
        {
            try
            {
                // Varificamos al usuario que nos manda el request
                string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                // Creamos Submission
                SubmissionResponseDTO submissionResponse = await _submissionService.CreateSubmissionAsync(userId, problemId, create);

                if (submissionResponse == null)
                {
                    return NotFound();
                }

                _ = await _submissionService.AnalyzeSubmissionAsync(submissionResponse.Id);

                return CreatedAtAction(nameof(GetSubmission), new { submissionId = submissionResponse.Id, problemId = problemId }, submissionResponse);
            }
            catch (SubmissionTooSoonException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        // --- GET SUBMISSION BY PROBLEM AND ID
        [HttpGet("{submissionId:int}")]
        public async Task<ActionResult<SubmissionResponseDTO>> GetSubmission(int submissionId)
        {
            SubmissionResponseDTO response = await _submissionService.GetSubmissionAsync(submissionId);

            if (response is null)
            {
                return NotFound();
            }

            return Ok(response.Results);
        }

    }
}
